using System;
using System.Numerics;

namespace Markov
{
    public class DefaultNodeEvaluator : INodeEvaluator
    {
        public uint Evaluate(TestChunk testChunk, ushort[] branches, ushort singleValue)
        {
            var nodeSpace = new Span<ushort>(branches, testChunk.KeyOffset, testChunk.LeafLength);
            return SetMask16_SIMD_Optimized_1(nodeSpace, singleValue);
        }

        static readonly ushort[] BITSHIFT_16 = new ushort[]
        {
            1,
            2,
            4,
            8,
            16,
            32,
            64,
            128,
            256,
            512,
            1024,
            2048,
            4096,
            8192,
            16384,
            32768,
        };

        private static uint SetMask16_SIMD_Optimized_1(Span<ushort> checks, ushort value)
        {
            var a = new Vector<ushort>(value);
            var b = new Vector<ushort>(checks);

            // INTERNAL TEST 
            var test = Vector.LessThanOrEqual(a, b);
            var right = new Vector<ushort>(BITSHIFT_16);

            var y = Vector.ConditionalSelect(test, right, Vector<ushort>.Zero);

            // Console.WriteLine(test);
            // Console.WriteLine(y);

            Vector.Widen(y, out Vector<uint> f8, out Vector<uint> b8);
            var shuffle = Vector.BitwiseOr(f8, b8);

            // CONSOLIDATE
            Vector.Widen(shuffle, out Vector<ulong> f4, out Vector<ulong> b4);
            var sum = Vector.BitwiseOr(f4, b4);
            return (uint)(sum[0] | sum[1] | sum[2] | sum[3]);
        }

    }
}
