using System;
using System.Diagnostics;
using System.Numerics;

namespace NarrowWiden
{
    class Program
    {
        private const int MAX_TIMES = 100000;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var mask = new byte[] {
                1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1,

                1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1,
            };

            var mask16 = new ushort[] {
                1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1,
            };

            var flag16 = new ushort[] {
                1, 1, 1, 1, 1, 1, 1, 1,
                1, 1, 1, 1, 1, 1, 1, 1,
            };

            var simd_0 = new Stopwatch();
            var simd_2 = new Stopwatch();
            var simd_3 = new Stopwatch();
            var simd_4 = new Stopwatch();
            var simd_5 = new Stopwatch();
            var baseline_1 = new Stopwatch();
            var baseline_2 = new Stopwatch();

            uint alt_Value = 0;
            uint alt2_Value = 0;
            uint alt3_Value = 0;
            uint alt4_Value = 0;
            uint alt5_Value = 0;
            uint baseline_1_Value = 0;
            uint baseline_2_Value = 0;
            for (var i = 0; i < MAX_TIMES; i += 1)
            {
                simd_0.Start();
                alt_Value = SetMask16_SIMD_Optimized_2(mask16, flag16);
                simd_0.Stop();

                //simd_2.Start();
                //alt2_Value = SetMask_SIMD_Optimized_2(mask);
                //simd_2.Stop();

                //simd_3.Start();
                //alt3_Value = SetMask_SIMD_Optimized_3(mask);
                //simd_3.Stop();

                baseline_2.Start();
                baseline_2_Value = PlainLoop16_1(mask16, flag16);
                baseline_2.Stop();

                //simd_4.Start();
                //alt4_Value = SetMask_SIMD_Optimized_4(mask);
                //simd_4.Stop();

                simd_5.Start();
                alt5_Value = SetMask_SIMD_Optimized_6(mask);
                simd_5.Stop();

                baseline_1.Start();
                baseline_1_Value = PlainLoop_1(mask);
                baseline_1.Stop();

            }

            Console.WriteLine(alt_Value);
            //Console.WriteLine(alt2_Value);
            Console.WriteLine(alt3_Value);
            Console.WriteLine(alt4_Value);
            Console.WriteLine(alt5_Value);
            Console.WriteLine(baseline_1_Value);
            Console.WriteLine(baseline_2_Value);
            Console.WriteLine("SIMD 0: " + simd_0.Elapsed.TotalMilliseconds);
            Console.WriteLine("SIMD 2 : " + simd_2.Elapsed.TotalMilliseconds);
            Console.WriteLine("SIMD 3 : " + simd_3.Elapsed.TotalMilliseconds);
            Console.WriteLine("SIMD 4 : " + simd_4.Elapsed.TotalMilliseconds);
            Console.WriteLine("SIMD 5 : " + simd_5.Elapsed.TotalMilliseconds);
            double forLoop_1 = baseline_1.Elapsed.TotalMilliseconds;
            Console.WriteLine("FOR LOOP 1: " + forLoop_1);
            Console.WriteLine(" - vs SIMD 0: " + simd_0.Elapsed.TotalMilliseconds / forLoop_1);
            Console.WriteLine(" - vs SIMD 2: " + simd_2.Elapsed.TotalMilliseconds / forLoop_1);
            Console.WriteLine(" - vs SIMD 3: " + simd_3.Elapsed.TotalMilliseconds / forLoop_1);
            Console.WriteLine(" - vs SIMD 4: " + simd_4.Elapsed.TotalMilliseconds / forLoop_1);
            Console.WriteLine(" - vs SIMD 5: " + simd_5.Elapsed.TotalMilliseconds / forLoop_1);

            double forLoop_2 = baseline_2.Elapsed.TotalMilliseconds;
            Console.WriteLine("FOR LOOP 2: " + forLoop_2);

