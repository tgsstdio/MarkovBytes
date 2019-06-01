using System;
using System.Collections.Generic;
using Markov;
using NUnit.Framework;

namespace MarkovBytes.UnitTests
{

    [TestFixture]
    public partial class OptimizeUnitTests
    {
        class HigherPercentComparer : Comparer<ushort>
        {
            public override int Compare(ushort x, ushort y)
            {
                if (x > y)
                    return -1;

                if (x < y)
                    return 1;

                return 0;
            }
        }

        public RowStats Investigate(int self, ushort[] row)
        {
            // Check if all zeros
            int noOfStates = row.Length;

            ushort? selfPercent = null;
            int noOfZeroPercents = 0;
            int noOfNonZeroPercents = 0;
            var lookup = new SortedDictionary<ushort, RowRecord>(
                new HigherPercentComparer());

            for (int i = 0; i < noOfStates; i += 1)
            {
                ushort percent = row[i];
                if (i == self)
                {
                    selfPercent = percent;
                }

                if (percent == 0)
                {
                    noOfZeroPercents += 1;
                }
                else
                {
                    if (lookup.TryGetValue(percent, out RowRecord found))
                    {
                        found.NoOfTimes += 1;
                    }
                    else
                    {
                        found = new RowRecord
                        {
                            Value = percent,
                            First = i,
                            NoOfTimes = 1,
                        };

                        lookup.Add(percent, found);
                    }


                    noOfNonZeroPercents += 1;
                }
            }

            var records = new RowRecord[lookup.Count];
            lookup.Values.CopyTo(records, 0);

            return new RowStats
            {
                Row = self,
                Records = records,
                SelfPercent = selfPercent,
                NoOfStates = noOfStates,
                NoOfZeroPercents = noOfZeroPercents,
                NoOfNonZeroPercents = noOfNonZeroPercents,
            };
        }

        public RowSolution Evaluate(RowStats stats)
        {
            if (stats.NoOfStates == stats.NoOfZeroPercents)
            {
                return new RowSolution
                {
                    Approach = SolutionType.NoOperation,
                };
            }
            else if (stats.NoOfNonZeroPercents == 1)
            {
                var top = stats.Records[0];
                int first = top.First;

                if (stats.Row == first)
                {
                    return new RowSolution
                    {
                        Approach = SolutionType.DeadEnd,
                        Branch = first,
                    };
                }
                else
                {
                    return new RowSolution
                    {
                        Approach = SolutionType.Redirect,
                        Branch = first,
                    };
                }
            }
            else
            {
                return new RowSolution
                {
                    Approach = SolutionType.Unoptimized,
                };
            }
        }

        [Test]
        public void CheckAllZeros_SingleRow()
        {
            // TAKE N x N matrix
            const int COUNT = 8;
            var row = new ushort[COUNT];
            var result = Investigate(0, row);

            Assert.AreEqual(COUNT, result.NoOfStates);
            Assert.AreEqual(0, result.NoOfNonZeroPercents);
            Assert.AreEqual(COUNT, result.NoOfZeroPercents);
            Assert.IsTrue(result.SelfPercent.HasValue);
            Assert.AreEqual(0, result.SelfPercent.Value);
        }

        [Test]
        public void Evaluate_NoOperation()
        {
            var stats = new RowStats();
            stats.NoOfStates = 4;
            stats.NoOfZeroPercents = 4;

            var result = Evaluate(stats);
            Assert.AreEqual(SolutionType.NoOperation, result.Approach);
        }

        [Test]
        public void Evaluate_Redirect_0()
        {
            var stats = new RowStats();
            const int EXPECTED_RESULT = 2;
            stats.Records = new[] { new RowRecord { First = EXPECTED_RESULT } };
            const int OTHER = 1;
            stats.Row = OTHER;

            stats.NoOfStates = 1;
            stats.NoOfNonZeroPercents = 1;
            stats.NoOfZeroPercents = 0;

            var result = Evaluate(stats);
            Assert.AreEqual(SolutionType.Redirect, result.Approach);
            Assert.AreEqual(EXPECTED_RESULT, result.Branch);
        }

