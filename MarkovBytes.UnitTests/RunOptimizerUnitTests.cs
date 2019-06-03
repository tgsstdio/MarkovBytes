using System;
using Markov;
using NUnit.Framework;

namespace MarkovBytes.UnitTests
{
    [TestFixture]
    public class RunOptimizerUnitTests
    {
        [Test]
        public void ZeroMatrix()
        {
            var source = new ushort[]
            {
                new {0 , 0 ,0 , 0},
                new {0 , 0 ,0 , 0},
                new {0 , 0 ,0 , 0},
                new {0 , 0 ,0 , 0},
            };

            IMatrixOptimizer optimizer = new MatrixOptimizer
            {
                MaxProbability = 100
            };
        }
    }
}
