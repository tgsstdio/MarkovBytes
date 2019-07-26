using System;

namespace Markov
{

    /// <summary>
    /// island / valley Even-All (2nd optimization function) is based on trying to find a same % once 
    /// the outlier has been excluded from the range of non-zero probabilities.
    /// HOWEVER this case is only valid if the outlier outside the string of repeating of the percentage.
    /// </summary>
    public class IslandValleyOptimizer : IRowValleyOptimizer
    {
        public static int GetNormalizedPeakIndex(int index, int length)
        {
            if (index >= length)
                return index % length;

            if (index < 0)
                return ((index % length) + length) % length;

            return index;
        }

        public bool IsOptimizable(MatrixRowSummary summary, bool[] queries, out IslandResult result)
        {
            if (summary.Clusters.Length != 2)
            {
                result = new IslandResult
                {
                    Status = IslandOptimizationStatus.Invalid,
                };
                return false;
            }

            var peakGroupIndex = summary.Clusters[0].NoOfTimes == 1 ? 0 : 1;
            var otherGroupIndex = (peakGroupIndex == 0) ? 1 : 0;

            var otherGroup = summary.Clusters[otherGroupIndex];
            if (summary.NoOfNonZeroPercents != (otherGroup.NoOfTimes + 1))
            {
                result = new IslandResult
                {
                    Status = IslandOptimizationStatus.Invalid,
                };
                return false;
            }

            //var queries = new bool[summary.NoOfStates];

            //// SETUP UP CHECKS
            //for (var i = 0; i < summary.NoOfStates; i += 1)
            //{
            //    queries[i] = (source[i] == otherGroup.Value);
            //}

            result = Examine(queries, summary.Clusters[peakGroupIndex].First);
            return (result.Status == IslandOptimizationStatus.EvenAll);
        }

        public static IslandResult Examine(bool[] queries, int peakIndex)
        {
            if (queries == null)
            {
                throw new ArgumentNullException(nameof(queries));
            }

            var length = queries.Length;
            if (length <= 0)
            {
                return new IslandResult
                {
                    Status = IslandOptimizationStatus.NoOperation,                    
                };
            }

            int normalizedPeak = GetNormalizedPeakIndex(peakIndex, length);

            bool firstValue = queries[0];
            bool inverseValue = !firstValue;

            var left = 0;
            // GET FIRST INVERSE
            for(var i = 1; i < length; i += 1)
            {
                if (queries[i] == inverseValue)
                {
                    break;
                }
                left = i;
            }

            var right = length - 1;
            if (left > right)
            {
                if (firstValue)
                {
                    return new IslandResult
                    {
                        Status = IslandOptimizationStatus.EvenAll,
                        Peak = normalizedPeak,
                        Left = 0,
                        Right = right,
                    };
                }
                else
                {
                    // NO OP
                    return new IslandResult
                    {
                        Status = IslandOptimizationStatus.NoOperation,
                        Peak = normalizedPeak,
                        Left = 0,
                        Right = -1,
                    };
                }
            }

            // GET LAST BOUNDARY
            bool lastValue = queries[right];
            if (lastValue == firstValue)
            {
                for (int i = right; i >= 0; i -= 1)
                {
                    if (queries[i] == firstValue)
                    {
                        break;
                    }
                    right = i;
                }
            }

            bool inconsistencyFound = false;
            // CHECK BOUNDS IS CONSIST
            for (var i = left + 1; i <= right - 1; i += 1)
            {
                if (queries[i] == firstValue)
                {
                    inconsistencyFound = true;
                    break;
                }
            }

            if (inconsistencyFound)
            {
                return new IslandResult
                {
                    Peak = normalizedPeak,
                    Status = IslandOptimizationStatus.Invalid,
                };
            }

            var start =
                (firstValue != lastValue)
                ? 0
                : left + 1; // VALLEY                 

            var end =
                (firstValue != lastValue)
                   ? left
                   : right - 1; // VALLEY
                           
          
            // IF firstValue is true => then valley scenario
                // THEN INVALID if peak index is found outside (i.e. not in valley)
            // IF firstValue is false => then island scenario
                // THEN INVALID if peak index is found inside (i.e peak makes range inconsistant)  
                
            // IN RANGE
            if (start <= normalizedPeak && normalizedPeak <= end)
            {
                // FRONT BUMP || ISLAND
                if ((firstValue && !lastValue) || (!firstValue && !lastValue))
                {
                    return new IslandResult
                    {
                        Peak = normalizedPeak,
                        Status = IslandOptimizationStatus.Invalid,
                    };
                }
            }

            // BUMP ON LEFT 
            if (firstValue && !lastValue)
            {
                return new IslandResult
                {
                    Peak = normalizedPeak,
                    Status = IslandOptimizationStatus.EvenAll,
                    Left = 0,
                    Right = left,
                };
            }

            // BUMP ON RIGHT
            if (!firstValue && lastValue)
            {
                return new IslandResult
                {
                    Peak = normalizedPeak,
                    Status = IslandOptimizationStatus.EvenAll,
                    Left = right,
                    Right = length - 1,
                };
            }

            if (!firstValue)
            {
                return new IslandResult
                {
                    Peak = normalizedPeak,
                    Status = IslandOptimizationStatus.EvenAll,
                    Left = start,
                    Right = end,
                };
            }
            else
            {
                return new IslandResult
                {
                    Peak = normalizedPeak,
                    Status = IslandOptimizationStatus.EvenAll,
                    Left = right,
                    Right = left,
                };
            }
        }
    }
}
