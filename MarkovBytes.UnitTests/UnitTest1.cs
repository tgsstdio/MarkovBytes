using System;
using System.Collections;
using Markov;
using NUnit.Framework;

namespace MarkovBytes.UnitTests
{
    public partial class Tests
    {



        public void Setup()
        {
            // [0, 100] inclusive

            var rand = new Random();
            var next = rand.Next(ushort.MinValue, ushort.MaxValue);
        }

        [Test]
        public void EvenAll()
        {
            var maxDomain = 100;

            var N = 10;

            {
                var offset = 0;

                Assert.AreEqual(0, Solver.GetEvenAllTransistion(9, maxDomain, N, offset));
                //Assert.AreEqual(1, GetEvenAllTransistion(10, minDomain, maxDomain, N, offset));
                Assert.AreEqual(1, Solver.GetEvenAllTransistion(11, maxDomain, N, offset));
                Assert.AreEqual(2, Solver.GetEvenAllTransistion(25, maxDomain, N, offset));
                Assert.AreEqual(3, Solver.GetEvenAllTransistion(35, maxDomain, N, offset));
                Assert.AreEqual(7, Solver.GetEvenAllTransistion(77, maxDomain, N, offset));
                Assert.AreEqual(9, Solver.GetEvenAllTransistion(100, maxDomain, N, offset));
            }
        }

        [Test]
        public void EvenAllOffset()
        {
            var maxDomain = 100;

            var N = 10;

            var offset = 3;

            Assert.AreEqual(3, Solver.GetEvenAllTransistion(9, maxDomain, N, offset));
            //Assert.AreEqual(1, GetEvenAllTransistion(10, minDomain, maxDomain, N, offset));
            Assert.AreEqual(4, Solver.GetEvenAllTransistion(11, maxDomain, N, offset));
            Assert.AreEqual(5, Solver.GetEvenAllTransistion(25, maxDomain, N, offset));
            Assert.AreEqual(6, Solver.GetEvenAllTransistion(35, maxDomain, N, offset));
            Assert.AreEqual(0, Solver.GetEvenAllTransistion(77, maxDomain, N, offset));
            Assert.AreEqual(2, Solver.GetEvenAllTransistion(100, maxDomain, N, offset));
        }

        [Test]
        public void EvenAllOffset_2()
        {
            var maxDomain = 100;

            var N = 10;

            var offset = 3;

            Assert.AreEqual(2, Solver.GetEvenAllTransistion(100, maxDomain, N, offset));
        }

        [Test]
        public void EvenAllOffset_3()
        {
            var maxDomain = 100;

            var N = 10;

            var offset = 3;

            Assert.AreEqual(2, Solver.GetEvenAllTransistion(99, maxDomain, N, offset));
        }

        [Test]
        public void EvenAllOffset_4()
        {
            var maxDomain = 100;

            var N = 10;

            var offset = 3;

            Assert.AreEqual(2, Solver.GetEvenAllTransistion(90, maxDomain, N, offset));
        }

        [Test]
        public void EvenAllOffset_5()
        {
            var minDomain = 0;
            var maxDomain = 100;

            var N = 10;

            var offset = 3;

            Assert.AreEqual(1, Solver.GetEvenAllTransistion(89, maxDomain, N, offset));
        }

        [Test]
        public void EvenAllOffset_6()
        {
            var maxDomain = 100;

            var N = 10;

            var offset = 3;

            Assert.AreEqual(0, Solver.GetEvenAllTransistion(79, maxDomain, N, offset));
        }

        [Test]
        public void EvenAlNormal()
        {



            const int Next = 11;
            const int MinDomain = 0;
            const int MaxDomain = 100;
            const int N = 10;

            Assert.AreEqual(1, GetNormalized(Next, MinDomain, MaxDomain, N), "GetNormalized");
        }

        [Test]
        public void EvenAllIndex()
        {
            Assert.AreEqual(5, GetIndex(5, 10, 0, 10));
            Assert.AreEqual(17, GetIndex(7, 20, 10, 20));
            Assert.AreEqual(66, GetIndex(66, 100, 0, 100));
            Assert.AreEqual(99, GetIndex(100, 100, 0, 100));

            Assert.AreEqual(4, GetIndex(5, 5, 0, 5));
            Assert.AreEqual(0, GetIndex(10, 20, 10, 20));
            Assert.AreEqual(9, GetIndex(20, 20, 10, 20));
            Assert.AreEqual(0, GetIndex(1, 3, 2, 3));
            Assert.AreEqual(1, GetIndex(2, 3, 2, 3));
            Assert.AreEqual(1, GetIndex(3, 3, 2, 3));
        }

        [Test]
        public void EvenAllIndexWithOffset()
        {
            Assert.AreEqual(2, GetIndex(1, 5, 1, 5));
            Assert.AreEqual(3, GetIndex(1, 5, 2, 5));
            Assert.AreEqual(4, GetIndex(1, 5, 3, 5));
            Assert.AreEqual(4, GetIndex(5, 5, 0, 5));
            Assert.AreEqual(0, GetIndex(1, 5, 4, 5));
        }

