using Markov;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace MarkovBytes.UnitTests
{
    public class SlicersUnitTests
    {
        [Test]
        public void EmptyTest()
        {
            var rowData = new ushort[] { };
            var actual = ArrangeSlices(rowData);
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

            ProcessRow();

        }

        private RowTree ProcessRow()
        {
            var data = new SliceInfo
            {
                TotalNonZeros = 4,
                Branches = new ushort[] { 10, 12, 60, 80 },
                Leaves = new int[] { 0, 3, 4, 5 },
            };
            var masks = Get16BitMasks(data);
            var branch = CreateRowBranch(data.TotalNonZeros);
            CopyBranchNodes(data.Branches, 0, data.TotalNonZeros, branch.Keys.Length, branch);

            // Build test chunk
            // 1. branch vector 
            // 2. leaf offset
            // 3. leaf array
            const int BUCKET_SIZE = 16;
            var wholeBuckets = Math.DivRem(data.TotalNonZeros, BUCKET_SIZE, out int remainder);

            var chunks = new TestChunk[branch.NoOfBuckets];
            var offset = 0;
            for (var i = 0; i < branch.NoOfBuckets; i += 1)
            {
                var window = new Span<ushort>(branch.Keys, offset, BUCKET_SIZE);

                chunks[i] = new TestChunk
                {
                    Keys = new Vector<ushort>(window),
                    LeafOffset = offset,
                    LeafLength = (i < wholeBuckets) ? BUCKET_SIZE : remainder,
                };

                offset += BUCKET_SIZE;
            }

            // generate instruction set
            // 1. set chunk no
            // 2. branch mask
            var instructions = new Instruction[masks.Length];

            if (masks.Length <= 1)
            {
                instructions[0] = new Instruction
                {
                    Chunk = 0,
                    Mask = masks[0],
                };
            }

            if (masks.Length <= 2)
            {
                instructions[1] = new Instruction
                {
                    Chunk = 0,
                    Mask = masks[1],
                };
            }

            for (var i = 2; i < masks.Length; i += 1)
            {
                instructions[i] = new Instruction
                {
                    Chunk = i - 1,
                    Mask = masks[i],
                };
            }

            return new RowTree
            {
                TestChunks = chunks,
                Instructions = instructions,
            };
        }

        [Test]
        public void GetBranch_00()
        {
            var actual = CreateRowBranch(0);
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Keys);
            Assert.AreEqual(0, actual.Keys.Length);
            Assert.AreEqual(0, actual.NoOfBuckets);
        }

        [Test]
        public void GetBranch_01()
        {
            var BRANCH_VALUES = new ushort[]
            {
                1,
            };

            var actual = CreateRowBranch(1);
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

            CopyBranchNodes(BRANCH_VALUES, 0, MAX_VALUES, MAX_VALUES, row);

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
            var actual = CreateRowBranch(MAX_VALUES);
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

            CopyBranchNodes(BRANCH_VALUES, 0, MAX_VALUES, BUCKET_SIZE, row);

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
            var actual = CreateRowBranch(MAX_VALUES);
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

            CopyBranchNodes(BRANCH_VALUES, 0, MAX_VALUES, MAX_VALUES, row);

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
            var actual = CreateRowBranch(MAX_VALUES);
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

            CopyBranchNodes(BRANCH_VALUES, 0, MAX_VALUES, TWO_BUCKETS_LONG, row);

            for (var i = 0; i < MAX_VALUES; i += 1)
            {
                Assert.AreEqual(BRANCH_VALUES[i], row.Keys[i]);
            }

            for (var i = MAX_VALUES; i < TWO_BUCKETS_LONG; i += 1)
            {
                Assert.AreEqual(ushort.MaxValue, row.Keys[i]);
            }
        }

        private RowBranch CreateRowBranch(int length)
        {
            const int BUCKET_SIZE = 16;
            int noOfBuckets = GetNoOfBuckets(length, BUCKET_SIZE);

            int dstArraySize = noOfBuckets * BUCKET_SIZE;
            var result = new RowBranch
            {
                NoOfBuckets = noOfBuckets,
                Keys = new ushort[dstArraySize],
            };

            // CopyBranchNodes(values, offset, length, dstArraySize, result);

            return result;
        }

        private static int GetNoOfBuckets(int length, int BUCKET_SIZE)
        {
            var wholeBuckets = Math.DivRem(length, BUCKET_SIZE, out int remainder);
            return wholeBuckets + ((remainder > 0) ? 1 : 0);
        }

        private static void CopyBranchNodes(ushort[] src, int offset, int srcLength, int dstLength, RowBranch result)
        {
            // TODO: if majority exists, insert as index 0

            var srcSpan = new Span<ushort>(src, offset, srcLength);
            var dstSpan = new Span<ushort>(result.Keys, 0, dstLength);
            srcSpan.CopyTo(dstSpan);

            for (var i = srcLength; i < dstLength; i += 1)
            {
                result.Keys[i] = ushort.MaxValue;
            }
        }

        [Test]
        public void BuildTree_0()
        {
            var data = new SliceInfo
            {
                TotalNonZeros = 0,
            };
            var actual = Get16BitMasks(data);
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Length);
        }

        [Test]
        public void BuildTree_1()
        {
            var data = new SliceInfo
            {
                TotalNonZeros = 1,
            };
            var actual = Get16BitMasks(data);
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Length);

            Assert.AreEqual(0x1, actual[0]);

        }

        [Test]
        public void BuildTree_15()
        {
            var data = new SliceInfo
            {
                TotalNonZeros = 15,
            };
            var actual = Get16BitMasks(data);
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);

            Assert.AreEqual(0x1, actual[0]);
            Assert.AreEqual(0xfffe, actual[1]);

        }

        [Test]
        public void BuildTree_16()
        {
            var data = new SliceInfo
            {
                TotalNonZeros = 16,
            };
            var actual = Get16BitMasks(data);
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);

            Assert.AreEqual(0x1, actual[0]);
            Assert.AreEqual(0xfffe, actual[1]);
            Assert.AreEqual(0x1, actual[2]);
        }

        [Test]
        public void BuildTree_17()
        {
            var data = new SliceInfo
            {
                TotalNonZeros = 17,
            };
            var actual = Get16BitMasks(data);
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);

            Assert.AreEqual(0x1, actual[0]);
            Assert.AreEqual(0xfffe, actual[1]);
            Assert.AreEqual(0x3, actual[2]);
        }

        [Test]
        public void BuildTree_31()
        {
            var data = new SliceInfo
            {
                TotalNonZeros = 31,
            };
            var actual = Get16BitMasks(data);
            Assert.IsNotNull(actual);
            Assert.AreEqual(3, actual.Length);

            Assert.AreEqual(0x1, actual[0]);
            Assert.AreEqual(0xfffe, actual[1]);
            Assert.AreEqual(0xffff, actual[2]);
        }

        [Test]
        public void BuildTree_47()
        {
            var data = new SliceInfo
            {
                TotalNonZeros = 47,
            };
            var actual = Get16BitMasks(data);
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
            var data = new SliceInfo
            {
                TotalNonZeros = 48,
            };
            var actual = Get16BitMasks(data);
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
            var data = new SliceInfo
            {
                TotalNonZeros = 32,
            };
            var actual = Get16BitMasks(data);
            Assert.IsNotNull(actual);
            Assert.AreEqual(4, actual.Length);
            Assert.AreEqual(0x1, actual[0]);
            Assert.AreEqual(0xfffe, actual[1]);
            Assert.AreEqual(0xffff, actual[2]);
            Assert.AreEqual(0x1, actual[3]);            
        }

        private ushort[] Get16BitMasks(SliceInfo data)
        {
            var masks = new List<ushort>();

            if (data.TotalNonZeros == 0)
            {
                return new ushort[] { };
            }

            if (data.TotalNonZeros >= 1)
            {
                // create first node is cutoff
                masks.Add(0x1);
            }

            if (data.TotalNonZeros >= 2)
            {
                // create second node is 15 item check
                masks.Add(0xfffe);
            }

            if (data.TotalNonZeros > 15)
            {
                // keep create nodes until empty

                var remaining = data.TotalNonZeros - 15;

                var noOfNodes = Math.DivRem(remaining, 16, out int rem);

                for (var i = 0; i < noOfNodes; i += 1)
                {
                    masks.Add(0xffff);
                }

                if (rem > 0)
                {
                    ushort lastMask = 0;
                    for (var i = 0; i < rem; i += 1)
                    {
                        lastMask |= (ushort)(1 << i);
                    }

                    masks.Add(lastMask);
                }
            }

            return masks.ToArray();
        }

        private static SliceInfo ArrangeSlices(ushort[] rowData)
        {
            ushort total = 0;
            var leaves = new List<int>();
            var branches = new List<ushort>();
            var count = rowData.Length;

            var nonZeros = 0;
            for (var i = 0; i < count; i += 1)
            {
                var percent = rowData[i];
                if (percent > 0)
                {
                    total += percent;
                    leaves.Add(i);
                    branches.Add(total);
                    nonZeros += 1;
                }
            }
            return new SliceInfo {
                TotalStates = count,
                TotalNonZeros = nonZeros,
                Leaves = leaves.ToArray(),
                Branches = branches.ToArray(),
            };
        }
    }
}
