using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WcfSortTest.Utils;

namespace WcfSortTest
{
    /// <summary>
    /// Collection store for multiple arrays, to be used in multi-thread environment.
    /// Arrays content is immutable ( not constrained yet ), thus reading is fast and without lockings
    /// Adding new array makes only a short lock, for adding array reference to list.
    /// For map of sorting is used additional array, which helds sorted map with indexes to real data.
    /// Each time read is need, a new copy of map is made, not to interfere with new additions to arrays.
    /// CON: storage is simple aray, instead of B-Tree, Red-Black-Tree. 
    /// Will make it with more efficient storage, if time allows it
    /// </summary>
    public class ConcurentArrays : IDisposable
    {
        #region Private Fields

        // ConcurrentBag seems overhead, need to test our data is immutable and doesn't change
        private volatile List<string[]> _containersData = new List<string[]>();
        private volatile List<int> _containersLengths = new List<int>();
        private volatile int[] _sortedMap = new int[0];
        private int _itemsAllCount = 0;

        private object _lockAdd = new object();
        private object _lockMap = new object();
        private bool _isDisposing = false;

        #endregion

        #region Public Fields

        /// <inheritdoc />
        public int Length { get { return _itemsAllCount; } }

        /// <summary>
        /// Allow access to data to all containers as usual array, e.g. concurentArr[5]
        /// </summary>
        /// <param name="index">index of item to retrieve </param>
        /// <returns>Returns string item for index position </returns>
        public string this[int index]
        {
            get
            {
                return GetByIndexRaw(index);
            }
        }

        #endregion

        #region Private/Protected Methods

        private void SortMapOfArrays()
        {
            if (_itemsAllCount > _sortedMap.Length)
            {
                lock (_lockMap)
                {
                    if (_itemsAllCount > _sortedMap.Length)
                    {
                        int[] arrayNewMap = new int[_itemsAllCount];
                        if (_sortedMap.Length > 0)
                        {
                            Array.Copy(_sortedMap, arrayNewMap, _sortedMap.Length);
                        }

                        // init new items which will be sorted
                        for (int i = _sortedMap.Length; i < arrayNewMap.Length; i++)
                        {
                            arrayNewMap[i] = i;
                        }

                        this.InsertionSortMap(arrayNewMap, _sortedMap.Length);
                        _sortedMap = arrayNewMap;
                    }
                }
            }
        }

        private int[] GetCurrentMapCopy()
        {
            int[] currentMap;
            lock (_lockMap)
            {
                currentMap = (int[])_sortedMap.Clone();
            }
            return currentMap;
        }

        protected string GetByIndexRaw(int index)
        {
            if (index < 0 || index > _itemsAllCount)
                throw new System.ArgumentOutOfRangeException("index parameter is out of range.");

            // IMPORTANT !!! ZDGV speed-up reading by 2D array instead. 
            // Reading property is too slow
            // But best is Tree for a container, so leave this optimization for later      
            int curContainer = 0;
            while (index > _containersLengths[curContainer] - 1)
            {
                index -= _containersLengths[curContainer];
                curContainer++;
            }
            return _containersData[curContainer][index];
        }

        /// <summary>
        /// Same as <see cref="BinarySearch{T}"/>, but allows compare to be done on another array of data
        /// </summary>
        /// <param name="list">List with input data</param>
        /// <param name="item">Item to find position </param>
        /// <param name="low">low range to search</param>
        /// <param name="high">high range to search</param>
        /// <returns>Position, to which item should be instert</returns>
        private int BinarySearchMap(int[] arrayMap, int item, int low, int high)
        {
            if (high <= low)
                return (CompareContent(arrayMap, item, low) > 0) ? (low + 1) : low;

            int mid = (low + high) / 2;

            if (CompareContent(arrayMap, item, mid) == 0)
                return mid + 1;

            if (CompareContent(arrayMap, item, mid) > 0)
                return BinarySearchMap(arrayMap, item, mid + 1, high);

            return BinarySearchMap(arrayMap, item, low, mid - 1);
        }

        /// <summary>
        /// Sorts a List with binary insertion sort. 
        /// Can skip part of List, if already sorted ( no checks made for skipped part)
        /// </summary>
        /// <param name="list">List to be sorted</param>
        /// <param name="start">Start position (default should be 0 )</param>
        private void InsertionSortMap(int[] arrayMap, int start)
        {
            int loc, j;
            int selected;

            for (int i = start; i < arrayMap.Length; ++i)
            {
                j = i - 1;
                selected = arrayMap[i];

                // find location where selected should be inseretd
                loc = BinarySearchMap(arrayMap, i, 0, j);

                // Shift all elements after location
                while (j >= loc)
                {
                    arrayMap[j + 1] = arrayMap[j];
                    j--;
                }
                arrayMap[j + 1] = selected;
            }
        }

        private int CompareContent(int[] arrayMap, int mapIndex1, int mapIndex2)
        {
            var a1 = GetByIndexRaw(arrayMap[mapIndex1]);
            var a2 = GetByIndexRaw(arrayMap[mapIndex2]);
            return a1.CompareTo(a2);
        }

        #endregion

        #region Public Methods

        public void Add(string[] item)
        {
            lock (_lockAdd)
            {
                _containersData.Add(item);
                _containersLengths.Add(item.Length);
                _itemsAllCount += item.Length;
            }

            this.SortMapOfArrays();
        }

        public Stream GetSortedItems()
        {
            MemoryStream resultStream = new MemoryStream();

            if (_sortedMap.Length != 0 && !_isDisposing)
            {
                int[] currentSortedMap = GetCurrentMapCopy();
        
                try
                {
                    int offset = 0;
                    for (int i = 0; i < currentSortedMap.Length; i++)
                    {
                        byte[] currentLine = System.Text.Encoding.UTF8.GetBytes(
                            GetByIndexRaw(currentSortedMap[i]) + System.Environment.NewLine);
                        resultStream.Write(currentLine, 0, currentLine.Length);
                        offset += currentLine.Length;
                    }
                    resultStream.Position = 0;
                }
                catch (Exception)
                {
                    resultStream.Dispose();
                    throw;
                }
            }
            return resultStream;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _isDisposing = true;
            lock (_lockMap)
            {
                _sortedMap = null;
            }
            lock (_lockMap)
            {
                _containersData = null;
                _containersLengths = null;
                _itemsAllCount = 0;
            }
        }

        #endregion
    }
}