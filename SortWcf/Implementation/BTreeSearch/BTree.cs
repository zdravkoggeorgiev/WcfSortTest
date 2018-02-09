using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WcfSortTest.Utils;

namespace WcfSortTest.BTreeSearch
{
    // The Binary tree itself
    public class BTree : IDisposable
    {
        private BNode _root = null;
        private bool _isDisposed = false;
        private List<string[]> _container2d;

        public int Count { get; private set; }
        public BNode MostLeftNode { get; private set; }

        public BTree(List<string[]> container2d)
        {
            _container2d = container2d;
            Count = 0;
            MostLeftNode = null;
        }

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

                // Keep track of BNode with Miminal Value, for sake of reading all BTree sorted
                if (MostLeftNode == null || MostLeftNode.CompareTo(valueNew) > 0)
                    MostLeftNode = tree;

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

        public IntInt[] ReadSortedValues()
        {
            if (_root == null)
                return null;

            var sortedItems = ReadSorted(_root);

            // enumerate lazy IEnumerable.
            // TODO: Consider to avoid unnecessary cast
            var result = sortedItems.ToArray();

            return result;
        }

        private static IEnumerable<IntInt> ReadSorted(BNode startNode)
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
        /// Inserts new value ( address to string ) into BTree. Supports duplicates
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
        /// Find value in tree. Return a reference to the node, or null, if missing
        public BNode FindValue(IntInt value)
        {
            throw new NotImplementedException("later");
        }

        public void Dispose()
        {
            _isDisposed = true;
            Count = 0;
            DisposeTree(ref _root);
        }

        private void DisposeTree(ref BNode p)
        {
            if (p != null)
            {
                DisposeTree(ref p.Left);
                DisposeTree(ref p.Right);
                p = null;
            }
        }
    }
}