            Console.WriteLine(" - vs SIMD 0: " + simd_0.Elapsed.TotalMilliseconds / forLoop_2);
            Console.WriteLine(" - vs SIMD 2: " + simd_2.Elapsed.TotalMilliseconds / forLoop_2);
            Console.WriteLine(" - vs SIMD 3: " + simd_3.Elapsed.TotalMilliseconds / forLoop_2);
            Console.WriteLine(" - vs SIMD 4: " + simd_4.Elapsed.TotalMilliseconds / forLoop_2);
            Console.WriteLine(" - vs SIMD 5: " + simd_5.Elapsed.TotalMilliseconds / forLoop_2);

            //Vector.Widen(fraise16, out Vector<uint> ff8, out Vector<uint> fb8);
            //var fcombined = Vector.BitwiseOr(ff8, fb8);
            //Console.WriteLine(fcombined);

            //var factor8_src = new uint[]
            //{
            //    1, 1, 1, 1,
            //    65536, 65536, 65536, 65536,
            //};

            //Vector.Widen(fcombined, out Vector<ulong> fff4, out Vector<ulong> ffb4);
            //Console.WriteLine(fff4);

            //Vector.Widen(b16, out Vector<uint> bf8, out Vector<uint> bb8);

            //var line6 = Vector.BitwiseOr(ff8, fb8);
            //var line7 = Vector.BitwiseOr(bf8, bb8);


            //vConsole.WriteLine(line6);
            //Console.WriteLine(line7);

            //Console.WriteLine(line6);
            //Vector.Widen(line6, out Vector<ulong> line7, out Vector<ulong> line8);            
            //var factor = new Vector<ulong>(2UL);
            //var upper = Vector.Multiply(line7, factor);
            //Console.WriteLine(line7);
            //Console.WriteLine(upper);
            //Console.WriteLine(line8);

            //Vector.Widen(right, out Vector<ushort> d, out Vector<ushort> e);
            //Vector.Widen(right2, out Vector<short> d2, out Vector<short> e2);
            //Vector.Widen(right3, out Vector<ulong> d3, out Vector<ulong> e3);

            //var f = Vector.Narrow(right3, right4);

            ////Console.WriteLine(d);
            ////Console.WriteLine(d2);

            ////Console.WriteLine(e);
            ////Console.WriteLine(e2);
            //Console.WriteLine(e3);
            //Console.WriteLine(f);
        }

        private static uint PlainLoop_1(byte[] mask)
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

        static readonly Vector<uint> CONSTANT_0 = new Vector<uint>(65536);
        static readonly Vector<uint> CONSTANT_1 = new Vector<uint>(16777216);

        private static uint SetMask_SIMD_Optimized_2(byte[] checks)
        {
            //Console.WriteLine(full32);
            Vector.Widen(Vector.ConditionalSelect(Vector.GreaterThan<byte>(new Vector<byte>(checks), Vector<byte>.Zero), LEFT, Vector<byte>.Zero), out Vector<ushort> f16, out Vector<ushort> b16);
            Vector.Widen(b16, out Vector<uint> bf8, out Vector<uint> bb8);
            Vector.Widen(Vector.Multiply(f16, RIGHT), out Vector<uint> ff8, out Vector<uint> fb8);
            Vector.Widen(Vector.BitwiseOr(ff8, fb8), out Vector<ulong> fff4, out Vector<ulong> ffb4);
            Vector.Widen(Vector.BitwiseOr(Vector.Multiply(bf8, CONSTANT_0), Vector.Multiply(bb8, CONSTANT_1)), out Vector<ulong> bf4, out Vector<ulong> bb4);
            return (uint)(Vector.Dot(Vector.BitwiseOr(fff4, ffb4), Vector<ulong>.One) | Vector.Dot(Vector.BitwiseOr(bf4, bb4), Vector<ulong>.One));
        }

        private static uint SetMask_SIMD_Optimized_3(byte[] checks)
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

        static readonly ushort[] F_BITMASK16_SRC = new ushort[]
{
            256, 512, 1024, 2048, 4096, 8192, 16384, 32768,
            1, 1, 1, 1, 1, 1, 1, 1,
        };

