using System.Numerics;

namespace Markov
{ 
    public class TestChunk
    {
        public int LeafLength { get; set; }
        public int LeafOffset { get; set; }
        public Vector<ushort> Keys { get; set; }
    }    
}
