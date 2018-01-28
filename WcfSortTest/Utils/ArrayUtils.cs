using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfSortTest.Utils
{
    /// <summary>
    /// Class with Helper functions to extend base Array
    /// </summary>
    public static class ArrayUtils
    {
        /// <summary>
        /// Appends an array to end of another, Concat.
        /// </summary>
        /// <typeparam name="T">Any type of IComparable ( int, string, any classes, etc.) </typeparam>
        /// <param name="arrayDest">Destination array </param>
        /// <param name="arrayToAppend">Array to be append </param>
        public static void Append<T>(ref T[] arrayDest, T[] arrayToAppend) where T : IComparable
        {
            if (arrayToAppend?.Length > 0)
            {
                if(arrayDest != null)
                {
                    int arrayOldLength = arrayDest.Length;

                    Array.Resize(ref arrayDest, arrayOldLength + arrayToAppend.Length);
                    arrayToAppend.CopyTo(arrayDest, arrayOldLength);
                }
                else
                {
                    arrayDest = (T[])arrayToAppend.Clone();
                }
            }
        }
    }
}