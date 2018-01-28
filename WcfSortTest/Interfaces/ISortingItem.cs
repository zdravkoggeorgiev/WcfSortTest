using System;
using System.IO;

namespace WcfSortTest
{
    public interface ISortingItem : IDisposable
    {
        Guid UID { get; }

        void AddItems(string[] newItems);

        Stream GetSortedItems();

        int CountLines();
    }
}