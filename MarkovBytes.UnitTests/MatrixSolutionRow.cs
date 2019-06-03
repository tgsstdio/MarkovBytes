namespace Markov
{
    public struct MatrixSolutionRow
    {
        public SolutionType Approach { get; set; }
        public int Branch { get; set; }
        public int Left { get; set; }
        public int Domain { get; set; }
    }
}
