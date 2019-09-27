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

        private readonly ISlicer mSlicer;
        private readonly IRowValleyOptimizer mSecondary;
        public MatrixOptimizer(ISlicer slicer, IRowValleyOptimizer secondary)
        {
            mSlicer = slicer;
            mSecondary = secondary;
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

                ushort rowDenominator = rowDenominators[i];
                var solution = FindPrimarySolution(rowDenominator, summary);

                if (solution != SolutionType.Unoptimized)
                {
                    InsertSolutionVia2DMatrix(solution, solutions, trees, matrix, i, summary, rowDenominator);
                }
                else
                {
                    if (MaybeSecondaryOptimization(summary, out int groupIndex))
                    {
                        var percentValue = summary.Clusters[groupIndex].Value;
                        var checks = FindPeakIn2DMatrix(summary.NoOfStates, matrix, i, percentValue);
                        var success = InsertSecondarySolution(solutions, summary, rowDenominator, checks, percentValue);
                        if (!success)
                        {
                            InsertSolutionVia2DMatrix(solution, solutions, trees, matrix, i, summary, rowDenominator);
                        }
                    }
                }
            }
            return CreateSolution(solutions, noOfStates, trees);
        }

        private void InsertSolutionVia2DMatrix(SolutionType solutionType, List<MatrixRowSolution> dest, List<RowTree> trees, ushort[,] matrix, int i, MatrixRowSummary summary, ushort rowDenominator)
        {
            var treeIndex = GenerateTreeFromMatrix(solutionType, trees, i, matrix);
            var item = SetupPrimaryLevel(solutionType, summary, rowDenominator, treeIndex);
            dest.Add(item);
        }

        public static bool[] FindPeakIn2DMatrix(int noOfStates, ushort[,] rows, int rowIndex, ushort percentValue)
        {
            var queries = new bool[noOfStates];

            // SETUP UP CHECKS
            for (var i = 0; i < noOfStates; i += 1)
            {
                queries[i] = (rows[rowIndex, i] == percentValue);
            }

            return queries;
        }

        private int GenerateTreeFromMatrix(SolutionType solutionType, List<RowTree> trees, int i, ushort[,] matrix)
        {
            switch (solutionType) {
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
        public static bool DetrimineGroupIndexInPair(MatrixRowSummary summary, out int groupIndex)
        {
            groupIndex = 0;
            if (summary.Clusters[groupIndex].NoOfTimes != 1)
            {
                groupIndex = 1;
                if (summary.Clusters[groupIndex].NoOfTimes != 1)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="groupIndex">Always 0 or 1 if true; -1 if false </param>
        /// <returns></returns>
        public static bool MaybeSecondaryOptimization(MatrixRowSummary summary, out int outIndex)
        {
            if (summary.Clusters.Length != 2)
            {
                outIndex = -1;
                return false;
            }

            if (!DetrimineGroupIndexInPair(summary, out int groupIndex))
            {
                outIndex = -1;
                return false;
            }

            var otherGroup = summary.Clusters[GetOtherGroupIndex(groupIndex)];
            if (summary.NoOfNonZeroPercents != (otherGroup.NoOfTimes + 1))
            {
                outIndex = groupIndex;
                return false;
            }
            else
            {
                outIndex = -1;
                return true;
            }
        }

        private static int GetOtherGroupIndex(int groupIndex)
        {
            return (groupIndex == 0) ? 1 : 0;
        }

        public static bool[] FindPeakIn1DArray(int noOfStates, ushort[] rows, ushort percentValue)
        {
            var queries = new bool[noOfStates];

            // SETUP UP CHECKS
            for (var i = 0; i < noOfStates; i += 1)
            {
                queries[i] = (rows[i] == percentValue);
            }

            return queries;
        }

        public MatrixRowSolution SetupPrimaryLevel(
            SolutionType solutionType,
            MatrixRowSummary summary,
            ushort rowDenominator,
            int treeIndex)
        {
            return new MatrixRowSolution
            {
                Approach = solutionType,
                RowDenominator = rowDenominator,
                Branch = GetBranch(solutionType, summary),
                Left = GetLeft(solutionType, summary),
                Domain = GetDomain(solutionType, summary),
                Tree = treeIndex,
            };
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
                var input = rows[i];
                var summary = Investigate1DArray(i, input);
                ushort rowDenominator = rowDenominators[i];
                var solutionType = FindPrimarySolution(rowDenominator, summary);

                if (solutionType != SolutionType.Unoptimized)
                {
                    InsertSolutionVia1DArray(solutionType, solutions, trees, input, summary, rowDenominator);
                }
                else
                {
                    if (MaybeSecondaryOptimization(summary, out int groupIndex))
                    {
                        var percentValue = summary.Clusters[groupIndex].Value;
                        var checks = FindPeakIn1DArray(summary.NoOfStates, input, percentValue);
                        var success = InsertSecondarySolution(solutions, summary, rowDenominator, checks, percentValue);
                        if (!success)
                        {
                            InsertSolutionVia1DArray(solutionType, solutions, trees, input, summary, rowDenominator);
                        }
                    }
                }
            }
            return CreateSolution(solutions, noOfStates, trees);
        }

        private bool InsertSecondarySolution(List<MatrixRowSolution> dest, MatrixRowSummary summary, ushort rowDenominator, bool[] checks, ushort percentValue)
        {
            if (mSecondary.IsOptimizable(summary, checks, out IslandResult result))
            {
                var secondary = new MatrixRowSolution
                {
                    Approach = SolutionType.SecondaryOptimization,
                    Branch = result.Peak,
                    Left = result.Left,
                    Domain = result.Right,
                    RowDenominator = rowDenominator,
                    Cutoff = percentValue,
                };

                dest.Add(secondary);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void InsertSolutionVia1DArray(SolutionType solutionType, List<MatrixRowSolution> dest, List<RowTree> trees, ushort[] input, MatrixRowSummary summary, ushort rowDenominator)
        {
            var treeIndex = GenerateTreeFromArray(solutionType, trees, input);
            var primary = SetupPrimaryLevel(solutionType, summary, rowDenominator, treeIndex);
            dest.Add(primary);
        }

        private int GetDomain(SolutionType approach, MatrixRowSummary summary)
        {                
            switch (approach)
            {
                case SolutionType.EvenAll:
                    {
                         return Solver.WrapRange(0, summary.NoOfNonZeroPercents - 1, summary.NoOfStates);
                    }
                case SolutionType.EvenOut:
                    return Solver.WrapRange(
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

        public static SolutionType FindPrimarySolution(ushort rowDenominator, MatrixRowSummary summary)
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
            var solutionType = FindPrimarySolution(rowDenominator, summary);

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
