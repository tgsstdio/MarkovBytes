using System.Collections.Generic;

namespace Markov
{
    public class MatrixRowSummary
    {
        public int NoOfZeroPercents { get; set; }
        public int NoOfNonZeroPercents { get; set; }
        public ushort? SelfPercent { get; set; }
        public ValueCluster[] Clusters { get; set; }
        public int NoOfStates { get; internal set; }
        public int Row { get; internal set; }
    }
}
