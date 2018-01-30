using System;
using System.Diagnostics;

namespace SortOnFileSystem
{
    class Program
    {
        #region Private Static Fields
        private static string _targetFile = "sorted.txt";
        private static string _dictionaryFile = "fish_names.txt";

        private static int _maxMemBlockSize = 500 * 1024 * 1024;
        private static int _maxBlocs = 33;

        private static int _numberOfGenWordsInLine = 100;

        // 17MB
        private static long _maxTotalSize = 18253611;

        // 17GB
        // private static long _maxTotalSize = 18253611008; 

        #endregion

        static void Main(string[] args)
        {

            SortBigData bigData = new SortBigData();
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                bigData.GenerateFakeData(_maxMemBlockSize, _maxBlocs, _dictionaryFile, _numberOfGenWordsInLine);
                bigData.SortOnFileSystem(_targetFile);
                sw.Stop();
                Console.WriteLine("Sorting of {0} bytes took : {1} ", _maxTotalSize, sw.Elapsed);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something terrible just happend!\n" + ex.ToString());
            }
            Console.ReadKey();
        }
    }
}
