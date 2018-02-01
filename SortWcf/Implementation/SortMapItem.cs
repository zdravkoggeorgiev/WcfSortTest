using System;
using System.IO;

namespace WcfSortTest
{
    /// <summary>
    /// Maping  implementation of ISortingItem, using binary search and sort, to order data.
    /// </summary>
    public class SortMapItem : ISortingItem
    {
        #region Private Fields

        // Used to unique distinguish different instances.
        private Guid _guid = Guid.NewGuid();
        private ConcurentArrays _concurentArrays = new ConcurentArrays();

        #endregion

        #region Public Fields

        /// <inheritdoc />
        public Guid UID { get { return _guid; } }

        /// <inheritdoc />
        public void AddItems(string[] newItems)
        {
            _concurentArrays.Add(newItems);
        }

        /// <inheritdoc />
        public Stream GetSortedItems()
        {
            return _concurentArrays.GetSortedItems();
        }

        /// <inheritdoc />
        public int CountLines()
        {
            return _concurentArrays.Length;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _concurentArrays.Dispose();
        }

        #endregion
    }
}