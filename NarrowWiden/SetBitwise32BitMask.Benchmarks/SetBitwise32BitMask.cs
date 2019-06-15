using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Numerics;

namespace NarrowWiden.Benchmarks
{

    public class SetBitwise32BitMask
    {
        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(Inputs))]
        public uint ForLoopWithIf(byte[] mask)
        {
            return PlainLoop_1(mask);
        }


        [Benchmark]
        [ArgumentsSource(nameof(Inputs))]
        public uint ForLoopWithToggle(byte[] mask)
        {
            return PlainLoop_2(mask);
        }

        private static uint PlainLoop_2(byte[] mask)
        {
            uint w = 0;
            for (var j = 0; j < 32; j += 1)
            {
                // the bit mask

                w ^= (uint)(-((mask[j] > 0) ? 1U : 0U) ^ w) & 1U << j;
            }
            return w;
        }

        public static uint PlainLoop_1(byte[] mask)
        {
            uint w = 0;
            for (var j = 0; j < 32; j += 1)
            {
                // the bit mask
                if (mask[j] > 0)
                    w |= 1U << j;
            }
            return w;
        }

        static readonly byte[] BIT_FIELDS = new byte[] {
                1, 2, 4, 8, 16, 32, 64, 128,
                1, 2, 4, 8, 16, 32, 64, 128,

                1, 2, 4, 8, 16, 32, 64, 128,
                1, 2, 4, 8, 16, 32, 64, 128,
            };

        static readonly ushort[] F_FACTOR16_SRC = new ushort[]
        {
                1, 1, 1, 1, 1, 1, 1, 1,
                256, 256, 256, 256, 256, 256, 256, 256,
        };

        static readonly Vector<ushort> RIGHT = new Vector<ushort>(F_FACTOR16_SRC);
        static readonly Vector<byte> LEFT = new Vector<byte>(BIT_FIELDS);

        public IEnumerable<byte[]> Inputs()
        {
            var mask = new byte[] {
                1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1,

                1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1,
            };

            yield return mask;
        }

        [Benchmark]
        [ArgumentsSource(nameof(Inputs))]
        public uint SetBySIMD(byte[] mask)
        {
            return SetMask_SIMD_Optimized_3(mask);
        }

        [Benchmark]
        [ArgumentsSource(nameof(Inputs))]
        public uint SetBySIMD_Dot(byte[] mask)
        {
            return SetMask_SIMD_Optimized_4(mask);
        }

        [Benchmark]
        [ArgumentsSource(nameof(Inputs))]
        public uint SetBySIMD_Shuffle(byte[] mask)
        {
            return SetMask_SIMD_Optimized_6(mask);
        }

        [Benchmark]
        [ArgumentsSource(nameof(Inputs))]
        public uint SetBySIMD_Shuffle2(byte[] mask)
        {
            return SetMask_SIMD_Optimized_7(mask);
        }

        static readonly Vector<uint> CONSTANT_0 = new Vector<uint>(65536);
        static readonly Vector<uint> CONSTANT_1 = new Vector<uint>(16777216);

        private static uint SetMask_SIMD_Optimized_4(byte[] checks)
        {
            //Console.WriteLine(full32);
            Vector.Widen(Vector.ConditionalSelect(Vector.GreaterThan<byte>(new Vector<byte>(checks), Vector<byte>.Zero), LEFT, Vector<byte>.Zero), out Vector<ushort> f16, out Vector<ushort> b16);
            Vector.Widen(b16, out Vector<uint> bf8, out Vector<uint> bb8);
            Vector.Widen(Vector.Multiply(f16, RIGHT), out Vector<uint> ff8, out Vector<uint> fb8);
            Vector.Widen(Vector.BitwiseOr(ff8, fb8), out Vector<ulong> fff4, out Vector<ulong> ffb4);
            Vector.Widen(Vector.BitwiseOr(Vector.Multiply(bf8, CONSTANT_0), Vector.Multiply(bb8, CONSTANT_1)), out Vector<ulong> bf4, out Vector<ulong> bb4);

            return (uint)Vector.Dot(Vector.BitwiseOr(Vector.BitwiseOr(fff4, ffb4), Vector.BitwiseOr(bf4, bb4)), Vector<ulong>.One);
        }

        public static uint SetMask_SIMD_Optimized_3(byte[] checks)
        {
            //Console.WriteLine(full32);
            Vector.Widen(Vector.ConditionalSelect(Vector.GreaterThan<byte>(new Vector<byte>(checks), Vector<byte>.Zero), LEFT, Vector<byte>.Zero), out Vector<ushort> f16, out Vector<ushort> b16);
            Vector.Widen(b16, out Vector<uint> bf8, out Vector<uint> bb8);
            Vector.Widen(Vector.Multiply(f16, RIGHT), out Vector<uint> ff8, out Vector<uint> fb8);
            Vector.Widen(Vector.BitwiseOr(ff8, fb8), out Vector<ulong> fff4, out Vector<ulong> ffb4);
            Vector.Widen(Vector.BitwiseOr(Vector.Multiply(bf8, new Vector<uint>(65536)), Vector.Multiply(bb8, new Vector<uint>(16777216))), out Vector<ulong> bf4, out Vector<ulong> bb4);

            var sum = Vector.BitwiseOr(Vector.BitwiseOr(fff4, ffb4), Vector.BitwiseOr(bf4, bb4));
            return (uint)(sum[0] | sum[1] | sum[2] | sum[3]);
        }

        static readonly uint[] BITSHIFT_8 = new uint[]
        {
            1, 2, 4, 8, 16, 32, 64, 128,
        };

        private static uint SetMask_SIMD_Optimized_6(byte[] checks)
        {
            // INTERNAL TEST 
            // CONSOLIDATE
            Vector<uint> BIT_SHIFT_8 = new Vector<uint>(BITSHIFT_8);
            Vector.Widen(Vector.Multiply(Vector.AsVectorUInt32(Vector.ConditionalSelect(Vector.GreaterThan<byte>(new Vector<byte>(checks), Vector<byte>.Zero), Vector<byte>.One, Vector<byte>.Zero)), BIT_SHIFT_8), out Vector<ulong> f4, out Vector<ulong> b4);
            return (uint)Vector.Dot(Vector.BitwiseOr(f4, b4), Vector<ulong>.One);
        }

        static readonly Vector<uint> BIT_SHIFT_8 = new Vector<uint>(BITSHIFT_8);
        private static uint SetMask_SIMD_Optimized_7(byte[] checks)
        {
            // INTERNAL TEST 
            // CONSOLIDATE
            Vector.Widen(Vector.Multiply(Vector.AsVectorUInt32(Vector.ConditionalSelect(Vector.GreaterThan<byte>(new Vector<byte>(checks), Vector<byte>.Zero), Vector<byte>.One, Vector<byte>.Zero)), BIT_SHIFT_8), out Vector<ulong> f4, out Vector<ulong> b4);
            var sum = Vector.BitwiseOr(f4, b4);
            return (uint)(sum[0] | sum[1] | sum[2] | sum[3]);
        }

    }
}
