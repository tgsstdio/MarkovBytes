namespace Markov
{ 
    public class SliceInfo
    {
        public ushort[] Branches { get; set; }
        public int[] Leaves { get; set; }
        public int TotalNonZeros { get; internal set; }
        public int TotalStates { get; internal set; }
    }    
}
