using System;
using System.Collections.Generic;

namespace WcfSortTest.Utils
{
    /// <summary>
    /// Class with helper functions to perform Sort.
    /// </summary>
    public static class SearchUtils
    {
        #region Public Methods


        /// <summary>
        /// Sorts a List with binary insertion sort. 
        /// Can skip part of List, if already sorted ( no checks made for skipped part)
        /// </summary>
        /// <param name="list">List to be sorted</param>
        /// <param name="array">array with input data</param>
        /// <param name="start">Start position (first time should be 0 )</param>
        public static void InsertionSort<T>(T[] array, int start) where T : IComparable
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

        /// <summary>
        /// Sorts an array with binary insertion sort. 
        /// Can skip part of List, if already sorted ( no checks made for skipped part)
        /// </summary>
        /// <param name="container">2D container with data ( List with arrays with strings ), to be compared</param>
        /// <param name="arrayMap">Sorted Map of data</param>
        /// <param name="start">Start position (first time should be 0 )</param>
        public static void InsertionSortMap(IList<string[]> container, IntInt[] arrayMap, int start)
        {
            int loc, j;
            IntInt selected;

            for (int i = start; i < arrayMap.Length; ++i)
            {
                j = i - 1;
                selected = arrayMap[i];

                // find location where selected should be inseretd
                loc = BinarySearchMap(container, arrayMap, selected, 0, j);

                // Shift all elements after location
                while (j >= loc)
                {
                    arrayMap[j + 1] = arrayMap[j];
                    j--;
                }
                arrayMap[j + 1] = selected;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// A recursive binary search to find the position where item should be inserted.
        /// Item should be IComparable
        /// </summary>
        /// <param name="array">array with input data</param>
        /// <param name="item">Item to find position </param>
        /// <param name="low">low range to search</param>
        /// <param name="high">high range to search</param>
        /// <returns>Position, to which item should be instert</returns>
        private static int BinarySearch<T>(T[] array, T item, int low, int high) where T : IComparable
        {
            if (high <= low)
                return (item.CompareTo(array[low]) > 0) ? (low + 1) : low;

            int mid = (low + high) / 2;

            switch (item.CompareTo(array[mid]))
            {
                case -1:
                    return BinarySearch(array, item, low, mid - 1);
                case 0:
                    return mid + 1;
                case 1:
                    return BinarySearch(array, item, mid + 1, high);
                default:
                    throw new NotImplementedException("String compare returned unexpected result.");
            }
        }

        /// <summary>
        /// Same as <see cref="BinarySearch{T}"/>, but allows compare to be done on another array of data
        /// </summary>
        /// <param name="container">2D container with data ( List with arrays with strings ), to be compared</param>
        /// <param name="arrayMap">Array with Map to containers, with (partially) sorted data</param>
        /// <param name="item">Item to find position </param>
        /// <param name="low">low range to search</param>
        /// <param name="high">high range to search</param>
        /// <returns>Position, to which item should be instert</returns>
        private static int BinarySearchMap(IList<string[]> container, IntInt[] arrayMap, IntInt item, int low, int high)
        {
            if (high <= low)
                return (CompareContent(container, item, arrayMap[low]) > 0) ? (low + 1) : low;

            int mid = (low + high) / 2;

            switch (CompareContent(container, item, arrayMap[mid]))
            {
                case -1:
                    return BinarySearchMap(container, arrayMap, item, low, mid - 1);
                case 0:
                    return mid + 1;
                case 1:
                    return BinarySearchMap(container, arrayMap, item, mid + 1, high);
                default:
                    throw new NotImplementedException("String compare returned unexpected result.");
            }
        }

        /// <summary>
        /// Comapre the content in unordered container of string arrays, based on two int index parameters.
        /// </summary>
        /// <param name="container">2D container with data ( List with arrays with strings ) </param>
        /// <param name="item1">First Item to be compared </param>
        /// <param name="item2">Second Item, to be compared</param>
        /// <returns></returns>
        private static int CompareContent(IList<string[]> container, IntInt item1, IntInt item2)
        {
            string a1 = container[item1.ContainerIndex][item1.ArrayIndex];
            string a2 = container[item2.ContainerIndex][item2.ArrayIndex];
            return a1.CompareTo(a2);
        }

        #endregion
    }
}