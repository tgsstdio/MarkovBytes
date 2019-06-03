using System;
using Markov;
using NUnit.Framework;

namespace MarkovBytes.UnitTests
{

    [TestFixture]
    public partial class OptimizeUnitTests
    {
        [Test]
        public void CheckAllZeros_SingleRow()
        {
            var optimizer = new MatrixOptimizer();

            // TAKE N x N matrix
            const int COUNT = 8;
            var row = new ushort[COUNT];
            var result = optimizer.Investigate(0, row);

            Assert.AreEqual(COUNT, result.NoOfStates);
            Assert.AreEqual(0, result.NoOfNonZeroPercents);
            Assert.AreEqual(COUNT, result.NoOfZeroPercents);
            Assert.IsFalse(result.SelfPercent.HasValue);
        }

        [Test]
        public void Evaluate_NoOperation()
        {
            var stats = new MatrixRowSummary
            {
                NoOfStates = 4,
                NoOfZeroPercents = 4
            };

            var optimizer = new MatrixOptimizer();
            var result = optimizer.Evaluate(stats);
            Assert.AreEqual(SolutionType.NoOperation, result.Approach);
        }

        [Test]
        public void Evaluate_Redirect_0()
        {

            const ushort MAX_VALUE = ushort.MaxValue;
            var optimizer = new MatrixOptimizer
            {
                MaxProbability = MAX_VALUE
            };

            const int EXPECTED_RESULT = 2;
            const int OTHER_BRANCH = 1;

            var stats = new MatrixRowSummary
            {
                SelfPercent = null,
                Clusters = new[] {
                    new ValueCluster
                    {
                        Value = MAX_VALUE,
                        First = EXPECTED_RESULT,
                        NoOfTimes = 1,
                    }
                },
                Row = OTHER_BRANCH,
                NoOfStates = 2,
                NoOfNonZeroPercents = 1,
                NoOfZeroPercents = 1,
            };
   

            var result = optimizer.Evaluate(stats);
            Assert.AreEqual(SolutionType.Redirect, result.Approach);
            Assert.AreEqual(EXPECTED_RESULT, result.Branch);
        }

        [Test]
        public void Evaluate_Redirect_1()
        {


            const int EXPECTED_RESULT = 3;

            const ushort MAX_VALUE = ushort.MaxValue;
            var optimizer = new MatrixOptimizer
            {
                MaxProbability = MAX_VALUE
            };

            const int OTHER = 1;
            var stats = new MatrixRowSummary
            {
                SelfPercent = null,
                Row = OTHER,
                Clusters = new[] {
                    new ValueCluster {
                        Value = MAX_VALUE,
                        First = EXPECTED_RESULT,
                        NoOfTimes = 1,
                    }
                },
                NoOfStates = 2,
                NoOfNonZeroPercents = 1,
                NoOfZeroPercents = 1
            };

            var result = optimizer.Evaluate(stats);
            Assert.AreEqual(SolutionType.Redirect, result.Approach);
            Assert.AreEqual(EXPECTED_RESULT, result.Branch);
        }

        [Test]
        public void Evaluate_DeadEnd_0()
        {
            const ushort MAX_VALUE = ushort.MaxValue;
            var optimizer = new MatrixOptimizer
            {
                MaxProbability = MAX_VALUE
            };

            const int EXPECTED_RESULT = 3;
            var stats = new MatrixRowSummary
            {
                SelfPercent = 100,
                Clusters = new[] { 
                    new ValueCluster {
                        Value = MAX_VALUE,
                        First = EXPECTED_RESULT,
                        NoOfTimes = 1,
                    }
                },
                Row = EXPECTED_RESULT,
                NoOfStates = 2,
                NoOfNonZeroPercents = 1,
                NoOfZeroPercents = 1
            };

            var result = optimizer.Evaluate(stats);
            Assert.AreEqual(SolutionType.DeadEnd, result.Approach);
            Assert.AreEqual(EXPECTED_RESULT, result.Branch);
        }

        [Test]
        public void Evaluate_Unoptimized_0()
        {
            const int PERCENT_0 = 65;
            const int PERCENT_1 = 35;

            var stats = new MatrixRowSummary
            {
                SelfPercent = PERCENT_0,
                NoOfStates = 4,
                NoOfNonZeroPercents = 2,
                NoOfZeroPercents = 2,
                Clusters = new ValueCluster[] {
                    new ValueCluster
                    {
                        Value = PERCENT_0,
                    },
                    new ValueCluster
                    {
                        Value = PERCENT_1,
                    },
                },
            };

            var optimizer = new MatrixOptimizer
            {
                MaxProbability = 100
            };
            var result = optimizer.Evaluate(stats);
            Assert.AreEqual(SolutionType.Unoptimized, result.Approach);
        }

