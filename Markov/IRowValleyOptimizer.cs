namespace Markov
{
    public interface IRowValleyOptimizer
    {
        bool IsOptimizable(MatrixRowSummary summary, bool[] checks, out IslandResult result);
    }
}
