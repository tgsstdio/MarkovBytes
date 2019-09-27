using System;

namespace Markov
{
    public class MatrixRecording
    {
        public ushort[] RowDenominators { get; set; }
        public ushort[][] Rows { get; set; }

        public void Record(ushort previousState, ushort nextState, ushort noOfTimes)
        {
            RowDenominators[previousState] += noOfTimes;
            Rows[previousState][nextState] += noOfTimes;
        }

        public static MatrixRecording Generate(uint noOfStates)
        {
            var rows =
                (noOfStates == 0)
                ? Array.Empty<ushort[]>()
                : new ushort[noOfStates][];

            for (var i = 0; i < noOfStates; i += 1)
            {
                rows[i] = new ushort[noOfStates];
            }

            return new MatrixRecording
            {
                RowDenominators =
                    (noOfStates == 0)
                        ? Array.Empty<ushort>()
                        : new ushort[noOfStates],
                Rows = rows
            };
        }
    }
}
