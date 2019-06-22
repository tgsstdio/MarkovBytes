using Markov;
using Moq;
using NUnit.Framework;
using System.Collections;

namespace MarkovBytes.UnitTests
{
    public class IdentityMatrixUnitTests
    {
        private MatrixSolution mSolution;

        [SetUp]
        public void Setup()
        {
            var source = new ushort[4, 4]
{
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 },
};

            var slicer = new Mock<ISlicer>();
            var optimizer = new MatrixOptimizer(slicer.Object);

            const ushort MaxProbability = 1;

            var rowDenominators = new[] { MaxProbability, MaxProbability, MaxProbability, MaxProbability };

            mSolution = optimizer.Optimize(rowDenominators, source);
        }

        [Test]
        [TestCaseSource(nameof(SafeValues))]
        public int RunSolver(int pastState)
        {
            var rng = new Mock<IRandomNumberGenerator>();
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
                yield return new TestCaseData(0U).Returns(0U);
                yield return new TestCaseData(1U).Returns(1U);
                yield return new TestCaseData(2U).Returns(2U);
                yield return new TestCaseData(3U).Returns(3U);
                yield return new TestCaseData(-1).Returns(-1);
                yield return new TestCaseData(4).Returns(-1);
            }
        }
    }
}
