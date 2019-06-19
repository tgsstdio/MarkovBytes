namespace Markov
{
    public class MatrixSolution
    {
        //public ushort[][] Original { get; set; }
        public bool IsOptimized { get; set; }
        public MatrixRowSolution[] Rows { get; set; }
        public int NoOfStates { get; set; }

        public RowTree[] Trees { get; set; }
    }
}