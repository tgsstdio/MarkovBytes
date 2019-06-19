using Markov;
using Moq;
using NUnit.Framework;

namespace MarkovBytes.UnitTests
{
    public class DeadEndUnitTests
    {

        [Test]
        public void Deadend_0()
        {
            var rng = new Mock<IRandomNumberGenerator>();
            var evaluator = new Mock<INodeEvaluator>();
            var bitAnalyser = new Mock<IBitAnalyser>();
            var stepper = new Stepper(evaluator.Object, bitAnalyser.Object);

            MatrixSolution matrixSolution = new MatrixSolution
            {
                IsOptimized = true,
                Rows = new[]
                    {
                        new MatrixRowSolution
                        {
                            Branch = 0,
                            Approach = SolutionType.DeadEnd,
                        },
                    }
            };
            var machine = new Solver(
                rng.Object,
                stepper
            );
            Assert.IsTrue(machine.Resolve(matrixSolution, 0, out int next));
            Assert.AreEqual(0, next);
            Assert.IsTrue(machine.Resolve(matrixSolution, 0, out int next1));
            Assert.AreEqual(0, next1);
        }

        [Test]
        public void Deadend_1()
        {
            var rng = new Mock<IRandomNumberGenerator>();
            var evaluator = new Mock<INodeEvaluator>();
            var bitAnalyser = new Mock<IBitAnalyser>();
            var stepper = new Stepper(evaluator.Object, bitAnalyser.Object);

            var solution = new MatrixSolution
            {

                IsOptimized = true,
                Rows = new[]
                    {
                        new MatrixRowSolution
                        {
                            Branch = 0,
                            Approach = SolutionType.DeadEnd,
                        },
                        new MatrixRowSolution
                        {
                            Branch = 1,
                            Approach = SolutionType.DeadEnd,
                        },
                    }
            };

            var machine = new Solver(
                rng.Object,
                stepper
            );
            Assert.IsTrue(machine.Resolve(solution, 1, out int next));
            Assert.AreEqual(1, next);
            Assert.IsTrue(machine.Resolve(solution, 1, out int next1));
            Assert.AreEqual(1, next1);
        }
    }
}