namespace Markov
{
    public interface ISlicer
    {
        public RowTree[] Slice(ushort[][] srcRows);
        RowTree SliceRow(ushort[] row);
    }
}
