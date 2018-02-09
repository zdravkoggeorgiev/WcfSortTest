using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WcfSortTest.Utils;

namespace WcfSortTest.BTreeSearch
{
    /// <summary>
    /// A binart Tree implementation. Storing address to strings in List{string[]} container.
    /// <para> In Nodes are stored addresses (IntInt) to data container. </para>
    /// <para> Inserts are always sorted, comparing strings in container. </para>
    /// </summary>
    public class BTree : IDisposable
    {
        #region Fields

        private BNode _root = null;
        private bool _isDisposed = false;
        private List<string[]> _container2d;

        public int Count { get; private set; }

        #endregion

        #region Constructors

        // Disable empty constructor
        private BTree() { }

        /// <summary>
        /// Creates new instance of BTree.
        /// </summary>
        /// <param name="container2d">Container with strings data. </param>
        public BTree(List<string[]> container2d)
        {
            _container2d = container2d;
            Count = 0;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Inserts new value ( address to string ) into BTree. Supports duplicates.
        /// </summary>
        public BNode Insert(IntInt valueNew)
        {
            // TODO: Add Exception and exception handling later
            if (_isDisposed)
                return null;

            Count++;
            return InsertInternal(ref _root, _root, valueNew);
        }

        /// <summary>
        /// Read Addressed to data, sorting strings asceding.
        /// </summary>
        /// <returns></returns>
        public IntInt[] ReadSortedValues()
        {
            // TODO: Add Exception and exception handling later, on Disposed use
            if (_root == null || _isDisposed)
                return null;

            var sortedItems = ReadSorted(_root);

            // enumerate lazy IEnumerable.
            // TODO: Consider to avoid unnecessary cast
            var result = sortedItems.ToArray();

            return result;
        }

        /// <summary>
        /// Disposes Tree, and disables further use.
        /// </summary>
        public void Dispose()
        {
            _isDisposed = true;
            Count = 0;
            DisposeTree(ref _root);
        }
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Recursively inserts new value (address to string ) into the Tree.
        /// </summary>
        /// <param name="tree">Tree to store node</param>
        /// <param name="valueNew">New value (address to string ) to insert</param>
        private BNode InsertInternal(ref BNode tree, BNode parent, IntInt valueNew)
        {
            if (tree == null)
            {
                tree = new BNode(_container2d, valueNew, parent);
                return tree;
            }
            else
            {
                switch (tree.CompareTo(valueNew))
                {
                    case 1:
                        return InsertInternal(ref tree.Left, tree, valueNew);
                    case 0:
                        return tree.AddValue(valueNew);
                    case -1:
                        return InsertInternal(ref tree.Right, tree, valueNew);
                    default:
                        throw new NotImplementedException("String compare returned unexpected result.");
                }
            }
        }

        /// <summary>
        /// Reads BTree in sorted order.
        /// </summary>
        /// <param name="startNode">node to start, usually root</param>
        private IEnumerable<IntInt> ReadSorted(BNode startNode)
        {
            // Firest return left childs,
            if (startNode.Left != null)
                foreach (IntInt item in ReadSorted(startNode.Left))
                {
                    yield return item;
                }

            // second return self values
            foreach (IntInt item in startNode.Values)
            {
                yield return item;
            }

            // Third return right childs
            if (startNode.Right != null)
                foreach (IntInt item in ReadSorted(startNode.Right))
                {
                    yield return item;
                }
        }

        /// <summary>
        /// Nullify all nodes.
        /// </summary>
        /// <param name="p">Start Node, to nullify all his children. </param>
        private void DisposeTree(ref BNode p)
        {
            if (p != null)
            {
                DisposeTree(ref p.Left);
                DisposeTree(ref p.Right);
                p = null;
            }
        }

        #endregion

    }
}
