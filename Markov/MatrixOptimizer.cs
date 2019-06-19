﻿using System.Collections.Generic;

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

        public MatrixSolution Optimize(ushort[] rowDenominators, ushort[,] matrix)
        {
            var noOfRows = matrix.GetLength(0);
            var noOfColumns = matrix.GetLength(1);
            ValidateDimensions(noOfRows, noOfColumns);

            var solutions = new List<MatrixRowSolution>();
            var noOfStates = noOfRows;

            for (var i = 0; i < noOfStates; i += 1)
            {
                var summary = Investigate2DArray(i, noOfStates, matrix);
                MatrixRowSolution item = Evaluate(rowDenominators[i], summary);
                solutions.Add(item);
            }

            return CreateSolution(solutions, noOfStates);
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

            for (var i = 0; i < noOfStates; i += 1)
            {
                var summary = Investigate1DArray(i, rows[i]);
                MatrixRowSolution item = Evaluate(rowDenominators[i], summary);
                solutions.Add(item);
            }
            return CreateSolution(solutions, noOfStates);
        }

        private static MatrixSolution CreateSolution(List<MatrixRowSolution> solutions, int noOfStates)
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

        public MatrixRowSolution Evaluate(ushort rowDenominator, MatrixRowSummary summary)
        {
            if (summary.NoOfStates == summary.NoOfZeroPercents)
            {
                return new MatrixRowSolution
                {
                    Approach = SolutionType.NoOperation,
                    RowDenominator = rowDenominator,
                };
            }

            if (summary.Clusters.Length == 1)
            {
                var top = summary.Clusters[0];

                if (top.Value == rowDenominator && top.NoOfTimes == 1)
                {
                    if (summary.Row == top.First)
                    {
                        return new MatrixRowSolution
                        {
                            Approach = SolutionType.DeadEnd,
                            Branch = top.First,
                            RowDenominator = rowDenominator,
                        };
                    }

                    // ELSE REDIRECT
                    return new MatrixRowSolution
                    {
                        Approach = SolutionType.Redirect,
                        Branch = top.First,
                        RowDenominator = rowDenominator,
                    };
                }
                if (summary.NoOfNonZeroPercents == summary.NoOfStates)
                {
                    // TODO: check for consecutive non-zeros
                    int left = 0;
                    int count = summary.NoOfNonZeroPercents;
                    int right = left + count - 1;
                    int domain = Solver.GetDomain(left, right, summary.NoOfStates);

                    return new MatrixRowSolution
                    {
                        Approach = SolutionType.EvenAll,
                        Branch = summary.Row,
                        Domain = domain,
                        RowDenominator = rowDenominator,
                    };
                }

                if (!summary.SelfPercent.HasValue
                  && summary.NoOfNonZeroPercents == summary.NoOfStates - 1)
                {
                    var left = summary.Row + 1;
                    var right = summary.Row - 1;
                    var domain = Solver.GetDomain(
                        left,
                        right,
                        summary.NoOfStates);

                    return new MatrixRowSolution
                    {
                        Approach = SolutionType.EvenOut,
                        Branch = summary.Row,
                        Left = left,
                        Domain = domain,
                        RowDenominator = rowDenominator,
                    };
                }
            }

            //else DEFAULT
            return new MatrixRowSolution
            {
                Approach = SolutionType.Unoptimized,
                RowDenominator = rowDenominator,
            };

        }
    }

}
