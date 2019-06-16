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

            var machine = new Solver(
                rng.Object,
                new MatrixSolution
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
                }
            );
            Assert.IsTrue(machine.Resolve(0, out int next));
            Assert.AreEqual(0, next);
            Assert.IsTrue(machine.Resolve(0, out int next1));
            Assert.AreEqual(0, next1);
        }

        [Test]
        public void Deadend_1()
        {
            var rng = new Mock<IRandomNumberGenerator>();

            var machine = new Solver(
                rng.Object,
                new MatrixSolution
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
                }
            );
            Assert.IsTrue(machine.Resolve(1, out int next));
            Assert.AreEqual(1, next);
            Assert.IsTrue(machine.Resolve(1, out int next1));
            Assert.AreEqual(1, next1);
        }
    }
}