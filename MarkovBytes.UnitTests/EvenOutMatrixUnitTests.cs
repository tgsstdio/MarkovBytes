using Markov;
using Moq;
using NUnit.Framework;
using System.Collections;

namespace MarkovBytes.UnitTests
{
    class EvenOutMatrixUnitTests
    {
        private MatrixSolution mSolution;

        [SetUp]
        public void Setup()
        {
            var source = new ushort[4, 4]
{
                { 0, 1, 1, 1 },
                { 2, 0, 2, 2 },
                { 3, 3, 0, 3 },
                { 6, 6, 6, 0 },
};

            var slicer = new Mock<ISlicer>();
            var secondary = new Mock<IRowValleyOptimizer>();
            var optimizer = new MatrixOptimizer(slicer.Object, secondary.Object);

            var rowDenominators = new ushort[] { 3, 6, 9, 18 };

            mSolution = optimizer.Optimize(rowDenominators, source);
        }

        [Test]
        public void SetupRight()
        { 
            Assert.That(mSolution.Rows, Is.Not.Null);
            Assert.That(mSolution.Rows.Length, Is.EqualTo(4));
            Assert.That(mSolution.Rows[0].Approach, Is.EqualTo(SolutionType.EvenOut));
            Assert.That(mSolution.Rows[1].Approach, Is.EqualTo(SolutionType.EvenOut));
            Assert.That(mSolution.Rows[2].Approach, Is.EqualTo(SolutionType.EvenOut));
            Assert.That(mSolution.Rows[3].Approach, Is.EqualTo(SolutionType.EvenOut));
        }

        [Test]
        [TestCaseSource(nameof(SafeValues))]
        public int RunSolver(int pastState, int nextValue)
        {
            var rng = new Mock<IRandomNumberGenerator>();
            rng.Setup(ev => ev.Next(It.IsAny<int>())).Returns(nextValue);
            var analyser = new Mock<IBitAnalyser>();
            var evaluator = new Mock<INodeEvaluator>();
            var stepper = new Stepper(evaluator.Object, analyser.Object);
            var solver = new Solver(rng.Object, stepper);

            if (solver.Resolve(mSolution, pastState, out int next))
                return next;
            else
                return -1;

        }

        [TearDown]
        public void Dispose()
        {
            mSolution = null;
        }

        public static IEnumerable SafeValues
        {
            get
            {
                yield return new TestCaseData(0, 0).Returns(1);
                yield return new TestCaseData(0, 1).Returns(2);
                yield return new TestCaseData(0, 2).Returns(3);
                yield return new TestCaseData(0, 3).Returns(3);

                yield return new TestCaseData(1, 0).Returns(2);
                yield return new TestCaseData(1, 1).Returns(2);
                yield return new TestCaseData(1, 2).Returns(3);
                yield return new TestCaseData(1, 3).Returns(3);
                yield return new TestCaseData(1, 4).Returns(0);
                yield return new TestCaseData(1, 5).Returns(0);
                yield return new TestCaseData(1, 6).Returns(0);

                yield return new TestCaseData(2, 0).Returns(3);
                yield return new TestCaseData(2, 1).Returns(3);
                yield return new TestCaseData(2, 2).Returns(3);
                yield return new TestCaseData(2, 3).Returns(0);
                yield return new TestCaseData(2, 4).Returns(0);
                yield return new TestCaseData(2, 5).Returns(0);
                yield return new TestCaseData(2, 6).Returns(1);
                yield return new TestCaseData(2, 7).Returns(1);
                yield return new TestCaseData(2, 8).Returns(1);
                yield return new TestCaseData(2, 9).Returns(1);
                yield return new TestCaseData(2, 10).Returns(1);

                yield return new TestCaseData(3, 0).Returns(0);
                yield return new TestCaseData(3, 1).Returns(0);
                yield return new TestCaseData(3, 2).Returns(0);
                yield return new TestCaseData(3, 3).Returns(0);
                yield return new TestCaseData(3, 4).Returns(0);
                yield return new TestCaseData(3, 5).Returns(0);

                yield return new TestCaseData(3, 6).Returns(1);
                yield return new TestCaseData(3, 7).Returns(1);
                yield return new TestCaseData(3, 8).Returns(1);
                yield return new TestCaseData(3, 9).Returns(1);
                yield return new TestCaseData(3, 10).Returns(1);
                yield return new TestCaseData(3, 11).Returns(1);

                yield return new TestCaseData(3, 12).Returns(2);
                yield return new TestCaseData(3, 13).Returns(2);
                yield return new TestCaseData(3, 14).Returns(2);
                yield return new TestCaseData(3, 15).Returns(2);
                yield return new TestCaseData(3, 16).Returns(2);
                yield return new TestCaseData(3, 17).Returns(2);

                yield return new TestCaseData(3, 18).Returns(2);

                // yield return new TestCaseData(-1).Returns(-1);
                // yield return new TestCaseData(4).Returns(-1);
            }
        }
    }
}
