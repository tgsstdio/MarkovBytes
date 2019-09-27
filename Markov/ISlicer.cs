namespace Markov
{
    public interface ISlicer
    {
        RowTree[] Slice(ushort[][] srcRows);
        RowTree SliceRow(ushort[] row);

        RowTree SliceMatrix(int i, ushort[,] matrix);
    }
}
