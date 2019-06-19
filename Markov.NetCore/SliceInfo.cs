namespace Markov
{ 
    public class SliceInfo
    {
        public ushort[] Branches { get; set; }
        public int[] Leaves { get; set; }
        public int TotalNonZeros { get; set; }
        public int TotalStates { get; set; }
    }    
}
