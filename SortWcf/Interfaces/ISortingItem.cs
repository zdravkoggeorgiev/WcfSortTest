using System;
using System.IO;

namespace WcfSortTest
{
    /// <summary>
    /// Interface to allow storage, sorting and retrieving arrays of string, in multithread environment.
    /// </summary>
    public interface ISortingItem : IDisposable
    {
        /// <summary>
        /// Returns unique ID of a Sorting Item/Series.
        /// </summary>
        Guid UID { get; }

        /// <summary>
        /// Add new items to be appended and sorted to current items. Can be call multiple times.
        /// </summary>
        /// <param name="newItems">Array with strings to be append </param>
        void AddItems(string[] newItems);

        /// <summary>
        /// Get currently append and sorted items. Can be call multiple times.
        /// </summary>
        /// <returns></returns>
        Stream GetSortedItems();

        /// <summary>
        /// Gives count of currently sorted items. Used to recognize too big array ( and too much memory usage ) and to switch to another sorting schema 
        /// ( using file instead of memory )
        /// </summary>
        /// <returns></returns>
        int CountLines();
    }
}