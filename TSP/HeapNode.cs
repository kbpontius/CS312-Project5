namespace NetworkRouting
{
    class HeapNode
    {
        public double pathCost = double.MaxValue;
        public int LOOKUPINDEX;
        public int backPointer;

        public HeapNode(double pathCost, int lookupIndex, int backPointer)
        {
            this.LOOKUPINDEX = lookupIndex;
            this.pathCost = pathCost;
            this.backPointer = backPointer;
        }
    }
}
