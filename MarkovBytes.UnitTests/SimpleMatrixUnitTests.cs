using Markov;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;

namespace MarkovBytes.UnitTests
{
    public class SimpleMatrixUintTess
    {
        private MatrixSolution mSolution;

        [SetUp]
        public void Setup()
        {
            var source = new ushort[4, 4]
{
                { 1, 1, 1, 1 }, // EVEN ALL
                { 0, 1, 0, 0 }, // DEAD END
                { 0, 100, 0, 0 }, // REDIRECT
                { 0, 0, 0, 0 },  // NO-OP
};

            var slicer = new Mock<ISlicer>();
            var secondary = new Mock<IRowValleyOptimizer>();
            var optimizer = new MatrixOptimizer(slicer.Object, secondary.Object);

            var rowDenominators = new ushort[] { 4, 1, 100, 1 };

            mSolution = optimizer.Optimize(rowDenominators, source);
        }

        [Test]
        [TestCaseSource(nameof(SafeValues))]
        public int RunSolver(int pastState, int randValue)
        {
            var rng = new Mock<IRandomNumberGenerator>();
            rng.Setup(a => a.Next(It.IsAny<int>()))
                .Returns(randValue);
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
                // EVEN ALL
                yield return new TestCaseData(0, 0).Returns(0);
                yield return new TestCaseData(0, 1).Returns(1);
                yield return new TestCaseData(0, 2).Returns(2);
                yield return new TestCaseData(0, 3).Returns(3);
                yield return new TestCaseData(0, 4).Returns(3);

                // DEAD END
                yield return new TestCaseData(1, 0).Returns(1);
                yield return new TestCaseData(1, 1).Returns(1);

                // REDIRECT
                yield return new TestCaseData(2, 1).Returns(1);
                yield return new TestCaseData(2, 100).Returns(1);

                // 
                yield return new TestCaseData(3, 1).Returns(-1);
                // yield return new TestCaseData(4).Returns(-1);
            }
        }
    }
}
