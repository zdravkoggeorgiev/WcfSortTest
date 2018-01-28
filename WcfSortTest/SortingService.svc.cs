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
            ISortingItem sortingItem = new SortingSmallItem();

            _store.TryAdd(sortingItem.UID, sortingItem);
            return sortingItem.UID;
        }

        /// <inheritdoc />
        public void PutStreamData(Guid streamGuid, string[] text)
        {
            if (_store.TryGetValue(streamGuid, out ISortingItem sortingItem))
            {
                sortingItem.AddItems(text);
            }
        }

        /// <inheritdoc />
        public Stream GetSortedStream(Guid streamGuid)
        {
            if (_store.TryGetValue(streamGuid, out ISortingItem sortingItem))
            {
                return sortingItem.GetSortedItems();
            }
            else
            {
                return new MemoryStream();
            }
        }

        /// <inheritdoc />
        public void EndStream(Guid streamGuid)
        {
            if (_store.TryRemove(streamGuid, out ISortingItem sortingItem))
            {
                sortingItem.Dispose();
                sortingItem = null;
            }
        }

        #endregion
    }
}
