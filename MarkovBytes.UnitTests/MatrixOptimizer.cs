using System.Collections.Generic;

namespace Markov
{
    public class MatrixOptimizer : IMatrixOptimizer
    {
        public ushort MaxProbability { get; set; }

        class HigherPercentComparer : Comparer<ushort>
        {
            public override int Compare(ushort x, ushort y)
            {
                if (x > y)
                    return -1;

                if (x < y)
                    return 1;

                return 0;
            }
        }

        public MatrixSolution Optimize(ushort[][] grids)
        {
            var solutions = new List<MatrixSolutionRow>();

            for(var i = 0; i < grids.Length; i += 1) {
                var row = grids[i];

                var summary = Investigate(0, row);
                solutions.Add(Evaluate(summary));
            }

            bool isOptimized = false;
            foreach(var row in solutions)
            {
                if (row.Approach != SolutionType.Unoptimized)
                {
                    isOptimized = true;
                    break;
                }
            }

            return new MatrixSolution
            {
                IsOptimized = isOptimized,
                Rows = solutions.ToArray(),
            };
        }

        public MatrixRowSummary Investigate(int self, ushort[] rowValues)
        {
            // Check if all zeros
            int noOfValues = rowValues.Length;

            ushort? selfPercent = null;
            int noOfZeroPercents = 0;
            int noOfNonZeroPercents = 0;
            var lookup = new SortedDictionary<ushort, ValueCluster>(
                new HigherPercentComparer());

            for (int i = 0; i < noOfValues; i += 1)
            {
                ushort percent = rowValues[i];
                if (i == self && rowValues[i] > 0)
                {
                    selfPercent = percent;
                }

                if (percent == 0)
                {
                    noOfZeroPercents += 1;
                }
                else
                {
                    if (lookup.TryGetValue(percent, out ValueCluster found))
                    {
                        found.NoOfTimes += 1;
                    }
                    else
                    {
                        found = new ValueCluster
                        {
                            Value = percent,
                            First = i,
                            NoOfTimes = 1,
                        };

                        lookup.Add(percent, found);
                    }


                    noOfNonZeroPercents += 1;
                }
            }

            var records = new ValueCluster[lookup.Count];
            lookup.Values.CopyTo(records, 0);

            return new MatrixRowSummary
            {
                Row = self,
                Clusters = records,
                SelfPercent = selfPercent,
                NoOfStates = noOfValues,
                NoOfZeroPercents = noOfZeroPercents,
                NoOfNonZeroPercents = noOfNonZeroPercents,
            };
        }

        public MatrixSolutionRow Evaluate(MatrixRowSummary stats)
        {
            if (stats.NoOfStates == stats.NoOfZeroPercents)
            {
                return new MatrixSolutionRow
                {
                    Approach = SolutionType.NoOperation,
                };
            }

            if (stats.Clusters.Length == 1)
            {
                var top = stats.Clusters[0];

                if (top.Value == MaxProbability)
                {
                    if (stats.Row == top.First)
                    {
                        return new MatrixSolutionRow
                        {
                            Approach = SolutionType.DeadEnd,
                            Branch = top.First,
                        };
                    }

                    // ELSE REDIRECT
                    return new MatrixSolutionRow
                    {
                        Approach = SolutionType.Redirect,
                        Branch = top.First,
                    };
                }
                if (stats.NoOfNonZeroPercents == stats.NoOfStates)
                {
                    return new MatrixSolutionRow
                    {
                        Approach = SolutionType.EvenAll,
                    };
                }

                if (!stats.SelfPercent.HasValue
                  && stats.NoOfNonZeroPercents == stats.NoOfStates - 1)
                {
                    return new MatrixSolutionRow
                    {
                        Approach = SolutionType.EvenOut,
                        Branch = stats.Row,
                    };
                }
            }

            //else DEFAULT
            return new MatrixSolutionRow
            {
                Approach = SolutionType.Unoptimized,
            };

        }
    }

}
