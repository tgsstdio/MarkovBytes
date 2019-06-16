using Markov;
using NUnit.Framework;
using System.Numerics;

namespace MarkovBytes.UnitTests
{
    public class SlicersUnitTests
    {
        [Test]
        public void EmptyTest()
        {
            var rowData = new ushort[] { };
            var actual = Slicer.ArrangeSlices(rowData);
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.TotalStates);
            Assert.AreEqual(0, actual.TotalNonZeros);
            Assert.IsNotNull(actual.Leaves);
            Assert.AreEqual(0, actual.Leaves.Length);
            Assert.IsNotNull(actual.Branches);
            Assert.AreEqual(0, actual.Branches.Length);
        }

        public void BuildTree()
        {
            Assert.AreEqual(32, Vector<byte>.Count);
            Assert.AreEqual(16, Vector<ushort>.Count);


        }

        [Test]
        public void GetBranch_00()
        {
            var slicer = new Slicer(new SIMDRuler());
            var actual = slicer.CreateRowBranch(0);
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Keys);
            Assert.AreEqual(0, actual.Keys.Length);
            Assert.AreEqual(0, actual.NoOfBuckets);
        }

        [Test]
        public void GetBranch_01()
        {
            var slicer = new Slicer(new SIMDRuler());
            var actual = slicer.CreateRowBranch(1);
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Keys);
            Assert.AreEqual(16, actual.Keys.Length);
            Assert.AreEqual(1, actual.NoOfBuckets);
        }

        [Test]
        public void CopyBranchNodes_01()
        {
            const int MAX_VALUES = 1;
            var BRANCH_VALUES = new ushort[]
            {
                1,
            };

            var row = new RowBranch
            {
                Keys = new ushort[MAX_VALUES],
            };

            Slicer.CopyBranchNodes(BRANCH_VALUES, 0, MAX_VALUES, MAX_VALUES, row);

            for (var i = 0; i < MAX_VALUES; i += 1)
            {
                Assert.AreEqual(BRANCH_VALUES[i], row.Keys[i]);
            }
        }

        [Test]
        public void GetBranch_05()
        {
            var BRANCH_VALUES = new ushort[]
            {
                11, 12, 13, 14, 15
            };

            const int MAX_VALUES = 5;
            var slicer = new Slicer(new SIMDRuler());
            var actual = slicer.CreateRowBranch(MAX_VALUES);
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Keys);
            Assert.AreEqual(16, actual.Keys.Length);
            Assert.AreEqual(1, actual.NoOfBuckets);
        }

        [Test]
        public void CopyBranchNodes_05()
        {
            const int BUCKET_SIZE = 16;
            var BRANCH_VALUES = new ushort[]
            {
                11, 12, 13, 14, 15
            };

            const int MAX_VALUES = 5;

            var row = new RowBranch
            {
                Keys = new ushort[BUCKET_SIZE],
            };

            Slicer.CopyBranchNodes(BRANCH_VALUES, 0, MAX_VALUES, BUCKET_SIZE, row);

            for (var i = 0; i < MAX_VALUES; i += 1)
            {
                Assert.AreEqual(BRANCH_VALUES[i], row.Keys[i]);
            }

            for (var i = MAX_VALUES; i < BUCKET_SIZE; i += 1)
            {
                Assert.AreEqual(ushort.MaxValue, row.Keys[i]);
            }
        }

        [Test]
        public void GetBranch_16()
        {
            const int MAX_VALUES = 16;
            var BRANCH_VALUES = new ushort[]
            {
                201, 202, 203, 204, 205, 206, 207, 208, 209, 210,
                211, 212, 213, 214, 215, 216,
            };
            var slicer = new Slicer(new SIMDRuler());
            var actual = slicer.CreateRowBranch(MAX_VALUES);
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Keys);
            Assert.AreEqual(16, actual.Keys.Length);
            Assert.AreEqual(1, actual.NoOfBuckets);
        }

        public void CopyBranchNodes_16()
        {
            const int MAX_VALUES = 16;
            var BRANCH_VALUES = new ushort[]
            {
                201, 202, 203, 204, 205, 206, 207, 208, 209, 210,
                211, 212, 213, 214, 215, 216,
            };

            var row = new RowBranch
            {
                Keys = new ushort[MAX_VALUES],
            };

            Slicer.CopyBranchNodes(BRANCH_VALUES, 0, MAX_VALUES, MAX_VALUES, row);

            for (var i = 0; i < MAX_VALUES; i += 1)
            {
                Assert.AreEqual(BRANCH_VALUES[i], row.Keys[i]);
            }
        }

        [Test]
        public void GetBranch_17()
        {
            const int TWO_BUCKETS_LONG = 32;
            const int MAX_VALUES = 17;
            ushort[] BRANCH_VALUES = new ushort[]
            {
                 1,  2,  3,  4,  5,  6, 7, 8, 9, 10,
                11, 12, 13, 14, 15, 16, 17,
            };
            var slicer = new Slicer(new SIMDRuler());
            var actual = slicer.CreateRowBranch(MAX_VALUES);
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Keys);

            Assert.AreEqual(TWO_BUCKETS_LONG, actual.Keys.Length);
            Assert.AreEqual(2, actual.NoOfBuckets);
        }

        [Test]
        public void CopyBranchNodes_17()
        {
            const int TWO_BUCKETS_LONG = 32;
            const int MAX_VALUES = 17;
            ushort[] BRANCH_VALUES = new ushort[]
            {
                 1,  2,  3,  4,  5,  6, 7, 8, 9, 10,
                11, 12, 13, 14, 15, 16, 17,
            };

            var row = new RowBranch
            {
                Keys = new ushort[TWO_BUCKETS_LONG],
            };

            Slicer.CopyBranchNodes(BRANCH_VALUES, 0, MAX_VALUES, TWO_BUCKETS_LONG, row);

            for (var i = 0; i < MAX_VALUES; i += 1)
            {
                Assert.AreEqual(BRANCH_VALUES[i], row.Keys[i]);
            }

            for (var i = MAX_VALUES; i < TWO_BUCKETS_LONG; i += 1)
            {
                Assert.AreEqual(ushort.MaxValue, row.Keys[i]);
            }
        }

        [Test]
        public void BuildTree_0()
        {
            var ruler = new SIMDRuler();
            var actual = ruler.GetBitMasks(0);
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Length);
        }

        [Test]
        public void BuildTree_1()
        {
            var ruler = new SIMDRuler();
            var actual = ruler.GetBitMasks(1);
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Length);

            Assert.AreEqual(0x1, actual[0]);

        }

        [Test]
        public void BuildTree_15()
        {
            var ruler = new SIMDRuler();
            var actual = ruler.GetBitMasks(15);
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);

            Assert.AreEqual(0x1, actual[0]);
            Assert.AreEqual(0xfffe, actual[1]);

        }

        [Test]
        public void BuildTree_16()
        {
            var ruler = new SIMDRuler();
            var actual = ruler.GetBitMasks(16);
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);

            Assert.AreEqual(0x1, actual[0]);
            Assert.AreEqual(0xfffe, actual[1]);
            Assert.AreEqual(0x1, actual[2]);
        }

        [Test]
        public void BuildTree_17()
        {
            var ruler = new SIMDRuler();
            var actual = ruler.GetBitMasks(17);
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);

            Assert.AreEqual(0x1, actual[0]);
            Assert.AreEqual(0xfffe, actual[1]);
            Assert.AreEqual(0x3, actual[2]);
        }

        [Test]
        public void BuildTree_31()
        {
            var ruler = new SIMDRuler();
            var actual = ruler.GetBitMasks(31);
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);

            Assert.AreEqual(0x1, actual[0]);
            Assert.AreEqual(0xfffe, actual[1]);
            Assert.AreEqual(0xffff, actual[2]);
        }

        [Test]
        public void BuildTree_47()
        {
            var ruler = new SIMDRuler();
            var actual = ruler.GetBitMasks(47);
            Assert.IsNotNull(actual);
            Assert.AreEqual(4, actual.Length);

            Assert.AreEqual(0x1, actual[0]);
            Assert.AreEqual(0xfffe, actual[1]);
            Assert.AreEqual(0xffff, actual[2]);
            Assert.AreEqual(0xffff, actual[3]);
        }


        [Test]
        public void BuildTree_48()
        {
            var ruler = new SIMDRuler();
            var actual = ruler.GetBitMasks(48);
            Assert.IsNotNull(actual);
            Assert.AreEqual(5, actual.Length);

            Assert.AreEqual(0x1, actual[0]);
            Assert.AreEqual(0xfffe, actual[1]);
            Assert.AreEqual(0xffff, actual[2]);
            Assert.AreEqual(0xffff, actual[3]);
            Assert.AreEqual(0x1, actual[4]);            
        }

        [Test]
        public void BuildTree_32()
        {
            var ruler = new SIMDRuler();
            var actual = ruler.GetBitMasks(32);
            Assert.IsNotNull(actual);
            Assert.AreEqual(4, actual.Length);
            Assert.AreEqual(0x1, actual[0]);
            Assert.AreEqual(0xfffe, actual[1]);
            Assert.AreEqual(0xffff, actual[2]);
            Assert.AreEqual(0x1, actual[3]);            
        }



    }
}
