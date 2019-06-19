namespace Markov
{
    public interface IMatrixOptimizer
    {
        MatrixSolution Optimize(ushort[] rowDenominators, ushort[][] rows);

        MatrixSolution Optimize(ushort[] rowDenominators, ushort[,] matrix);
    }
}