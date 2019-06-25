namespace Markov
{
    public struct IslandResult
    {
        public IslandOptimizationStatus Status { get; set; }

        public int Left { get; set; }

        public int Right { get; set; }
        public int Peak { get; internal set; }
    }
}
