namespace Markov
{
    public interface ISliceRuler
    {
        int BucketSize { get; }

        ushort[] GetBitMasks(int length);
        void GetChunkInfo(int length, out int wholeBuckets, out int remainder);
        int GetNoOfBuckets(int length);
    }
}
