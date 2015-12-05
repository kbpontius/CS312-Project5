using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP
{
    class LbDifferenceResult
    {
        public State IncludeState;
        public State ExcludeState;
        public double LowerBoundDifference { get; }
        public int FromCity;
        public int ToCity;

        public LbDifferenceResult(State includeState, State excludeState, double lowerBoundDifference, int fromCity, int toCity)
        {
            IncludeState = includeState;
            ExcludeState = excludeState;
            LowerBoundDifference = lowerBoundDifference;
            FromCity = fromCity;
            ToCity = toCity;
        }
    }
}
