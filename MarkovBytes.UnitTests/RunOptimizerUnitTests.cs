using System;
using Markov;
using Moq;
using NUnit.Framework;

namespace MarkovBytes.UnitTests
{
    [TestFixture]
    public class RunOptimizerUnitTests
    {
        [Test]
        public void ZeroMatrix()
        {
            var source = new ushort[][]
            {
                new ushort[]{0 , 0 ,0 , 0},
                new ushort[]{0 , 0 ,0 , 0},
                new ushort[]{0 , 0 ,0 , 0},
                new ushort[]{0 , 0 ,0 , 0},
            };

            const ushort MAX_PROBABILITY = 100;
            var slicer = new Mock<ISlicer>();
            var secondary = new Mock<IRowValleyOptimizer>();
            var optimizer = new MatrixOptimizer(slicer.Object, secondary.Object);


            ushort[] rowDenominators = new[] {
                MAX_PROBABILITY,
                MAX_PROBABILITY,
                MAX_PROBABILITY,
                MAX_PROBABILITY,
            };
            var solution = optimizer.Optimize(rowDenominators, source);
            Assert.IsNotNull(solution);
            Assert.IsNotNull(solution.Rows);
            Assert.IsTrue(solution.IsOptimized);

            foreach(var row in solution.Rows)
            {
                Assert.IsNotNull(row);
                Assert.AreEqual(SolutionType.NoOperation, row.Approach);
            }
        }

        [Test]
        public void IdentityMatrix()
        {
            var source = new ushort[4,4]
            {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 },
            };

            var slicer = new Mock<ISlicer>();
            var secondary = new Mock<IRowValleyOptimizer>();
            var optimizer = new MatrixOptimizer(slicer.Object, secondary.Object);

            const ushort MaxProbability = 1;

            var rowDenominators = new[] { MaxProbability, MaxProbability, MaxProbability, MaxProbability };

            var solution = optimizer.Optimize(rowDenominators, source);
            Assert.IsNotNull(solution);

            Assert.AreEqual(4, solution.NoOfStates);
            Assert.IsNotNull(solution.Rows);
            Assert.IsTrue(solution.IsOptimized);

            // SHOULD BE ALL 

            for (var i = 0; i < 4; i++)
            {
                var row = solution.Rows[i];
                Assert.IsNotNull(row);
                Assert.AreEqual(SolutionType.DeadEnd, row.Approach);
                Assert.AreEqual(i, row.Branch);
            }
        }
    }
}
