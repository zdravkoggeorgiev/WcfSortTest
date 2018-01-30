using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WcfSortTest
{
    public class SortMapItem : ISortingItem
    {
        public Guid UID => throw new NotImplementedException();

        public void AddItems(string[] newItems)
        {
            throw new NotImplementedException();
        }

        public int CountLines()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Stream GetSortedItems()
        {
            throw new NotImplementedException();
        }
    }
}