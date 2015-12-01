namespace TSP
{
    class HeapNode
    {
        public Matrix matrix;
        public double ReductionCost = double.MaxValue;
        public int LOOKUPINDEX;
        public int backPointer;

        public HeapNode(Matrix matrix, double reductionCost, int lookupIndex, int backPointer)
        {
            this.matrix = matrix;
            this.LOOKUPINDEX = lookupIndex;
            this.ReductionCost = reductionCost;
            this.backPointer = backPointer;
        }
    }
}
