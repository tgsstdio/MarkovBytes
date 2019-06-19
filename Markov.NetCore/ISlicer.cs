namespace Markov
{
    public interface ISlicer
    {
        RowTree[] Slice(ushort[][] srcRows);
        RowTree SliceRow(ushort[] row);
    }
}
