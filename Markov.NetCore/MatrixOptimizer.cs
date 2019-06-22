using System;
using System.Collections.Generic;

namespace Markov
{
    public class MatrixOptimizer : IMatrixOptimizer
    {
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

        private ISlicer mSlicer;
        public MatrixOptimizer(ISlicer slicer)
        {
            mSlicer = slicer;
        }

        public MatrixSolution Optimize(ushort[] rowDenominators, ushort[,] matrix)
        {
            var noOfRows = matrix.GetLength(0);
            var noOfColumns = matrix.GetLength(1);
            ValidateDimensions(noOfRows, noOfColumns);

            var solutions = new List<MatrixRowSolution>();
            var noOfStates = noOfRows;

            var trees = new List<RowTree>();
            for (var i = 0; i < noOfStates; i += 1)
            {
                var summary = Investigate2DArray(i, noOfStates, matrix);

                var solutionType = GetSolutionType(rowDenominators[i], summary);
                var item = new MatrixRowSolution
                {
                    Approach = solutionType,
                    RowDenominator = rowDenominators[i],
                    Branch = GetBranch(solutionType, summary),
                    Left = GetLeft(solutionType, summary),
                    Domain = GetDomain(solutionType, summary),
                    Tree = GenerateTreeFromMatrix(solutionType, trees, i, matrix),
                };

                solutions.Add(item);
            }
            return CreateSolution(solutions, noOfStates, trees);
        }

        private int GenerateTreeFromMatrix(SolutionType solutionType, List<RowTree> trees, int i, ushort[,] matrix)
        {
            switch(solutionType) {
                case SolutionType.Sparse:
                case SolutionType.Unoptimized:
                    trees.Add(mSlicer.SliceMatrix(i, matrix));
                    return trees.Count - 1;            
                default:
                    return -1;
            }
        }

        private int GenerateTreeFromArray(SolutionType solutionType, List<RowTree> trees, ushort[] input)
        {
            switch (solutionType)
            {
                case SolutionType.Sparse:
                case SolutionType.Unoptimized:
                    trees.Add(mSlicer.SliceRow(input));
                    return trees.Count - 1;
                default:
                    return -1;
            }
        }    

        private static void ValidateDimensions(int noOfRows, int noOfColumns)
        {
            if (noOfRows != noOfColumns)
                throw new System.Exception("Invalid Matrix Dimensions");
        }

        private MatrixRowSummary Investigate2DArray(int i, int noOfStates, ushort[,] matrix)
        {
            var summary = new MatrixRowSummary
            {
                Row = i,
                NoOfStates = noOfStates,
            };

            var clusters = GetClustersVia2DArray(summary, noOfStates, i, matrix);
            CopyClusters(clusters, summary);

            return summary;
        }

        private static IDictionary<ushort, ValueCluster> GetClustersVia2DArray(MatrixRowSummary summary, int noOfStates, int i, ushort[,] rowValues)
        {
            var lookup = new SortedDictionary<ushort, ValueCluster>(
                new HigherPercentComparer());
            for (int j = 0; j < noOfStates; j += 1)
            {
                AnalyseRowValue(summary, i, j, rowValues[i, j], lookup);
            }

            return lookup;
        }


        public MatrixSolution Optimize(ushort[] rowDenominators, ushort[][] rows)
        {
            int noOfStates = rows.Length;

            for (var i = 0; i < noOfStates; i += 1)
            {
                var noOfColumns = rows[i].Length;
                ValidateDimensions(noOfStates, noOfColumns);
            }

            var solutions = new List<MatrixRowSolution>();
            var trees = new List<RowTree>();

            for (var i = 0; i < noOfStates; i += 1)
            {
                var summary = Investigate1DArray(i, rows[i]);
                var solutionType = GetSolutionType(rowDenominators[i], summary);
                var item = new MatrixRowSolution
                {
                    Approach = solutionType,
                    RowDenominator = rowDenominators[i],
                    Branch = GetBranch(solutionType, summary),
                    Left = GetLeft(solutionType, summary),
                    Domain = GetDomain(solutionType, summary),
                    Tree = GenerateTreeFromArray(solutionType, trees, rows[i]),
                };

                solutions.Add(item);
            }
            return CreateSolution(solutions, noOfStates, trees);
        }

