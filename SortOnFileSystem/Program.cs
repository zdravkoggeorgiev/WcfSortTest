using System;
using System.Diagnostics;

namespace SortOnFileSystem
{
    class Program
    {
        static string _targetFile = "sorted.txt";

        // 17MB
        static long _maxSize = 18253611;

        // 17GB
        // static long _maxSize = 18253611008; 

        static void Main(string[] args)
        {

            SortBigData bigData = new SortBigData();
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                bigData.SortOnFileSystem(_targetFile);
                sw.Stop();
                Console.WriteLine("Sorting of {0} bytes took : {1} ", _maxSize, sw.Elapsed);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something terrible just happend!\n" + ex.ToString());
            }
            Console.ReadKey();
        }
    }
}
