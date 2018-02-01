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
    /// Collection store for multiple sorted arrays, to be used in multi-thread environment.
    /// <para> Arrays content is immutable ( not constrained yet ), thus reading is fast and without lockings </para>
    /// Adding new array makes only a short lock, for adding new array reference to list.
    /// Map of sorting is used , instead to sort real data. Map is held in Additional array, with indexes to real data.
    /// Each time read is need, a new copy of map is made, not to interfere with new additions to arrays.
    /// !!! CON: Map is a simple array, instead of B-Tree or Red-Black-Tree. 
    /// Will make it with more efficient storage, if time allows it
    /// </summary>
    public class ConcurentArrays : IDisposable
    {
        #region Private Fields

        // ConcurrentBag seems overhead, need to test. Our data is immutable and doesn't change
        private volatile List<string[]> _containersData = new List<string[]>();
        private volatile List<int> _containersLengths = new List<int>();

        // Sorted Map of items. Accessing data through this index guaranties sorted data.
        private volatile IntInt[] _sortedMap = new IntInt[0];
        private volatile int _itemsAllCount = 0;
        private volatile int _lastSortedContainer = -1; // Container 0 is not yet sorted
        private bool _isDisposed = false;

        // locking objects
        private object _lockAdd = new object();
        private object _lockMap = new object();

        #endregion

        #region Public Fields

        /// <inheritdoc />
        public int Length { get { return _itemsAllCount; } }

        /// <summary>
        /// <see cref="GetByIndexRaw()" />
        /// </summary>
        public string this[int index]
        {
            get
            {
                return GetBySingleIndex(index);
            }
        }

        /// <summary>
        /// Allow unsorted access to data to all containers as usual array, by IntInt index, e.g. concurentArr[currentIndex]
        /// </summary>
        /// <param name="index">IntInt index of item to retrieve </param>
        /// <returns>Returns string item for index position </returns>
        public string this[IntInt index]
        {
            get
            {
                return _containersData[index.ContainerIndex][index.ArrayIndex];
            }
        }

        #endregion

        #region Private/Protected Methods

        /// <summary>
        /// Generates new Sorted Map of data. Generation is made in a copy, not to interfere with readings. Usually used after new data additions.
        /// </summary>
        private void SortMapOfArrays()
        {
            if (_itemsAllCount > _sortedMap.Length && !_isDisposed)
            {
                lock (_lockMap)
                {
                    if (_itemsAllCount > _sortedMap.Length && !_isDisposed)
                    {
                        int _containersCount = _containersData.Count;
                        int _lastIndex = _sortedMap.Length;

                        IntInt[] arrayNewMap = new IntInt[_itemsAllCount];
                        Array.Copy(_sortedMap, arrayNewMap, _sortedMap.Length);

                        // initialize new items in Sort Map with initial unsorted positions of new items
                        for (int containerNum = _lastSortedContainer + 1; containerNum < _containersCount; containerNum++)
                        {
                            for (int arrayPos = 0; arrayPos < _containersLengths[containerNum]; arrayPos++)
                            {
                                arrayNewMap[_lastIndex] = new IntInt(containerNum, arrayPos);
                                _lastIndex++;
                            }
                        }

                        this.InsertionSortMap(arrayNewMap, _sortedMap.Length);

                        // Apply sorting as current Map
                        _sortedMap = arrayNewMap;
                        _lastSortedContainer = _containersCount - 1;
                    }
                }
            }
        }

        /// <summary>
        /// Duplicate current sorted map. Purpose is not to work with original one, to avoid locking/ access violations on sortings.
        /// </summary>
        /// <returns></returns>
        private IntInt[] GetCurrentMapCopy()
        {
            IntInt[] currentMap = new IntInt[_sortedMap.Length] ;
            lock (_lockMap)
            {
                Array.Copy(_sortedMap, currentMap, _sortedMap.Length);
            }
            return currentMap;
        }

        /// <summary>
        /// Allow access to data to all containers as usual array, e.g. concurentArr[5].
        /// WARNING !!! This is slow operation. Consider used this[IntInt index] instead.
        /// </summary>
        /// <param name="index">index of item to retrieve </param>
        /// <returns>Returns string item for index position </returns>
        protected string GetBySingleIndex(int index)
        {
            if (index < 0 || index > _itemsAllCount)
                throw new System.ArgumentOutOfRangeException("index parameter is out of range.");

            // Find to which container Index belongs
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
        /// <param name="arrayMap">Array with input data</param>
        /// <param name="item">Item to find position </param>
        /// <param name="low">low range to search</param>
        /// <param name="high">high range to search</param>
        /// <returns>Position, to which item should be instert</returns>
        private int BinarySearchMap(IntInt[] arrayMap, IntInt item, int low, int high)
        {
            if (high <= low)
                return (CompareContent(item, arrayMap[low]) > 0) ? (low + 1) : low;

            int mid = (low + high) / 2;

            if (CompareContent(item, arrayMap[mid]) == 0)
                return mid + 1;

            if (CompareContent(item, arrayMap[mid]) > 0)
                return BinarySearchMap(arrayMap, item, mid + 1, high);

            return BinarySearchMap(arrayMap, item, low, mid - 1);
        }

        /// <summary>
        /// Sorts an array with binary insertion sort. 
        /// Can skip part of List, if already sorted ( no checks made for skipped part)
        /// </summary>
        /// <param name="arrayMap">Array to be sorted</param>
        /// <param name="start">Start position (default should be 0 )</param>
        private void InsertionSortMap(IntInt[] arrayMap, int start)
        {
            int loc, j;
            IntInt selected;

            for (int i = start; i < arrayMap.Length; ++i)
            {
                j = i - 1;
                selected = arrayMap[i];

                // find location where selected should be inseretd
                loc = BinarySearchMap(arrayMap, selected, 0, j);

                // Shift all elements after location
                while (j >= loc)
                {
                    arrayMap[j + 1] = arrayMap[j];
                    j--;
                }
                arrayMap[j + 1] = selected;
            }
        }

        /// <summary>
        /// Comapre the content in unordered string array, based on index parameters.
        /// </summary>
        /// <param name="item1">First Item to be compared </param>
        /// <param name="item2">Second Item, to be compared</param>
        /// <returns></returns>
        private int CompareContent(IntInt item1, IntInt item2)
        {
            string a1 = _containersData[item1.ContainerIndex][item1.ArrayIndex];
            string a2 = _containersData[item2.ContainerIndex][item2.ArrayIndex];
            return a1.CompareTo(a2);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds new array of strings to the collection
        /// </summary>
        /// <param name="item"></param>
        public void Add(string[] item)
        {
            if (_isDisposed)
                return;

            lock (_lockAdd)
            {
                _containersData.Add(item);
                _containersLengths.Add(item.Length);
                _itemsAllCount += item.Length;
            }

            this.SortMapOfArrays();
        }

        /// <summary>
        /// Return ordered stream with current items
        /// </summary>
        /// <returns></returns>
        public Stream GetSortedItems()
        {
            MemoryStream resultStream = new MemoryStream();

            if (_sortedMap.Length != 0 && !_isDisposed)
            {
                IntInt[] currentSortedMap = GetCurrentMapCopy();
                try
                {
                    int offset = 0;
                    for (int i = 0; i < currentSortedMap.Length; i++)
                    {
                        byte[] currentLine = System.Text.Encoding.UTF8.GetBytes(
                            this[currentSortedMap[i]] + System.Environment.NewLine);
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
            _isDisposed = true;
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

    /// <summary>
    /// Helper storage class to keep adress to unsorted data, 2 integers.
    /// </summary>
    public struct IntInt
    {
        public int ContainerIndex;
        public int ArrayIndex;

        public IntInt(int containerIndex, int arrayIndex)
        {
            ContainerIndex = containerIndex;
            ArrayIndex = arrayIndex;
        }
    }
}