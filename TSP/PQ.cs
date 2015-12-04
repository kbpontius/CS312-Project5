using System;
using System.Collections.Generic;

namespace TSP
{
    internal class MinHeap<T> where T : IComparable<T>
    {
        private readonly List<T> array = new List<T>();

        public int Count
        {
            get { return array.Count; }
        }

        public void Add(T element)
        {
            array.Add(element);
            var c = array.Count - 1;
            while (c > 0 && array[c].CompareTo(array[c/2]) == -1)
            {
                var tmp = array[c];
                array[c] = array[c/2];
                array[c/2] = tmp;
                c = c/2;
            }
        }

        public T RemoveMin()
        {
            var ret = array[0];
            array[0] = array[array.Count - 1];
            array.RemoveAt(array.Count - 1);

            var c = 0;
            while (c < array.Count)
            {
                var min = c;
                if (2*c + 1 < array.Count && array[2*c + 1].CompareTo(array[min]) == -1)
                    min = 2*c + 1;
                if (2*c + 2 < array.Count && array[2*c + 2].CompareTo(array[min]) == -1)
                    min = 2*c + 2;

                if (min == c)
                    break;
                var tmp = array[c];
                array[c] = array[min];
                array[min] = tmp;
                c = min;
            }

            return ret;
        }

        public T Peek()
        {
            return array[0];
        }
    }

    internal class PriorityQueue<T>
    {
        private readonly MinHeap<Node> minHeap = new MinHeap<Node>();

        public int Count
        {
            get { return minHeap.Count; }
        }

        public void Add(int priority, T element)
        {
            minHeap.Add(new Node {Priority = priority, O = element});
        }

        public T RemoveMin()
        {
            return minHeap.RemoveMin().O;
        }

        public T Peek()
        {
            return minHeap.Peek().O;
        }

        internal class Node : IComparable<Node>
        {
            public T O;
            public int Priority;

            public int CompareTo(Node other)
            {
                return Priority.CompareTo(other.Priority);
            }
        }
    }
}