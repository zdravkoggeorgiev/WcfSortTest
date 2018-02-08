using System;
using System.Collections.Generic;
using System.IO;

namespace WcfSortTest.Utils
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

        #region Public Methods

        /// <summary>
        /// Adds new array of strings to the collection
        /// </summary>
        /// <param name="item"></param>
        public void Add(string[] item)
        {
            // If is Disposed, new additions are not allowed
            if (_isDisposed)
                throw new ObjectDisposedException("ConcurentArrays is already disposed.");

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
            // If is Disposed, operations over arrays are not allowed
            if (_isDisposed)
                throw new ObjectDisposedException("ConcurentArrays is already disposed.");

            MemoryStream resultStream = new MemoryStream();

            if (_sortedMap.Length != 0)
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

        #region Private/Protected Methods

        /// <summary>
        /// Generates new Sorted Map of data. Generation is made in a copy, not to interfere with readings. Usually used after new data additions.
        /// </summary>
        private void SortMapOfArrays()
        {
            // If is Disposed, operations over arrays are not allowed
            if (_isDisposed)
                throw new ObjectDisposedException("ConcurentArrays is already disposed.");

            if (_itemsAllCount > _sortedMap.Length)
            {
                lock (_lockMap)
                {
                    if (_itemsAllCount > _sortedMap.Length)
                    {
                        // If is Disposed, operations over arrays are not allowed
                        if (_isDisposed)
                            throw new ObjectDisposedException("ConcurentArrays is already disposed.");

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

                        SearchUtils.InsertionSortMap(_containersData, arrayNewMap, _sortedMap.Length);

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

        #endregion

    }
}