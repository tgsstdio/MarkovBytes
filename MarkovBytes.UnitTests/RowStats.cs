using System.Collections.Generic;

namespace Markov
{
    public class RowStats
    {
        public int NoOfZeroPercents { get; set; }
        public int NoOfNonZeroPercents { get; set; }
        public ushort? SelfPercent { get; set; }
        public RowRecord[] Records { get; set; }
        public int NoOfStates { get; internal set; }
    }
}
