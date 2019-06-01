using System;
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
            var minDomain = 0;
            var maxDomain = 100;

            var N = 10;

            {
                var offset = 0;

                Assert.AreEqual(0, GetEvenAllTransistion(9, minDomain, maxDomain, N, offset));
                //Assert.AreEqual(1, GetEvenAllTransistion(10, minDomain, maxDomain, N, offset));
                Assert.AreEqual(1, GetEvenAllTransistion(11, minDomain, maxDomain, N, offset));
                Assert.AreEqual(2, GetEvenAllTransistion(25, minDomain, maxDomain, N, offset));
                Assert.AreEqual(3, GetEvenAllTransistion(35, minDomain, maxDomain, N, offset));
                Assert.AreEqual(7, GetEvenAllTransistion(77, minDomain, maxDomain, N, offset));
                Assert.AreEqual(9, GetEvenAllTransistion(100, minDomain, maxDomain, N, offset));
            }
        }

        [Test]
        public void EvenAllOffset()
        {
            var minDomain = 0;
            var maxDomain = 100;

            var N = 10;

            var offset = 3;

            Assert.AreEqual(3, GetEvenAllTransistion(9, minDomain, maxDomain, N, offset));
            //Assert.AreEqual(1, GetEvenAllTransistion(10, minDomain, maxDomain, N, offset));
            Assert.AreEqual(4, GetEvenAllTransistion(11, minDomain, maxDomain, N, offset));
            Assert.AreEqual(5, GetEvenAllTransistion(25, minDomain, maxDomain, N, offset));
            Assert.AreEqual(6, GetEvenAllTransistion(35, minDomain, maxDomain, N, offset));
            Assert.AreEqual(0, GetEvenAllTransistion(77, minDomain, maxDomain, N, offset));
            Assert.AreEqual(2, GetEvenAllTransistion(100, minDomain, maxDomain, N, offset));
        }

        [Test]
        public void EvenAllOffset_2()
        {
            var minDomain = 0;
            var maxDomain = 100;

            var N = 10;

            var offset = 3;

            Assert.AreEqual(2, GetEvenAllTransistion(100, minDomain, maxDomain, N, offset));
        }

        [Test]
        public void EvenAllOffset_3()
        {
            var minDomain = 0;
            var maxDomain = 100;

            var N = 10;

            var offset = 3;

            Assert.AreEqual(2, GetEvenAllTransistion(99, minDomain, maxDomain, N, offset));
        }

        [Test]
        public void EvenAllOffset_4()
        {
            var minDomain = 0;
            var maxDomain = 100;

            var N = 10;

            var offset = 3;

            Assert.AreEqual(2, GetEvenAllTransistion(90, minDomain, maxDomain, N, offset));
        }

        [Test]
        public void EvenAllOffset_5()
        {
            var minDomain = 0;
            var maxDomain = 100;

            var N = 10;

            var offset = 3;

            Assert.AreEqual(1, GetEvenAllTransistion(89, minDomain, maxDomain, N, offset));
        }

        [Test]
        public void EvenAllOffset_6()
        {
            var minDomain = 0;
            var maxDomain = 100;

            var N = 10;

            var offset = 3;

            Assert.AreEqual(0, GetEvenAllTransistion(79, minDomain, maxDomain, N, offset));
        }

        [Test]
        public void EvenAllDemo()
        {
            Assert.AreEqual(100, GetDenomintor(0, 100));
            Assert.AreEqual(13, GetDenomintor(13, 26));
            Assert.AreEqual(10, GetDenomintor(10, 20));
        }

        [Test]
        public void EvenAllNum()
        {
            Assert.AreEqual(15, GetNumerator(0, 15));
            Assert.AreEqual(2, GetNumerator(3, 5));
            Assert.AreEqual(10, GetNumerator(4, 14));
            Assert.AreEqual(11, GetNumerator(0, 11));
        }

        [Test]
        public void EvenAlNormal()
        {



            const int Next = 11;
            const int MinDomain = 0;
            const int MaxDomain = 100;
            const int N = 10;

            Assert.AreEqual(11, GetNumerator(0, 11), "GetNumerator");
            Assert.AreEqual(100, GetDenomintor(0, 100), "GetDenomintor");
            Assert.AreEqual(1, GetNormalized(Next, MinDomain, MaxDomain, N), "GetNormalized");
        }

        private static int GetEvenAllTransistion(
            int next, int minDomain, int maxDomain, int N, int offset)
        {
            var normal = GetNormalized(next, minDomain, maxDomain, N);
            var index = GetIndex(normal, N, offset, maxDomain - minDomain);
            return index;
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
        public void SlimWindow_0()
        {
            Assert.AreEqual(0, GenerateEvenAll(0, 0, 4, 10));
        }

        [Test]
        public void SlimWindow_1()
        {
            Assert.AreEqual(1, GenerateEvenAll(0, 1, 4, 10));
        }

        [Test]
        public void SlimWindow_2()
        {
            Assert.AreEqual(2, GenerateEvenAll(0, 2, 4, 10));
        }

        [Test]
        public void SlimWindow_3()
        {
            Assert.AreEqual(3, GenerateEvenAll(0, 3, 4, 10));
        }


        [Test]
        public void SlimWindow_4()
        {
            Assert.AreEqual(6, GenerateEvenAll(0, 6, 4, 10));
        }

        [Test]
        public void WindowOF4_1()
        {
            Assert.AreEqual(4, GetDenomintor(6, 10));
            Assert.AreEqual(1, GetNumerator(6, 7));
            Assert.AreEqual(7, GetIndex(1, 10, 6, 4));

            Assert.AreEqual(7, GenerateEvenAll(1, 6, 4, 10));
        }

        [Test]
        public void WindowOf4_0()
        {
            Assert.AreEqual(6, GenerateEvenAll(0, 6, 4, 10));
        }

        [Test]
        public void WindowOf4_2()
        { 
            Assert.AreEqual(8, GenerateEvenAll(2, 6, 4, 10));
        }

        [Test]
        public void WindowOf4_3()
        {
            Assert.AreEqual(9, GenerateEvenAll(3, 6, 4, 10));
        }

        [Test]
        public void WindowOf4_4()
        {
            Assert.AreEqual(9, GenerateEvenAll(4, 6, 4, 10));
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

        public int GenerateEvenAll(int next, int offset, int count, int N)
        {
            var normal = GetNormalized(next + offset, offset, offset + count, count);

            return GetIndex(normal, N, offset, count);
        }

        [Test]
        public void EvenOut_0_Rand_0()
        {
            // avoid 0
            int self = 0;
            int N = 10;
            int next = 0;
            int expected = 1;

            Assert.AreEqual(expected, GenerateEvenOut(next ,self, N));
        }

        [Test]
        public void EvenOut_1_Rand_0()
        {
            int self = 1;
            int N = 10;
            int next = 0;
            int expected = 2;

            Assert.AreEqual(expected, GenerateEvenOut(next, self, N));
        }

        [Test]
        public void EvenOut_1_Rand_5()
        {
            int self = 1;
            int N = 10;
            int next = 5;
            int expected = 6;

            Assert.AreEqual(expected, GenerateEvenOut(next, self, N));
        }

        [Test]
        public void EvenOut_1_Rand_9()
        {
            int self = 1;
            int N = 10;
            int next = 9;
            int expected = 0;

            Assert.AreEqual(expected, GenerateEvenOut(next, self, N));
        }

        private int GenerateEvenOut(int next, int self, int n)
        {
            return GenerateEvenAll(next, self + 1, n - 1, n);
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
                N * GetNumerator(minDomain, next),
                 GetDenomintor(minDomain, maxDomain),
                 out int result);
        }

        private static int GetNumerator(int minDomain, int next)
        {
            return (next - minDomain);
        }

        private static int GetDenomintor(int minDomain, int maxDomain)
        {
            return (maxDomain - minDomain);
        }
    }
}