using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MarkovBytes.UnitTests
{
    public class SymmetryGridAccessUnitTests
    {
        //[Test]
        //public void Grid4x4()
        //{
        //    const uint NO_OF_STATES = 4U;
        //    var expected = new uint[4, 4]
        //    {
        //        { 0, 1, 2, 3 },
        //        { 1, 4, 5, 6 },
        //        { 2, 5, 7, 8 },
        //        { 3, 6, 8, 9 },
        //    };
            
        //    var actual = GetArrayLocation(1, 1, 4);
        //    Assert.AreEqual(4, actual);

        //    for (var i = 0U; i < 1; i += 1)
        //    {
        //        for (var j = 0U; j < NO_OF_STATES; j += 1)
        //        {
        //            Assert.AreEqual(expected[i, j], GetArrayLocation(i, j, NO_OF_STATES), i + "," + j);
        //        }
        //    }
        //}

        [Test]
        [TestCaseSource(typeof(GetArrayLocationData), "SafeValues")]
        public uint GetArrayLocationTest(uint x, uint y, uint length)
        {
            return GetArrayLocation(x, y, length);
        }

        [Test]
        [TestCaseSource(typeof(GetArrayLocationData), "SwappedValue")]
        public uint SwappedArrayLocationTest(uint x, uint y, uint length)
        {
            return GetArrayLocation(x, y, length);
        }

        public class GetArrayLocationData
        {
            public static IEnumerable SafeValues
            {
                get
                {
                    yield return new TestCaseData(0U, 0U, 4U).Returns(0U);
                    yield return new TestCaseData(0U, 1U, 4U).Returns(1U);
                    yield return new TestCaseData(0U, 2U, 4U).Returns(2U);
                    yield return new TestCaseData(0U, 3U, 4U).Returns(3U);
                    yield return new TestCaseData(1U, 1U, 4U).Returns(4U);
                    yield return new TestCaseData(1U, 2U, 4U).Returns(5U);
                    yield return new TestCaseData(1U, 3U, 4U).Returns(6U);
                    yield return new TestCaseData(2U, 2U, 4U).Returns(7U);
                    yield return new TestCaseData(2U, 3U, 4U).Returns(8U);
                    yield return new TestCaseData(3U, 3U, 4U).Returns(9U);
                }
            }

            public static IEnumerable SwappedValue
            {
                get
                {
                    yield return new TestCaseData(0U, 0U, 4U).Returns(0U);
                    yield return new TestCaseData(1U, 0U, 4U).Returns(1U);
                    yield return new TestCaseData(2U, 0U, 4U).Returns(2U);
                    yield return new TestCaseData(3U, 0U, 4U).Returns(3U);
                    yield return new TestCaseData(1U, 1U, 4U).Returns(4U);
                    yield return new TestCaseData(2U, 1U, 4U).Returns(5U);
                    yield return new TestCaseData(3U, 1U, 4U).Returns(6U);
                    yield return new TestCaseData(2U, 2U, 4U).Returns(7U);
                    yield return new TestCaseData(3U, 2U, 4U).Returns(8U);
                    yield return new TestCaseData(3U, 3U, 4U).Returns(9U);
                }
            }
        }

        private uint GetArrayLocation(uint x, uint y, uint length)
        {
            // SWAP
            if (y < x)
            {
                x ^= y;
                y ^= x;
                x ^= y;
            }

            return y + (length * x) - ((x * (x + 1U)) >> 1);
        }
    }
}
