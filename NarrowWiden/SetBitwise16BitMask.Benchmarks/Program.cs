using BenchmarkDotNet.Running;

namespace SetBitwise16BitMask.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<SetBitwise16BitMask>();
        }
    }
}
