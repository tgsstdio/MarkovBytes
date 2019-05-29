namespace Tests
{
    public class Solution<T>
    {
        public SolutionType Approach { get; set; }
        public T Id { get; set; }
        public T Branch { get; set; }
        public float Threshold { get; set; }
    }
}