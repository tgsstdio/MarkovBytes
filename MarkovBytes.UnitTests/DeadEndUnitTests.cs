using Markov;
using NUnit.Framework;

namespace MarkovBytes.UnitTests
{
    public class DeadEndUnitTests
    {

        [Test]
        public void Test1()
        {
            var shortcut = new Solution
            {
                Id = 0,
                Approach = SolutionType.DeadEnd,
            };

            var machine = new Solver(new[] { shortcut });
            Assert.IsTrue(machine.Resolve(0, out ushort next));
            Assert.AreEqual(0, next);
            Assert.IsTrue(machine.Resolve(0, out ushort next1));
            Assert.AreEqual(0, next1);
        }
    }
}