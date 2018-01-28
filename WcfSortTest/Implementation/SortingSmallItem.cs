using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using WcfSortTest.Utils;

namespace WcfSortTest
{
    public class SortingSmallItem : ISortingItem
    {
        #region Private Fields

        private Guid _guid = Guid.NewGuid();
        private bool _isSorting = false;
        private bool _isSorted = true;
        private bool _isDisposing = false;

        private object _sortedLock = new Object();
        private object _queueLock = new Object();

        private string[] _sortedItems = new string[0];
        private string[] _queueItems = null;

        #endregion

        #region Public Fields

        public Guid UID { get { return _guid; } }

        #endregion

        #region Private Methods

        private void SortItems()
        {
            if (_isSorting || _isDisposing)
                return;

            lock (_sortedLock)
            {
                if (_isSorted && _queueItems == null)
                    return;

                _isSorting = true;
                try
                {
                    if (_queueItems?.Length > 0)
                    {
                        lock (_queueLock)
                        {
                            ArrayUtils.Append(ref _sortedItems,_queueItems);
                            _queueItems = null;
                        }
                    }

                    Array.Sort(_sortedItems);
                }
                finally
                {
                    // Be sure to turn the flag off on exit
                    _isSorting = false;
                }
            }
        }

        #endregion

        #region Public Methods

        public void AddItems(string[] newItems)
        {
            if (newItems == null || newItems.Length == 0 || _isDisposing)
                return;

            // if sorting is in progress, it's impossible to add new items to the main container(_sortedItems)
            // and we will add new items into the queue container (_queueItems)
            if (_isSorting)
            {
                lock (_queueLock)
                {
                    ArrayUtils.Append(ref _queueItems, newItems);
                }
            }
            else
            {
                lock (_sortedLock)
                {
                    ArrayUtils.Append(ref _sortedItems, newItems);
                    Array.Sort(_sortedItems);
                }
            }

        }

        public int CountLines()
        {
            return _sortedItems.Length + (_queueItems?.Length) ?? 0;
        }

        public void Dispose()
        {
            _isDisposing = true;
            _queueItems = null;
            _sortedItems = null;
        }

        public Stream GetSortedItems()
        {
            MemoryStream resultStream = null;

            SortItems();

            if (_sortedItems?.Length > 0 && !_isDisposing)
            {
                resultStream = new MemoryStream();
                try
                {
                    // ZDGV Measure array size, to resize MemoryStream at Once ?
                    int offset = 0;
                    foreach (var item in _sortedItems)
                    {
                        byte[] existingData = System.Text.Encoding.UTF8.GetBytes(item + System.Environment.NewLine);
                        resultStream.Write(existingData, 0, existingData.Length);
                        offset += existingData.Length;
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

        #endregion
    }
}