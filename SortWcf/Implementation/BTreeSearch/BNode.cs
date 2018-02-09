using System;
using System.Collections.Generic;
using WcfSortTest.Utils;

namespace WcfSortTest.BTreeSearch
{
    /// <summary>
    /// Node for a tree, tied to List{string[]} data container, and IntInt Sorting Map
    /// </summary>
    public class BNode : IComparable<BNode>, IComparable<IntInt>
    {
        #region Fields

        private List<string[]> _container2d;

        public List<IntInt> Values { get; private set; } 
        public IntInt Value { get { return Values[0]; } }
        public int Count { get { return Values.Count; } }

        public BNode Left = null;
        public BNode Right = null;
        public BNode Parent;

        #endregion

        #region Constructors

        // Disable empty constructor
        private BNode() { }

        /// <summary>
        /// Creates new instance of BNode.
        /// </summary>
        /// <param name="container2d">Container with strings . </param>
        /// <param name="valueNew">New value ( address to real string data ) to be stored in Node </param>
        /// <param name="parent">Parent node. Null for no-parent (root) situation.</param>
        public BNode(List<string[]> container2d, IntInt valueNew, BNode parent)
        {
            _container2d = container2d;

            Values = new List<IntInt> { valueNew };
            Parent = parent;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add new Value ( address to real string data ). Uses when duplicate is found
        /// </summary>
        /// <param name="valueNew">new Value ( address to real string data ) to be added. </param>
        /// <returns>Returns current Node. </returns>
        public BNode AddValue(IntInt valueNew)
        {
            Values.Add(valueNew);
            return this;
        }

        /// <summary>
        /// Compare strings in data container List{string[]}. 
        /// </summary>
        /// <param name="other">Value ( address to real string data ) to be compared with stored in Node. </param>
        /// <returns>Result is same as usual string compare.</returns>
        public int CompareTo(IntInt other)
        {
            string a1 = _container2d[Value.ContainerIndex][Value.ArrayIndex];
            string a2 = _container2d[other.ContainerIndex][other.ArrayIndex];
            return a1.CompareTo(a2);
        }

        /// <summary>
        /// Compare strings in data container List{string[]}. 
        /// </summary>
        /// <param name="other">Value ( address to real string data ) to be compared with stored in Node. </param>
        /// <returns>Result is same as usual string compare.</returns>
        public int CompareTo(BNode other)
        {
            return CompareTo(other.Value);
        }

        #endregion
    }
}
