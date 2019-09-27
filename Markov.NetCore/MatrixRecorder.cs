namespace Markov
{
    public class MatrixRecorder
    {
        public MatrixRecorder(MatrixRecording recording, ushort currentState)
        {
            Recording = recording;
            CurrentState = currentState;
        }

        public MatrixRecording Recording { get; private set; }
        public ushort CurrentState { get; private set; }

        public void Update(ushort nextState)
        {
            Recording.Record(CurrentState, nextState, 1);
            CurrentState = nextState;
        }
    }
}
