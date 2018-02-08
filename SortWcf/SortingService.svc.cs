using System;
using System.Collections.Concurrent;
using System.IO;

namespace WcfSortTest
{
    /// <inheritdoc />
    public class SortingService : ISortingService
    {
        /// <summary>
        /// Store for all session, distinguished by unique Guid. Bears in mind multithreading of WCF Service.
        /// To be implemented - on bigger data, use not In-Memory (SortingSmallItem), but another class, which supports File System store
        /// </summary>
        private static ConcurrentDictionary<Guid, ISortingItem> _store = new ConcurrentDictionary<Guid, ISortingItem>();

        #region Public Methods

        /// <inheritdoc />
        public Guid BeginStream()
        {
            try
            {
                ISortingItem sortingItem = new SortMapItem();

                _store.TryAdd(sortingItem.UID, sortingItem);
                return sortingItem.UID;
            }
            catch (Exception)
            {
                // Do some logging here. 
                // Consider decorate result, to send error message to the client
                return Guid.Empty;
            }
        }

        /// <inheritdoc />
        public void PutStreamData(Guid streamGuid, string[] text)
        {
            try
            {
                if (_store.TryGetValue(streamGuid, out ISortingItem sortingItem))
                {
                    sortingItem.AddItems(text);
                }
            }
            catch (Exception)
            {
                // Do some logging here. 
                // Consider decorate result, to send error message to the client
            }
        }

        /// <inheritdoc />
        public Stream GetSortedStream(Guid streamGuid)
        {
            try
            {
                if (_store.TryGetValue(streamGuid, out ISortingItem sortingItem))
                {
                    return sortingItem.GetSortedItems();
                }
            }
            catch (Exception)
            {
                // Do some logging here. 
                // Consider decorate result, to send error message to the client
            }
            return new MemoryStream();
        }

        /// <inheritdoc />
        public void EndStream(Guid streamGuid)
        {
            try
            {
                if (_store.TryRemove(streamGuid, out ISortingItem sortingItem))
                {
                    sortingItem.Dispose();
                    sortingItem = null;
                }
            }
            catch (Exception)
            {
                // Do some logging here. 
                // Consider decorate result, to send error message to the client
            }
        }

        #endregion
    }
}