        [Test]
        public void Evaluate_Redirect_1()
        {
            var stats = new RowStats();

            const int EXPECTED_RESULT = 3;

            const int OTHER = 1;
            stats.Row = OTHER;
            stats.Records = new[] { new RowRecord { First = EXPECTED_RESULT } };
            stats.NoOfStates = 2;
            stats.NoOfNonZeroPercents = 1;
            stats.NoOfZeroPercents = 1;

            var result = Evaluate(stats);
            Assert.AreEqual(SolutionType.Redirect, result.Approach);
            Assert.AreEqual(EXPECTED_RESULT, result.Branch);
        }

        [Test]
        public void Evaluate_DeadEnd_0()
        {
            var stats = new RowStats();
            const int EXPECTED_RESULT = 3;
            stats.Records = new[] { new RowRecord { First = EXPECTED_RESULT } };
            stats.Row = EXPECTED_RESULT;
            stats.NoOfStates = 2;
            stats.NoOfNonZeroPercents = 1;
            stats.NoOfZeroPercents = 1;

            var result = Evaluate(stats);
            Assert.AreEqual(SolutionType.DeadEnd, result.Approach);
            Assert.AreEqual(EXPECTED_RESULT, result.Branch);
        }

        [Test]
        public void Evaluate_Unoptimized()
        {
            var stats = new RowStats();
            stats.NoOfStates = 3;
            stats.NoOfZeroPercents = 2;

            var result = Evaluate(stats);
            Assert.AreEqual(SolutionType.Unoptimized, result.Approach);
        }

        [Test]
        public void CheckAlNonZeros_SingleRow()
        {
            // TAKE N x N matrix
            const int COUNT = 4;
            const ushort SELF_PERCENT = 100;
            var row = new ushort[] { SELF_PERCENT, 200, 300, 400 };
            var result = Investigate(0, row);

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
            var result = Investigate(0, row);

            Assert.AreEqual(COUNT, result.NoOfStates);
            Assert.AreEqual(COUNT, result.NoOfNonZeroPercents);
            Assert.AreEqual(0, result.NoOfZeroPercents);

            Assert.IsNotNull(result.Records);
            Assert.AreEqual(COUNT, result.Records.Length);

            {
                var record = result.Records[0];

                Assert.AreEqual(EXPECTED_1, record.Value);
                Assert.AreEqual(2, record.First);
                Assert.AreEqual(1, record.NoOfTimes);
            }

            {
                var record = result.Records[1];

                Assert.AreEqual(EXPECTED_2, record.Value);
                Assert.AreEqual(3, record.First);
                Assert.AreEqual(1, record.NoOfTimes);
            }

            {
                var record = result.Records[2];

                Assert.AreEqual(EXPECTED_3, record.Value);
                Assert.AreEqual(1, record.First);
                Assert.AreEqual(1, record.NoOfTimes);
            }

            {
                var record = result.Records[3];

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
            var result = Investigate(0, row);

            Assert.AreEqual(COUNT, result.NoOfStates);
            Assert.AreEqual(COUNT, result.NoOfNonZeroPercents);
            Assert.AreEqual(0, result.NoOfZeroPercents);

            Assert.IsNotNull(result.Records);
            Assert.AreEqual(1, result.Records.Length);

            {
                var record = result.Records[0];

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
            var result = Investigate(0, row);

            Assert.AreEqual(COUNT, result.NoOfStates);
            Assert.AreEqual(COUNT, result.NoOfNonZeroPercents);
            Assert.AreEqual(0, result.NoOfZeroPercents);

            Assert.IsNotNull(result.Records);
            Assert.AreEqual(0, result.Records.Length);
        }
    }
}
