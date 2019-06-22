using Markov;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MarkovBytes.UnitTests
{
    public class IslandValleyOptimizerUnitTests
    {
        [Test]
        [TestCaseSource(nameof(StatusData))]
        public IslandOptimizationStatus Test_Status(int peakIndex)
        {
            var input = new bool[]
            {
                true, true, false,
            };

            var actual = IslandValleyOptimizer.Examine(input, peakIndex);
            return actual.Status;
        }

        public static IEnumerable StatusData()
        {
            yield return new TestCaseData(0)
                .Returns(IslandOptimizationStatus.Invalid);
            yield return new TestCaseData(1)
                .Returns(IslandOptimizationStatus.Invalid);
            yield return new TestCaseData(2)
                .Returns(IslandOptimizationStatus.EvenAll);
            yield return new TestCaseData(-1)
                .Returns(IslandOptimizationStatus.EvenAll);
        }

        [Test]
        public void Test_Left_0()
        {
            var input = new bool[]
            {
                true, true, false,
            };

            var actual = IslandValleyOptimizer.Examine(input, 2);
            Assert.That(actual.Status, Is.EqualTo(IslandOptimizationStatus.EvenAll));
            Assert.That(actual.Left, Is.EqualTo(0));
            Assert.That(actual.Right, Is.EqualTo(1));
            Assert.That(actual.Peak, Is.EqualTo(2));
        }

        [Test]
        public void Test_Left_1()
        {
            var input = new bool[]
            {
                false, true, false,
            };

            var actual = IslandValleyOptimizer.Examine(input, 0);
            Assert.That(actual.Status, Is.EqualTo(IslandOptimizationStatus.EvenAll));
            Assert.That(actual.Left, Is.EqualTo(1));
            Assert.That(actual.Right, Is.EqualTo(1));
            Assert.That(actual.Peak, Is.EqualTo(0));
        }

        [Test]
        public void Test_Left_2()
        {
            var input = new bool[]
            {
                true, false, true,
            };

            var actual = IslandValleyOptimizer.Examine(input, 1);
            Assert.That(actual.Status, Is.EqualTo(IslandOptimizationStatus.EvenAll));
            Assert.That(actual.Left, Is.EqualTo(2));
            Assert.That(actual.Right, Is.EqualTo(0));
            Assert.That(actual.Peak, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(nameof(AlwaysInvalidData))]
        public IslandOptimizationStatus Test_InvalidData_3(int state)
        {
            var input = new bool[]
            {
                true, false, true, false,
            };

            var actual = IslandValleyOptimizer.Examine(input, state);
            return actual.Status;
        }

        public static IEnumerable AlwaysInvalidData()
        {
            yield return new TestCaseData(0)
                .Returns(IslandOptimizationStatus.Invalid);
            yield return new TestCaseData(1)
                .Returns(IslandOptimizationStatus.Invalid);
            yield return new TestCaseData(2)
                .Returns(IslandOptimizationStatus.Invalid);
            yield return new TestCaseData(3)
                .Returns(IslandOptimizationStatus.Invalid);
            yield return new TestCaseData(-1)
                .Returns(IslandOptimizationStatus.Invalid);
            yield return new TestCaseData(4)
                .Returns(IslandOptimizationStatus.Invalid);
        }


        [Test]
        [TestCaseSource(nameof(PeakData))]
        public int GetNormalizedPeakIndex(int peakIndex, int length)
        {
            return IslandValleyOptimizer.GetNormalizedPeakIndex(peakIndex, length);
        }

        public static IEnumerable PeakData()
        {
            yield return new TestCaseData(0, 2)
                .Returns(0);
            yield return new TestCaseData(-1, 2)
                .Returns(1);
            yield return new TestCaseData(-2, 2)
                .Returns(0);
            yield return new TestCaseData(1, 2)
                .Returns(1);
            yield return new TestCaseData(2, 2)
                .Returns(0);
            yield return new TestCaseData(5, 6)
                .Returns(5);
            yield return new TestCaseData(-1, 6)
                .Returns(5);
            yield return new TestCaseData(-1, 4)
                .Returns(3);
            yield return new TestCaseData(-2, 4)
                .Returns(2);
            yield return new TestCaseData(-3, 4)
                .Returns(1);
            yield return new TestCaseData(-4, 4)
                .Returns(0);
            yield return new TestCaseData(-5, 4)
                .Returns(3);
        }
    }
}
