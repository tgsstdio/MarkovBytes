namespace Markov
{
    public struct MatrixRowSolution
    {
        public SolutionType Approach { get; set; }
        public int Branch { get; set; }
        public int Left { get; set; }
        public int Domain { get; set; }
        public ushort RowDenominator { get; set; }
        public int Tree { get; internal set; }
        public int Cutoff { get; internal set; }
    }
}
