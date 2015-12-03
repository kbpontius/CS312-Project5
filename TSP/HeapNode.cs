namespace TSP
{
    class HeapNode
    {
        public State State;
        public double ReductionCost;
        public int LookupIndex;
        public int BackPointer;

        public HeapNode(State state, double reductionCost, int lookupIndex, int backPointer)
        {
            State = state;
            LookupIndex = lookupIndex;
            ReductionCost = state.GetLowerBound();
            BackPointer = backPointer;
        }
    }
}
