namespace Markov
{
    public class Solver
    {
        private readonly Solution[] mSolutions;

        public Solver(Solution[] solutions)
        {
            mSolutions = solutions;
        }

        public bool Resolve(int pastState, out ushort next)
        {

            var current = mSolutions[pastState];
            var approach = current.Approach;
            switch (approach)
            {
                case SolutionType.DeadEnd:
                    next = current.Id;
                    return true;
                case SolutionType.Redirect:
                    next = current.Branch;
                    return true;
                default:
                    next = default(ushort);
                    return false;
            }
        }
    }
}