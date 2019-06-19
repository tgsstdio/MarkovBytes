namespace Markov
{
    public interface INodeEvaluator
    {
        uint Evaluate(TestChunk testChunk, ushort[] branches, ushort singleValue);
    }
}
