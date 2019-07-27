using Markov;
using Moq;
using NUnit.Framework;
using System.Collections;

namespace MarkovBytes.UnitTests
{
    class RowValleySolverUnitTests
    {
        [Test]
        [TestCaseSource(nameof(NextState))]
        public int RunSolver(int randValue, int maxRand, int peak, int cutoff, int left, int domain, int arrayLength)
        {
            return Solver.GetRowValley(randValue, peak, cutoff, maxRand, left, domain, arrayLength);
        }
        public static IEnumerable NextState
        {
            get
            {
                // PEAK BEFORE ISLAND - FULL RANGE
                yield return new TestCaseData(0, 6, 0, 2, 1, 4, 5).Returns(0);
                yield return new TestCaseData(1, 6, 0, 2, 1, 4, 5).Returns(0);
                yield return new TestCaseData(2, 6, 0, 2, 1, 4, 5).Returns(1);
                yield return new TestCaseData(3, 6, 0, 2, 1, 4, 5).Returns(2);
                yield return new TestCaseData(4, 6, 0, 2, 1, 4, 5).Returns(3);
                yield return new TestCaseData(5, 6, 0, 2, 1, 4, 5).Returns(4);

                // PEAK BEFORE ISLAND - SMALL RANGE
                yield return new TestCaseData(0, 4, 0, 2, 3, 4, 5).Returns(0);
                yield return new TestCaseData(1, 4, 0, 2, 3, 4, 5).Returns(0);
                yield return new TestCaseData(2, 4, 0, 2, 3, 4, 5).Returns(3);
                yield return new TestCaseData(3, 4, 0, 2, 3, 4, 5).Returns(4);

                // MID PEAK VALLEY WRAP
                yield return new TestCaseData(0, 4, 2, 2, 4, 0, 5).Returns(2);
                yield return new TestCaseData(1, 4, 2, 2, 4, 0, 5).Returns(2);
                yield return new TestCaseData(2, 4, 2, 2, 4, 0, 5).Returns(4);
                yield return new TestCaseData(3, 4, 2, 2, 4, 0, 5).Returns(0);

                // PEAK AFTER VALLEY - SMALL RANGE
                yield return new TestCaseData(0, 4, 4, 2, 1, 2, 5).Returns(4);
                yield return new TestCaseData(1, 4, 4, 2, 1, 2, 5).Returns(4);
                yield return new TestCaseData(2, 4, 4, 2, 1, 2, 5).Returns(1);
                yield return new TestCaseData(3, 4, 4, 2, 1, 2, 5).Returns(2);

                // PEAK IN VALLEY - FULL RANGE
                yield return new TestCaseData(0, 6, 2, 2, 3, 1, 5).Returns(2);
                yield return new TestCaseData(1, 6, 2, 2, 3, 1, 5).Returns(2);
                yield return new TestCaseData(2, 6, 2, 2, 3, 1, 5).Returns(3);
                yield return new TestCaseData(3, 6, 2, 2, 3, 1, 5).Returns(4);
                yield return new TestCaseData(4, 6, 2, 2, 3, 1, 5).Returns(0);
                yield return new TestCaseData(5, 6, 2, 2, 3, 1, 5).Returns(1);
            }
        }

    }
}