        [Test]
        public void Evaluate_Unoptimized_1()
        {
            const int PERCENT_0 = 65;
            const int PERCENT_1 = 25;
            const int PERCENT_2 = 10;

            var stats = new MatrixRowSummary
            {
                SelfPercent = PERCENT_0,
                NoOfStates = 5,
                NoOfNonZeroPercents = 3,
                NoOfZeroPercents = 2,
                Clusters = new ValueCluster[] {
                    new ValueCluster
                    {
                        Value = PERCENT_0,
                    },
                    new ValueCluster
                    {
                        Value = PERCENT_1,
                    },
                    new ValueCluster
                    {
                        Value = PERCENT_2,
                    },
                },
            };

            var optimizer = new MatrixOptimizer
            {
                MaxProbability = 100
            };
            var result = optimizer.Evaluate(stats);
            Assert.AreEqual(SolutionType.Unoptimized, result.Approach);
        }

        [Test]
        public void CheckAlNonZeros_SingleRow()
        {
            // TAKE N x N matrix
            const int COUNT = 4;
            const ushort SELF_PERCENT = 100;
            var row = new ushort[] { SELF_PERCENT, 200, 300, 400 };
            var optimizer = new MatrixOptimizer();
            var result = optimizer.Investigate(0, row);

            Assert.AreEqual(COUNT, result.NoOfStates);
            Assert.AreEqual(COUNT, result.NoOfNonZeroPercents);
            Assert.AreEqual(0, result.NoOfZeroPercents);
            Assert.IsTrue(result.SelfPercent.HasValue);
            Assert.AreEqual(SELF_PERCENT, result.SelfPercent.Value);
        }

        [Test]
        public void CheckRecords_0()
        {
            // TAKE N x N matrix
            const int COUNT = 4;
            const int EXPECTED_1 = 1000;
            const int EXPECTED_2 = 400;
            const int EXPECTED_3 = 200;
            const int EXPECTED_4 = 50;
            var row = new ushort[] { EXPECTED_4, EXPECTED_3, EXPECTED_1, EXPECTED_2 };
            var optimizer = new MatrixOptimizer();
            var result = optimizer.Investigate(0, row);

            Assert.AreEqual(COUNT, result.NoOfStates);
            Assert.AreEqual(COUNT, result.NoOfNonZeroPercents);
            Assert.AreEqual(0, result.NoOfZeroPercents);

            Assert.IsNotNull(result.Clusters);
            Assert.AreEqual(COUNT, result.Clusters.Length);

            {
                var record = result.Clusters[0];

                Assert.AreEqual(EXPECTED_1, record.Value);
                Assert.AreEqual(2, record.First);
                Assert.AreEqual(1, record.NoOfTimes);
            }

            {
                var record = result.Clusters[1];

                Assert.AreEqual(EXPECTED_2, record.Value);
                Assert.AreEqual(3, record.First);
                Assert.AreEqual(1, record.NoOfTimes);
            }

            {
                var record = result.Clusters[2];

                Assert.AreEqual(EXPECTED_3, record.Value);
                Assert.AreEqual(1, record.First);
                Assert.AreEqual(1, record.NoOfTimes);
            }

            {
                var record = result.Clusters[3];

                Assert.AreEqual(EXPECTED_4, record.Value);
                Assert.AreEqual(0, record.First);
                Assert.AreEqual(1, record.NoOfTimes);
            }
        }

        [Test]
        public void CheckRecords_1()
        {
            // TAKE N x N matrix
            const int COUNT = 3;
            const int EXPECTED_1 = 1000;
            var row = new ushort[] { EXPECTED_1, EXPECTED_1, EXPECTED_1 };
            var optimizer = new MatrixOptimizer();
            var result = optimizer.Investigate(0, row);

            Assert.AreEqual(COUNT, result.NoOfStates);
            Assert.AreEqual(COUNT, result.NoOfNonZeroPercents);
            Assert.AreEqual(0, result.NoOfZeroPercents);

            Assert.IsNotNull(result.Clusters);
            Assert.AreEqual(1, result.Clusters.Length);

            {
                var record = result.Clusters[0];

                Assert.AreEqual(EXPECTED_1, record.Value);
                Assert.AreEqual(0, record.First);
                Assert.AreEqual(COUNT, record.NoOfTimes);
            }
        }

        [Test]
        public void CheckRecords_Empty()
        {
            // TAKE N x N matrix
            const int COUNT = 0;
            var row = new ushort[] { };
            var optimizer = new MatrixOptimizer();
            var result = optimizer.Investigate(0, row);

            Assert.AreEqual(COUNT, result.NoOfStates);
            Assert.AreEqual(COUNT, result.NoOfNonZeroPercents);
            Assert.AreEqual(0, result.NoOfZeroPercents);

            Assert.IsNotNull(result.Clusters);
            Assert.AreEqual(0, result.Clusters.Length);
        }
    }
}
