namespace Markov
{
    public class DefaultBitAnalyser : IBitAnalyser
    {
        static int[] MULTIPLY_DEBRUIJN_BITPOSITION2 = {
            0, 1, 28, 2, 29, 14, 24, 3,
            30, 22, 20, 15, 25, 17, 4, 8,
            31, 27, 13, 23, 21, 19, 16, 7,
            26, 12, 18, 6, 11, 5, 10, 9
        };

        public int GetRightmostBit(uint mask)
        {
            // find least significant bit =>  (v & -v)
            // https://graphics.stanford.edu/~seander/bithacks.html#ZerosOnRightMultLookup
            return MULTIPLY_DEBRUIJN_BITPOSITION2[(uint)((mask & -mask) * 0x077CB531U) >> 27];
        }
    }
}
