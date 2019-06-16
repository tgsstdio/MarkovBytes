using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Numerics;

namespace MarkovBytes
{
    class Program
    {
        static void Main(string[] args)
        {
            int count = Vector<byte>.Count;

            Debug.Assert(count >= 16);

            Console.WriteLine(nameof(count) + ":" + count);

            var array = new byte[count];
            var arraySpan = new Span<byte>(array);

            // percent exclusion
            array[0] = 255;
            // top level of 1
            array[1] = 4;
            // second level of 2
            array[2] = 3;
            array[3] = 4;
            // third level of 4 
            array[4] = 1;
            array[5] = 5;
            array[6] = 5;
            array[7] = 5;
            // fourth level check of 8
            array[8] = 5;
            array[9] = 5;
            array[10] = 5;
            array[11] = 5;
            array[12] = 5;
            array[13] = 5;
            array[14] = 5;
            array[15] = 5;

            // fifth level check of 16 
            array[16] = 5;
            array[17] = 5;
            array[18] = 5;
            array[19] = 5;
            array[20] = 5;
            array[21] = 5;
            array[22] = 5;
            array[23] = 5;

            array[24] = 5;
            array[25] = 5;
            array[26] = 5;
            array[27] = 5;
            array[28] = 5;
            array[29] = 5;
            array[30] = 5;
            array[31] = 5;

            int flag = 0; // EITHER 1 OR 0
            int mask = 1 << 1;
            int w = 1 << 0 | 1 << 1;
            int expected = 1 << 0;


            // conditional set or clear bits 
            // https://graphics.stanford.edu/~seander/bithacks.html#ConditionalSetOrClearBitsWithoutBranching
            w = (w & ~mask) | (-flag & mask);

            Console.WriteLine("Expected: " + expected.ToString("X"));
            Console.WriteLine("Actual: " + w.ToString("X"));

            var v = 0U;
            var l = (v & -v);
            var r = RightmostBit(v);

            Console.WriteLine(l);
            Console.WriteLine(r);
            TestBitwise(count, array, arraySpan);
        }

        private static int RightmostBit(uint v)
        {
            // find least significant bit =>  (v & -v)
                // https://graphics.stanford.edu/~seander/bithacks.html#ZerosOnRightMultLookup
            return multiplyDeBruijnBitPosition2[(uint)((v & -v) * 0x077CB531U) >> 27];
        }

        static int[] multiplyDeBruijnBitPosition2 = {
            0, 1, 28, 2, 29, 14, 24, 3,
            30, 22, 20, 15, 25, 17, 4, 8,
            31, 27, 13, 23, 21, 19, 16, 7,
            26, 12, 18, 6, 11, 5, 10, 9
        };

        private static void TestBitwise(int count, byte[] array, Span<byte> arraySpan)
        {

            //Length depends on your Vector<int>.Count. In my computer it is 4
            var vector1 = new Vector<byte>(arraySpan); //vector1 == {<4, 4, 4, 4>}

            const int NO_OF_RUNS = 10000;

            var simdTImer = new Stopwatch();
            var cpuTimer = new Stopwatch();

            const byte randomValue = 5;
            int MAX_COUNT = count - 1;
            for (var i = 0; i < NO_OF_RUNS; i++)
            {
                simdTImer.Start();

                var vector2 = new Vector<byte>(randomValue); //vector2 == {<5, 5, 5, 5>}

                var mask2 = Vector.LessThan(vector1, vector2); //mask == {<0, 0, 0, 0>}
                var chunk = Vector.ConditionalSelect(mask2, Vector<byte>.One, Vector<byte>.Zero);

                uint aReduce = 0;

                int k = MAX_COUNT;
                while (k > 0)
                {

                    if (mask2[k] > 0)
                        aReduce |= (1U << k);
                    //aReduce = (uint) (aReduce & (uint) ~(1U << k)) | ((uint) -chunk[k] & (1U << k));
                    k--;
                }
                var t = RightmostBit(aReduce);
                simdTImer.Stop();

                cpuTimer.Start();
                uint bReduce = 0;
                for (var j = 0; j < count; j += 1)
                {
                    if (array[j] < randomValue)
                        bReduce |= (1U << j);
                }

                var r = RightmostBit(bReduce);
                cpuTimer.Stop();
            }

            Console.WriteLine("A: " + simdTImer.Elapsed.ToString());
            Console.WriteLine("B: " + cpuTimer.Elapsed.ToString());

            //var reduced = Vector.AsVectorUInt64(mask);


            //Console.WriteLine("Reduce:" + reduced);
            //Console.WriteLine(nameof(mask) + " " + mask);
            //var selected = Vector.ConditionalSelect(mask, vector1, vector2); //selected == {<5, 5, 5, 5>}

            //vector1 = new Vector<byte>(4); //vector1 == {<4, 4, 4, 4>}
            //vector2 = new Vector<byte>(3); //vector2 == {<3, 3, 3, 3>}
            //mask = Vector.GreaterThan(vector1, vector2); //mask == {<-1, -1, -1, -1>}
            //selected = Vector.ConditionalSelect(mask, vector1, vector2); //selected == {<4, 4, 4, 4>}

            //mask = new Vector<byte>(123); //mask == {<123, 123, 123, 123>}
            //selected = Vector.ConditionalSelect(mask, vector1, vector2); //selected == {<0, 0, 0, 0>}

            //mask = new Vector<byte>(4);
            //selected = Vector.ConditionalSelect(mask, vector1, vector2); //selected == {<7, 7, 7, 7>}

            // Console.WriteLine(selected);
        }
    }
}
