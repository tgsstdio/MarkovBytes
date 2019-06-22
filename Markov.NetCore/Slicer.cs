using System;
using System.Collections.Generic;

namespace Markov
{
    public class Slicer : ISlicer
    {
        private readonly ISliceRuler mRuler;
        public Slicer(ISliceRuler ruler)
        {
            mRuler = ruler;
        }

        public RowTree[] Slice(ushort[][] srcRows)
        {
            var noOfRows = srcRows.Length;
            var dstRows = new RowTree[noOfRows];
            for (var i = 0; i < noOfRows; i += 1)
            {
                dstRows[i] = SliceRow(srcRows[i]);
            }
            return dstRows;
        }

        public RowTree SliceRow(ushort[] srcRows)
        {
            var info = ArrangeSlices(srcRows);
            return ProcessRow(info);
        }

        public RowTree ProcessRow(SliceInfo data)
        {
            //var data = new SliceInfo
            //{
            //    TotalNonZeros = 4,
            //    Branches = new ushort[] { 10, 12, 60, 80 },
            //    Leaves = new int[] { 0, 3, 4, 5 },
            //};
            var masks = mRuler.GetBitMasks(data.TotalNonZeros);
            var branch = CreateRowBranch(data.TotalNonZeros);
            CopyBranchNodes(data.Branches, 0, data.TotalNonZeros, branch.Keys.Length, branch);

            // Build test chunk
            // 1. branch vector 
            // 2. leaf offset
            // 3. leaf array
            mRuler.GetChunkInfo(data.TotalNonZeros, out int wholeBuckets, out int remainder);

            var chunks = new TestChunk[branch.NoOfBuckets];
            var offset = 0;
            for (var i = 0; i < branch.NoOfBuckets; i += 1)
            {
                chunks[i] = new TestChunk
                {
                    KeyOffset = offset,
                    LeafOffset = offset,
                    LeafLength = (i < wholeBuckets) ? mRuler.BucketSize : remainder,
                };

                offset += mRuler.BucketSize;
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
                Branches = data.Branches,
                Leaves = data.Leaves,
                TestChunks = chunks,
                Instructions = instructions,
            };
        }

        private static void GetChunkInfo(SliceInfo data, out int BUCKET_SIZE, out int wholeBuckets, out int remainder)
        {
            BUCKET_SIZE = 16;
            wholeBuckets = Math.DivRem(data.TotalNonZeros, BUCKET_SIZE, out remainder);
        }

        public static SliceInfo ArrangeSlices(ushort[] rowData)
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
            return new SliceInfo
            {
                TotalStates = count,
                TotalNonZeros = nonZeros,
                Leaves = leaves.ToArray(),
                Branches = branches.ToArray(),
            };
        }

        public RowBranch CreateRowBranch(int length)
        {
            int noOfBuckets = mRuler.GetNoOfBuckets(length);

            int dstArraySize = noOfBuckets * mRuler.BucketSize;
            var result = new RowBranch
            {
                NoOfBuckets = noOfBuckets,
                Keys = new ushort[dstArraySize],
            };

            // CopyBranchNodes(values, offset, length, dstArraySize, result);

            return result;
        }

        private static ushort[] Get16BitMasks(SliceInfo data)
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

        public static int GetNoOfBuckets(int length, int BUCKET_SIZE)
        {
            var wholeBuckets = Math.DivRem(length, BUCKET_SIZE, out int remainder);
            return wholeBuckets + ((remainder > 0) ? 1 : 0);
        }

        public static void CopyBranchNodes(ushort[] src, int offset, int srcLength, int dstLength, RowBranch result)
        {
            // TODO: if majority exists, insert as index 0
            for (var i = offset; i < srcLength; i += 1)
            {
                result.Keys[i] = src[i];
            }

            for (var i = srcLength; i < dstLength; i += 1)
            {
                result.Keys[i] = ushort.MaxValue;
            }
        }

        public RowTree SliceMatrix(int i, ushort[,] matrix)
        {
            var info = ArrangeMatrixRow(i , matrix);
            return ProcessRow(info);
        }

        public static SliceInfo ArrangeMatrixRow(int i, ushort[,] matrix)
        {
            ushort total = 0;
            var leaves = new List<int>();
            var branches = new List<ushort>();
            var count = matrix.GetLength(1);

            var nonZeros = 0;
            for (var j = 0; j < count; j += 1)
            {
                var percent = matrix[i, j];
                if (percent > 0)
                {
                    total += percent;
                    leaves.Add(j);
                    branches.Add(total);
                    nonZeros += 1;
                }
            }
            return new SliceInfo
            {
                TotalStates = count,
                TotalNonZeros = nonZeros,
                Leaves = leaves.ToArray(),
                Branches = branches.ToArray(),
            };
        }
    }
}
