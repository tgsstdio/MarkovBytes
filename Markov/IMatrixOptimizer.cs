namespace Markov
{
    public interface IMatrixOptimizer
    {
        ushort MaxProbability { get; set; }

        MatrixSolution Optimize(ushort[][] grids);

        MatrixSolution Optimize(ushort[,] grids);
    }
}