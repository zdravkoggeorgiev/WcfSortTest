using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfSortTest.Utils
{
    public static class ArrayUtils
    {
        public static void Append<T>(ref T[] array, T[] arrayToAppend) where T : IComparable
        {
            if (arrayToAppend?.Length > 0)
            {
                if(array != null)
                {
                    int arrayOldLength = array.Length;

                    Array.Resize(ref array, arrayOldLength + arrayToAppend.Length);
                    arrayToAppend.CopyTo(array, arrayOldLength);
                }
                else
                {
                    array = (T[])arrayToAppend.Clone();
                }
            }
        }
    }
}