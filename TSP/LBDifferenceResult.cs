using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP
{
    class LbDifferenceResult
    {
        public State IncludeState { get; }
        public State ExcludeState { get; }
        public double LowerBoundDifference { get; }
        public int FromCity = -1;
        public int ToCity = -1;

        public LbDifferenceResult(State includeState, State excludeState, double lowerBoundDifference)
        {
            IncludeState = includeState;
            ExcludeState = excludeState;
            LowerBoundDifference = lowerBoundDifference;
        }
    }
}
