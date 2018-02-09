using System;
using System.Collections.Generic;
using WcfSortTest.Utils;

namespace WcfSortTest.BTreeSearch
{
    // Define tree nodes
    public class BNode : IComparable<BNode>, IComparable<IntInt>
    {
        private List<string[]> _container2d;

        public List<IntInt> Values { get; private set; } 
        public IntInt Value { get { return Values[0]; } }
        public int Count { get { return Values.Count; } }

        public BNode Left = null;
        public BNode Right = null;
        public BNode Parent;

        // Disable empty constructor
        private BNode() { }

        public BNode(List<string[]> container2d, IntInt valueNew, BNode parent)
        {
            _container2d = container2d;

            Values = new List<IntInt> { valueNew };
            Parent = parent;
        }

        public BNode AddValue(IntInt valueNew)
        {
            Values.Add(valueNew);
            return this;
        }

        public int CompareTo(IntInt other)
        {
            string a1 = _container2d[Value.ContainerIndex][Value.ArrayIndex];
            string a2 = _container2d[other.ContainerIndex][other.ArrayIndex];
            return a1.CompareTo(a2);
        }

        public int CompareTo(BNode other)
        {
            return CompareTo(other.Value);
        }
    }
}
