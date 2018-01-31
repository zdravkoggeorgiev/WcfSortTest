using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfSortTest.Utils
{
    /// <summary>
    /// Class with helper functions to perform Sort.
    /// </summary>
    public class SearchUtils
    {
        /// <summary>
        /// A recursive binary search to find the position where item should be inserted.
        /// Item should be IComparable
        /// </summary>
        /// <param name="list">List with input data</param>
        /// <param name="item">Item to find position </param>
        /// <param name="low">low range to search</param>
        /// <param name="high">high range to search</param>
        /// <returns>Position, to which item should be instert</returns>
        public static int BinarySearch<T>(T[] array, T item, int low, int high) where T : IComparable
        {
            if (high <= low)
                return (item.CompareTo(array[low]) > 0) ? (low + 1) : low;

            int mid = (low + high) / 2;

            if (item.CompareTo(array[mid]) == 0)
                return mid + 1;

            if (item.CompareTo(array[mid]) > 0)
                return BinarySearch(array, item, mid + 1, high);
            return BinarySearch(array, item, low, mid - 1);
        }

        /// <summary>
        /// Sorts a List with binary insertion sort. 
        /// Can skip part of List, if already sorted ( no checks made for skipped part)
        /// </summary>
        /// <param name="list">List to be sorted</param>
        /// <param name="start">Start position (default should be 0 )</param>
        public static void InsertionSort<T>(ref T[] array, int start) where T : IComparable
        {
            int loc, j;
            T selected;

            for (int i = start; i < array.Length; ++i)
            {
                j = i - 1;
                selected = array[i];

                // find location where selected should be inseretd
                loc = BinarySearch<T>(array, selected, 0, j);

                // Shift all elements after location
                while (j >= loc)
                {
                    array[j + 1] = array[j];
                    j--;
                }
                array[j + 1] = selected;
            }
        }
    }
}