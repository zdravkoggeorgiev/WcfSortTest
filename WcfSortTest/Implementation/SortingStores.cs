using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WcfSortTest
{
    public class SortingStores
    {
        private static ConcurrentDictionary<Guid, ISortingItem> _store = new ConcurrentDictionary<Guid, ISortingItem>();

        public Guid BeginStream()
        {
            ISortingItem sortingItem = new SortingSmallItem();

            _store.TryAdd(sortingItem.UID, sortingItem);
            return sortingItem.UID;
        }

        public void PutStreamData(Guid streamGuid, string[] text)
        {
            if (_store.TryGetValue(streamGuid, out ISortingItem sortingItem))
            {
                sortingItem.AddItems(text);
            }
        }

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

        public void EndStream(Guid streamGuid)
        {
            if (_store.TryRemove(streamGuid, out ISortingItem sortingItem))
            {
                sortingItem.Dispose();
                sortingItem = null;
            }
        }
    }
}