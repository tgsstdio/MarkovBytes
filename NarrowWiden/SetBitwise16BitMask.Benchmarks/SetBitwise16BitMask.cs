using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Numerics;

namespace SetBitwise16BitMask.Benchmarks
{
    public class SetBitwise16BitMask
    {
        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(Inputs))]
        public uint ForLoop(ushort[] masks, ushort[] flags)
        {
            return PlainLoop16_1(masks, flags);
        }

        private static uint PlainLoop16_1(ushort[] mask, ushort[] flags)
        {
            uint w = 0;
            for (var j = 0; j < 16; j += 1)
            {
                // the bit mask
                if (mask[j] > flags[j])
                    w |= 1U << j;
            }
            return w;
        }

        [Benchmark]
        [ArgumentsSource(nameof(Inputs))]
        public uint SIMD(ushort[] masks, ushort[] flags)
        {
            return SetMask16_SIMD_Optimized_2(masks, flags);
        }


        public IEnumerable<object[]> Inputs()
        {
            yield return new object[] {
                new ushort[] {
                    1, 1, 1, 1, 1, 1, 1, 1,
                    1, 1, 1, 1, 1, 1, 1, 1,
                },
                new ushort[] {
                    1, 1, 1, 1, 1, 1, 1, 1,
                    1, 1, 1, 1, 1, 1, 1, 1,
                }
            };
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

        private static uint SetMask16_SIMD_Optimized_2(ushort[] checks, ushort[] flags)
        {

            // INTERNAL TEST 


            // Console.WriteLine(test);
            // Console.WriteLine(y);

            Vector.Widen(Vector.ConditionalSelect(Vector.GreaterThan(new Vector<ushort>(checks), new Vector<ushort>(flags)), new Vector<ushort>(BITSHIFT_16), Vector<ushort>.Zero), out Vector<uint> f8, out Vector<uint> b8);

            // CONSOLIDATE
            Vector.Widen(Vector.BitwiseOr(f8, b8), out Vector<ulong> f4, out Vector<ulong> b4);
            var sum = Vector.BitwiseOr(f4, b4);
            return (uint)(sum[0] | sum[1] | sum[2] | sum[3]);
        }
    }
}
