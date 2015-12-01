using System.Collections.Generic;

namespace NetworkRouting
{
    class PQ
    {
        private List<HeapNode> heap = new List<HeapNode>();
        private List<LookupNode> lookupTable = new List<LookupNode>();

        // MARK: INITIALIZER
        public PQ(int lookupTableSize)
        {
            for (int i = 0; i < lookupTableSize; i++)
            {
                lookupTable.Add(new LookupNode());   
            }
        }

        // MARK: PRIMARY METHODS
        public void Add(int lookupIndex, double pathCost, int backPointer)
        {
            lookupTable[lookupIndex].heapIndex = heap.Count;
            heap.Add(new HeapNode(pathCost, lookupIndex, backPointer));

            BubbleUp(heap.Count - 1);
        }

        public int PopMin()
        {
            HeapNode minNode = heap[0];
            heap[0] = heap[heap.Count - 1];
            lookupTable[heap[0].LOOKUPINDEX].heapIndex = 0;
            lookupTable[heap[0].LOOKUPINDEX].pathCost = minNode.pathCost;

            heap.RemoveAt(heap.Count - 1);
            lookupTable[minNode.LOOKUPINDEX].backPointer = minNode.backPointer;

            BubbleDown(0);

            return minNode.LOOKUPINDEX;
        }

        public void DecreaseKey(int lookupTableIndex, double newPathValue, int newBackPointer)
        {
            int heapIndex = lookupTable[lookupTableIndex].heapIndex;
            heap[heapIndex].pathCost = newPathValue;
            heap[lookupTable[lookupTableIndex].heapIndex].backPointer = newBackPointer;

            BubbleUp(heapIndex);
        }

        public bool IsEmpty()
        {
            return heap.Count <= 0;
        }

        public double GetPathCost(int lookupIndex)
        {
            if (NodeIsAdded(lookupIndex))
            {
                return heap[lookupTable[lookupIndex].heapIndex].pathCost;
            }

            return -1;
        }

        public double GetFinalPathCost(int lookupIndex)
        {
            if (NodeIsAdded(lookupIndex))
            {
                return lookupTable[lookupIndex].pathCost;
            }

            return -1;
        }

        public int GetBackPointer(int lookupIndex)
        {
            return lookupTable[lookupIndex].backPointer;
        }

        public bool NodeIsAdded(int lookupIndex)
        {
            return lookupTable[lookupIndex].heapIndex != -1;
        }

        public bool NodeIsVisited(int lookupIndex)
        {
            return lookupTable[lookupIndex].heapIndex == 0 && heap[0].LOOKUPINDEX != lookupIndex;
        }

        // MARK: HELPER METHODS
        private void BubbleUp(int index)
        {
            int currentIndex = index;
            double currentValue = double.MinValue;
            double parentValue = double.MaxValue;

            while (currentIndex != 0 && parentValue > currentValue)
            {
                currentValue = heap[currentIndex].pathCost;
                parentValue = GetParentValue(currentIndex);

                if (currentValue < parentValue)
                {
                    SwapIndices(currentIndex, GetParentIndex(currentIndex));
                    currentIndex = GetParentIndex(currentIndex);
                }
            }
        }

        private void BubbleDown(int index)
        {
            int currentIndex = index;
            int minChildIndex = -1;
            double currentValue = double.MaxValue;
            double minChildValue = double.MinValue;

            while (HasLeftChild(currentIndex) && minChildValue < currentValue)
            {
                currentValue = heap[currentIndex].pathCost;
                minChildIndex = GetMinChildIndex(currentIndex);
                minChildValue = heap[minChildIndex].pathCost;

                if (currentValue > minChildValue)
                {
                    SwapIndices(currentIndex, minChildIndex);
                    currentIndex = minChildIndex;
                }
            }
        }

        private void SwapIndices(int i1, int i2)
        {
            // Swap lookupTable values.
            lookupTable[heap[i1].LOOKUPINDEX].heapIndex = i2;
            lookupTable[heap[i2].LOOKUPINDEX].heapIndex = i1;

            HeapNode tempNode = heap[i1];

            // Perform Node swap.
            heap[i1] = heap[i2];
            heap[i2] = tempNode;
        }

        public bool AllPathsIsFinished()
        {
            return heap[0].pathCost == double.MaxValue;
        }

        // MARK: INDEX GETTER METHODS
        private int GetParentIndex(int index)
        {
            return (index - 1) / 2;
        }

        private int GetLeftChildIndex(int index)
        {
            return (2 * index) + 1;
        }

        private int GetRightChildIndex(int index)
        {
            return (2 * index) + 2;
        }

        private bool HasLeftChild(int index)
        {
            return GetLeftChildIndex(index) <= heap.Count - 1;
        }

        private bool HasRightChild(int index)
        {
            return GetRightChildIndex(index) <= heap.Count - 1;
        }

        // MARK: VALUE METHODS
        private int GetMinChildIndex(int index)
        {
            double leftChildValue = double.MaxValue;
            double rightChildValue = double.MaxValue;

            if (HasLeftChild(index))
            {
                leftChildValue = GetLeftChildValue(index);
            }

            if (HasRightChild(index))
            {
                rightChildValue = GetRightChildValue(index);
            }
            else
            {
                return GetLeftChildIndex(index);
            }

            if (leftChildValue < rightChildValue)
            {
                return GetLeftChildIndex(index);
            }
            else
            {
                return GetRightChildIndex(index);
            }
        }

        private double GetParentValue(int index)
        {
            return heap[GetParentIndex(index)].pathCost;
        }

        private double GetLeftChildValue(int index)
        {
            return heap[GetLeftChildIndex(index)].pathCost;
        }

        private double GetRightChildValue(int index)
        {
            return heap[GetRightChildIndex(index)].pathCost;
        }
    }
}
