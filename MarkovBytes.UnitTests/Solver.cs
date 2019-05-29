namespace Tests
{
    public class Solver<T>
    {
        private readonly Solution<T>[] mSolutions;

        public Solver(Solution<T>[] solutions)
        {
            mSolutions = solutions;
        }

        public bool Resolve(int pastState, out T next)
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
                    next = default(T);
                    return false;
            }
        }
    }
}