        [Test]
        [TestCaseSource(nameof(EvenAllValues))]
        public int SlimRecheck_EvenAll(int next, int arrayLength, int left, int count)
        {
            int right = left + count - 1;
            return Solver.GetEvenInRange(next, count, left, Solver.WrapRange(left, right, arrayLength), arrayLength);
        }

        public static IEnumerable EvenAllValues
        {
            get
            {
                // SlimWindow_0
                yield return new TestCaseData(0, 10, 0, 4).Returns(0);

                // SlimWindow_1
                yield return new TestCaseData(0, 10, 1, 4).Returns(1);

                // SlimWindow_2
                yield return new TestCaseData(0, 10, 2, 4).Returns(2);

                // SlimWindow_3
                yield return new TestCaseData(0, 10, 3, 4).Returns(3);

                // SlimWindow_4
                yield return new TestCaseData(0, 10, 6, 4).Returns(6);

                // WindowOF4_1
                yield return new TestCaseData(1, 10, 6, 4).Returns(7);

                // WindowOf4_0
                yield return new TestCaseData(0, 10, 6, 4).Returns(6);

                // WindowOf4_2
                yield return new TestCaseData(2, 10, 6, 4).Returns(8);

                // WindowOf4_3
                yield return new TestCaseData(3, 10, 6, 4).Returns(9);

                // WindowOf4_4
                yield return new TestCaseData(4, 10, 6, 4).Returns(9);
            }
        }

        [Test]
        public void SlimWindow_0()
        {
            Assert.AreEqual(0, Solver.GenerateEvenAll(0, 0, 4, 10));
        }

        [Test]
        public void SlimWindow_1()
        {
            Assert.AreEqual(1, Solver.GenerateEvenAll(0, 1, 4, 10));
        }

        [Test]
        public void SlimWindow_2()
        {
            Assert.AreEqual(2, Solver.GenerateEvenAll(0, 2, 4, 10));
        }

        [Test]
        public void SlimWindow_3()
        {
            Assert.AreEqual(3, Solver.GenerateEvenAll(0, 3, 4, 10));
        }


        [Test]
        public void SlimWindow_4()
        {
            Assert.AreEqual(6, Solver.GenerateEvenAll(0, 6, 4, 10));
        }

        [Test]
        public void WindowOF4_1()
        {
            Assert.AreEqual(7, GetIndex(1, 10, 6, 4));

            Assert.AreEqual(7, Solver.GenerateEvenAll(1, 6, 4, 10));
        }

        [Test]
        public void WindowOf4_0()
        {
            Assert.AreEqual(6, Solver.GenerateEvenAll(0, 6, 4, 10));
        }

        [Test]
        public void WindowOf4_2()
        { 
            Assert.AreEqual(8, Solver.GenerateEvenAll(2, 6, 4, 10));
        }

        [Test]
        public void WindowOf4_3()
        {
            Assert.AreEqual(9, Solver.GenerateEvenAll(3, 6, 4, 10));
        }

        [Test]
        public void WindowOf4_4()
        {
            Assert.AreEqual(9, Solver.GenerateEvenAll(4, 6, 4, 10));
        }

        [Test]
        public void EvenAllIndex_OVERUpper_0()
        {
            Assert.AreEqual(4, GetIndex(6, 5, 0, 5));
        }

        [Test]
        public void EvenAllIndex_OVERUpper_1()
        {
            Assert.AreEqual(0, GetIndex(6, 5, 1, 5));
        }

        [Test]
        public void EvenOut_0_Rand_0()
        {
            // avoid 0
            int self = 0;
            int N = 10;
            int next = 0;
            int expected = 1;

            Assert.AreEqual(expected, Solver.GenerateEvenOut(next ,self, N));
        }

        [Test]
        public void EvenOut_1_Rand_0()
        {
            int self = 1;
            int N = 10;
            int next = 0;
            int expected = 2;

            Assert.AreEqual(expected, Solver.GenerateEvenOut(next, self, N));
        }

        [Test]
        public void EvenOut_1_Rand_8()
        {
            int self = 1;
            int N = 5;
            int next = 1;
            int EXPECTED = 3;

            Assert.AreEqual(EXPECTED, Solver.GenerateEvenOut(next, self, N));
        }

        [Test]
        public void EvenOut_1_Rand_5()
        {
            int self = 1;
            int N = 10;
            int next = 5;
            int expected = 7;

            Assert.AreEqual(expected, Solver.GenerateEvenOut(next, self, N));
        }

        [Test]
        public void EvenOut_1_Rand_9()
        {
            int self = 1;
            int N = 10;
            int next = 9;
            int expected = 0;

            Assert.AreEqual(expected, Solver.GenerateEvenOut(next, self, N));
        }

        private static int GetIndex(int normal, int N, int offset, int count)
        {
            if (normal >= N)
            {
                // ALWAYS 100% should be last slot, only works for N less than 
                // data type max
                return (N - 1 + offset) % N;
            }
            else if ((normal >= count) && (normal + offset) == N)
            {
                // ALWAYS 100% should be last slot, only works for N less than 
                // data type max
                return (N - 1);
            }

            return (normal + offset) % N;
        }

        private static int GetNormalized(int next, int minDomain, int maxDomain, int N)
        {
            return Math.DivRem(
                N * (next - minDomain),
                 (maxDomain - minDomain),
                 out int result);
        }


    }
}