using NUnit.Framework;

namespace Tests
{
    public partial class DeadEndUnitTests
    {

        [Test]
        public void Test1()
        {
            var shortcut = new Solution<int>
            {
                Id = 0,
                Approach = SolutionType.DeadEnd,
            };

            var machine = new Solver<int>(new[] { shortcut });
            Assert.IsTrue(machine.Resolve(0, out int next));
            Assert.AreEqual(0, next);
            Assert.IsTrue(machine.Resolve(0, out int next1));
            Assert.AreEqual(0, next1);
        }
    }
}