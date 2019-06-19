using System;
using System.Collections.Generic;

namespace Markov
{
    public class SIMDRuler : ISliceRuler
    {
        public int BucketSize => 16;

        public ushort[] GetBitMasks(int length)
        {
            var masks = new List<ushort>();

            if (length == 0)
            {
                return new ushort[] { };
            }

            if (length >= 1)
            {
                // create first node is cutoff
                masks.Add(0x1); // SAME FOR 32 key bucket
            }

            if (length >= 2)
            {
                // create second node is 15 item check
                masks.Add(0xfffe); // 0xffff_fffe for 32 key bucket
            }

            if (length > 15)
            {
                // keep create nodes until empty

                var remaining = length - 15;

                var noOfNodes = Math.DivRem(remaining, 16, out int rem);

                for (var i = 0; i < noOfNodes; i += 1)
                {
                    masks.Add(0xffff); // 0xffff_ffff for 32 key bucket
                }

                if (rem > 0)
                {
                    ushort lastMask = 0;
                    for (var i = 0; i < rem; i += 1)
                    {
                        lastMask |= (ushort)(1 << i); 
                    }

                    masks.Add(lastMask); // SAME FOR 32 key bucket
                }
            }

            return masks.ToArray();
        }

        public void GetChunkInfo(int length, out int wholeBuckets, out int remainder)
        {
            wholeBuckets = Math.DivRem(length, this.BucketSize, out remainder);
        }

        public int GetNoOfBuckets(int length)
        { 
            var wholeBuckets = Math.DivRem(length, this.BucketSize, out int remainder);
            return wholeBuckets + ((remainder > 0) ? 1 : 0);
        }
    }
}