        private int GetDomain(SolutionType approach, MatrixRowSummary summary)
        {                
            switch (approach)
            {
                case SolutionType.EvenAll:
                    return Solver.GetDomain(0, summary.NoOfNonZeroPercents - 1, summary.NoOfStates);
                case SolutionType.EvenOut:
                    return Solver.GetDomain(
                        summary.Row + 1,
                        summary.Row - 1,
                        summary.NoOfStates);
                default:
                    return default(int);
            }
        }


        private int GetLeft(SolutionType approach, MatrixRowSummary summary)
        {
            switch (approach)
            {
                case SolutionType.EvenOut:
                    return summary.Row + 1;
                default:
                    return default(int);
            }
        }

        private static int GetBranch(SolutionType approach, MatrixRowSummary summary)
        {
           switch(approach) {
                case SolutionType.DeadEnd:
                case SolutionType.Redirect:                
                    return summary.Clusters[0].First;
                case SolutionType.EvenAll:
                case SolutionType.EvenOut:
                    return summary.Row;
                default:
                    return default(int);
            }
        }

        private static MatrixSolution CreateSolution(List<MatrixRowSolution> solutions, int noOfStates, List<RowTree> trees)
        {
            bool isOptimized = false;
            foreach (var row in solutions)
            {
                if (row.Approach != SolutionType.Unoptimized)
                {
                    isOptimized = true;
                    break;
                }
            }

            return new MatrixSolution
            {
                //  Original = grids,
                NoOfStates = noOfStates,
                IsOptimized = isOptimized,
                Rows = solutions.ToArray(),
                Trees = trees.ToArray(),
            };
        }

        public MatrixRowSummary Investigate1DArray(int i, ushort[] rowValues)
        {
            // Check if all zeros   

            var summary = new MatrixRowSummary
            {
                Row = i,
                NoOfStates = rowValues.Length,
            };

            var clusters = GetClustersVia1DArray(summary, i, rowValues);
            CopyClusters(clusters, summary);

            return summary;
        }

        private static void CopyClusters(IDictionary<ushort, ValueCluster> src, MatrixRowSummary dest)
        {
            dest.Clusters = new ValueCluster[src.Count];
            src.Values.CopyTo(dest.Clusters, 0);
        }

        private static IDictionary<ushort, ValueCluster> GetClustersVia1DArray(MatrixRowSummary summary, int i, ushort[] rowValues)
        {
            var lookup = new SortedDictionary<ushort, ValueCluster>(
                new HigherPercentComparer());
            int noOfValues = rowValues.Length;
            for (int j = 0; j < noOfValues; j += 1)
            {
                ushort value = rowValues[j];
                AnalyseRowValue(summary, i, j, value, lookup);
            }

            return lookup;
        }

        private static void AnalyseRowValue(MatrixRowSummary summary, int i, int j, ushort percent, SortedDictionary<ushort, ValueCluster> lookup)
        {
            if (j == i && percent > 0)
            {
                summary.SelfPercent = percent;
            }

            if (percent == 0)
            {
                summary.NoOfZeroPercents += 1;
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
                        First = j,
                        NoOfTimes = 1,
                    };

                    lookup.Add(percent, found);
                }


                summary.NoOfNonZeroPercents += 1;
            }
        }

        public static SolutionType GetSolutionType(ushort rowDenominator, MatrixRowSummary summary)
        {
            if (summary.NoOfStates == summary.NoOfZeroPercents)
            {
                return SolutionType.NoOperation;
            }

            if (summary.Clusters.Length == 1)
            {
                if (summary.Clusters[0].Value == rowDenominator && summary.Clusters[0].NoOfTimes == 1)
                {
                    return summary.Row == summary.Clusters[0].First ? SolutionType.DeadEnd : SolutionType.Redirect;
                }

                if (summary.NoOfNonZeroPercents == summary.NoOfStates)
                {
                    return SolutionType.EvenAll;
                }

                if (!summary.SelfPercent.HasValue
                  && summary.NoOfNonZeroPercents == summary.NoOfStates - 1)
                {
                    return SolutionType.EvenOut;
                }
            }

            if (summary.NoOfNonZeroPercents < summary.NoOfStates)
            {
                return SolutionType.Sparse;
            }

            return SolutionType.Unoptimized;
        }

        public MatrixRowSolution Evaluate(ushort rowDenominator, MatrixRowSummary summary)
        {
            var solutionType = GetSolutionType(rowDenominator, summary);

            return new MatrixRowSolution
            {
                Approach = solutionType,
                RowDenominator = rowDenominator,
                Branch = GetBranch(solutionType, summary),
                Left = GetLeft(solutionType, summary),
                Domain = GetDomain(solutionType, summary),
            };
        }
    }

}
