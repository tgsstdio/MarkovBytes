using BenchmarkDotNet.Running;

namespace NarrowWiden.Benchmarks
{
    class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<SetBitwise32BitMask>();
        }
    }
}
