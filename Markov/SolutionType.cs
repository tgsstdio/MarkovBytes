namespace Markov
{
    public enum SolutionType : uint
    {
        Unoptimized = 0,
        NoOperation = 1,
        EvenAll,
        EvenOut,
        Redirect,
        DeadEnd,
        Sparse,
        SecondaryOptimization,
    }
}