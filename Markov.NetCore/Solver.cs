using System;

namespace Markov
{
    public class Solver
    {
        private readonly IRandomNumberGenerator mRNG;
        private readonly Stepper mStepper;

        public Solver(IRandomNumberGenerator rng, Stepper stepper)
        {
            mRNG = rng;
            mStepper = stepper;
        }

        public bool Resolve(MatrixSolution solution, int pastState, out int next)
        {
            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            if (pastState < 0 || pastState >= solution.NoOfStates)
                throw new ArgumentOutOfRangeException(nameof(pastState), pastState, nameof(pastState) + " must be within [0,  " + solution.NoOfStates + ")");

            if (!solution.IsOptimized)
            {
                next = -1;
                return false;
            }

            var current = solution.Rows[pastState];
            var approach = current.Approach;
            switch (approach)
            {
                case SolutionType.DeadEnd:
                    next = current.Branch;
                    return true;
                case SolutionType.Redirect:
                    next = current.Branch;
                    return true;
                case SolutionType.EvenAll:
                case SolutionType.EvenOut:
                    int randValue = mRNG.Next(current.RowDenominator);
                    next = GetEvenInRange(randValue,
                        current.RowDenominator,
                        current.Left,
                        current.Domain,
                        solution.NoOfStates);
                    return true;
                case SolutionType.Sparse:
                    var tree = solution.Trees[current.Tree];                    
                    var result = mStepper.Iterate(tree, (ushort) mRNG.Next(current.RowDenominator));
                    if (result != -1)
                    {
                        next = result;
                        return true;
                    }
                    else
                    {
                        next = default(int);
                        return false;
                    }
                default:
                    next = default(int);
                    return false;
            }
        }

        public static int GetDomain(int left, int right, int arrayLength)
        {
            // move 
            int end = right;
            end += (right <= left) ? arrayLength : 0;

            // LERP
            int domain = end - left;
            return domain;
        }

        public static int GenerateEvenAll(int next, int left, int count, int N)
        {
            // var normal = GetNormalized(next + offset, offset, offset + count, count);

            // return GetIndex(normal, N, offset, count);

            int right = left + count - 1;
            int domain = GetDomain(left, right, N);
            return GetEvenInRange(next, count, left, domain, N);
        }

        public static int GenerateEvenOut(int next, int self, int n)
        {
            int left = self + 1;
            int right = self - 1;
            int domain = GetDomain(left, right, n);
            return GetEvenInRange(next, n - 1, left, domain, n);


            //return GenerateEvenAll(next, self + 1, n - 1, n);
        }

        // FUNCTION FOR ALL Even probability
        public static int GetEvenInRange(int numRand, int maxRand, int left, int domain, int arrayLength)
        {
            // int n = numRand - minRand; int d = maxRand - minRand;
            //int domain = GetDomain(left, right, arrayLength);
            int windowSize = domain + 1;
            int shift = (numRand * windowSize) / maxRand;

            // BOUNDED VALUES 100% => < 1.0
            int offset = Clamp(shift, 0, domain);
            // ? windowSize - 1 // LAST VALUE
            // : shift;

            return (left + offset) % arrayLength;
        }

        private static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static int GetEvenAllTransistion(
            int next, int maxDomain, int N, int offset)
        {
            //var normal = GetNormalized(next, minDomain, maxDomain, N);
            //var index = GetIndex(normal, N, offset, maxDomain - minDomain);
            //return index;

            int right = N - 1 + offset;
            int domain = GetDomain(offset, right, N);
            return GetEvenInRange(next, maxDomain, offset,
                domain, N);
        }
    }
}