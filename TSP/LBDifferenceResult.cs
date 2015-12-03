using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP
{
    class LbDifferenceResult
    {
        public Matrix IncludeMatrix { get; }
        public Matrix ExcludeMatrix { get; }
        public double LowerBoundDifference { get; }

        public LbDifferenceResult(Matrix includeMatrix, Matrix excludeMatrix, double lowerBoundDifference)
        {
            IncludeMatrix = includeMatrix;
            ExcludeMatrix = excludeMatrix;
            LowerBoundDifference = lowerBoundDifference;
        }
    }
}
