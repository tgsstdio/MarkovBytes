namespace Markov
{
    public class RowTree
    {
        public TestChunk[] TestChunks { get; set; }
        public Instruction[] Instructions { get; set; }
        public int[] Leaves { get; set; }
        public ushort[] Branches { get; set; }
    }
}