        private static uint SetMask_SIMD_Optimized_1(byte[] checks)
        {
            var bitmask = new Vector<byte>(BIT_FIELDS);
            var a = new Vector<byte>(checks);
            var b = Vector<byte>.Zero;

            // INTERNAL TEST 
            var test = Vector.GreaterThan<byte>(a, b);

            var y = Vector.ConditionalSelect(test, bitmask, Vector<byte>.Zero);

            //Console.WriteLine(full32);
            Vector.Widen(y, out Vector<ushort> f16, out Vector<ushort> b16);

            var f_factor16 = new Vector<ushort>(F_BITMASK16_SRC);
            var fraise16 = Vector.ConditionalSelect(f16, f_factor16, Vector<ushort>.Zero);

            var bf_factor16 = new Vector<uint>(65536);
            Vector.Widen(b16, out Vector<uint> bf8, out Vector<uint> bb8);
            var bf_raise8 = Vector.Multiply(bf8, bf_factor16);


            var bb_factor16 = new Vector<uint>(16777216);
            var bb_raise8 = Vector.Multiply(bb8, bb_factor16);

            Vector.Widen(fraise16, out Vector<uint> ff8, out Vector<uint> fb8);

            var f8_combined = Vector.BitwiseOr(ff8, fb8);
            Vector.Widen(f8_combined, out Vector<ulong> fff4, out Vector<ulong> ffb4);

            var b8_combined = Vector.BitwiseOr(bf_raise8, bb_raise8);
            Vector.Widen(b8_combined, out Vector<ulong> bf4, out Vector<ulong> bb4);

            var f4_combined = Vector.BitwiseOr(fff4, ffb4);
            var b4_combined = Vector.BitwiseOr(bf4, bb4);

            var low = Vector.Dot(f4_combined, Vector<ulong>.One);
            var high = Vector.Dot(b4_combined, Vector<ulong>.One);

            //Console.WriteLine(low);
            //Console.WriteLine(high);

            return (uint)(low | high);
        }

        static readonly uint[] BITSHIFT_8 = new uint[]
        {
            1, 2, 4, 8, 16, 32, 64, 128,
        };

        private static uint SetMask_SIMD_Optimized_5(byte[] checks)
        {
            var a = new Vector<byte>(checks);
            var b = Vector<byte>.Zero;

            // INTERNAL TEST 
            var test = Vector.GreaterThan<byte>(a, b);

            var y = Vector.ConditionalSelect(test, Vector<byte>.One, Vector<byte>.Zero);

            var combined = Vector.AsVectorUInt32(y);
            var factor = new Vector<uint>(BITSHIFT_8);
            var shuffle = Vector.Multiply(combined, factor);
            // Console.WriteLine(shuffle);

            // CONSOLIDATE
            Vector.Widen(shuffle, out Vector<ulong> f4, out Vector<ulong> b4);
            var sum = Vector.BitwiseOr(f4, b4);
            return (uint)(sum[0] | sum[1] | sum[2] | sum[3]);
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

        private static uint SetMask16_SIMD_Optimized_1(ushort[] checks, ushort[] flags)
        {
            var a = new Vector<ushort>(checks);
            var b = new Vector<ushort>(flags);

            // INTERNAL TEST 
            var test = Vector.GreaterThan(a, b);
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


        private static uint SetMask_SIMD_Optimized_6(byte[] checks)
        {
            // INTERNAL TEST 
            // CONSOLIDATE
            Vector<uint> BIT_SHIFT_8 = new Vector<uint>(BITSHIFT_8);
            Vector.Widen(Vector.Multiply(Vector.AsVectorUInt32(Vector.ConditionalSelect(Vector.GreaterThan<byte>(new Vector<byte>(checks), Vector<byte>.Zero), Vector<byte>.One, Vector<byte>.Zero)), BIT_SHIFT_8), out Vector<ulong> f4, out Vector<ulong> b4);
            return (uint)Vector.Dot(Vector.BitwiseOr(f4, b4), Vector<ulong>.One);
        }
    }
}
