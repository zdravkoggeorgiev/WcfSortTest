using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SortClient.ConsoleClientServiceReference;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Threading;

namespace SortClient
{
    class Program
    {
        #region Static Private Fields

        // Variables, used for communication between Threads
        private static Guid _uid1;
        private static bool _IsBGThreadSent = false;
        private static bool _IsClient1Ended = false;

        // Unsorted arrays of strings, to be sent to the service and to be sort
        private static string[] _unsortArr1 = { "Searobin", "Bluefish", "Dolphin", "Gag Grouper", "Hogfish", "King Mackerel", "Knobbed Porgy",
            "Red Snapper", "Vermillion Snapper", "Wahoo", "Yellowedge Grouper", "4632", "Spottail Pinfish", "Yellowtail Snapper", "019294", "Red Grouper" };
        private static string[] _unsortArr2 = { "White Grunt", "123", "43534", "Silver Snapper", "315", "Silver Snapper" };
        private static string[] _unsortArr3 = { "Speckled Hind", "715", "Blue Marlin", "Sailfish", "White Marlin", "Arowana", "Bluegill",
            "Lampfish", "Minnow of the deep", "Goblin shark", "Flathead catfish", "Freshwater hatchetfish", "Duckbill", "Loach catfish", "29243", "Swordfish" };

        // Already sorted arrays, to be compared with service results
        private static string[] _sortedArr1 = { "019294", "123", "29243", "315", "43534", "4632", "715", "Arowana", "Blue Marlin", "Bluefish", "Bluegill", "Dolphin", "Duckbill",
            "Flathead catfish", "Freshwater hatchetfish", "Gag Grouper", "Goblin shark", "Hogfish", "King Mackerel", "Knobbed Porgy", "Lampfish", "Loach catfish", "Minnow of the deep",
            "Red Grouper", "Red Snapper", "Sailfish", "Searobin", "Silver Snapper", "Silver Snapper", "Speckled Hind", "Spottail Pinfish", "Swordfish", "Vermillion Snapper", "Wahoo",
            "White Grunt", "White Marlin", "Yellowedge Grouper", "Yellowtail Snapper" };
        private static string[] _sortedArr2 = { "019294", "29243", "4632", "715", "Arowana", "Blue Marlin", "Bluefish", "Bluegill", "Dolphin", "Duckbill", "Flathead catfish",
            "Freshwater hatchetfish", "Gag Grouper", "Goblin shark", "Hogfish", "King Mackerel", "Knobbed Porgy", "Lampfish", "Loach catfish", "Minnow of the deep", "Red Grouper",
            "Red Snapper", "Sailfish", "Searobin", "Speckled Hind", "Spottail Pinfish", "Swordfish", "Vermillion Snapper", "Wahoo", "White Marlin", "Yellowedge Grouper", "Yellowtail Snapper" };

        #endregion

        /// <summary>
        /// A simple Console WCF Client Application, to test base WCF funcionality. Personally, I preffer to use tests for this.
        /// </summary>
        static void Main(string[] args)
        {
            SortingServiceClient clientSort1 = new SortingServiceClient("BasicHttpBinding_ISortingService");
            _uid1 = clientSort1.BeginStream();
            clientSort1.PutStreamData(_uid1, _unsortArr1);

            // Send data from another thread, to same uid
            Thread sortThread = new Thread(PutStreamBGThread) { IsBackground = true };
            sortThread.Start();

            // Start second client, to achieve second uid (uid2), sort small data, and close
            SortingServiceClient clientSort2 = new SortingServiceClient("BasicHttpBinding_ISortingService");
            var uid2 = clientSort2.BeginStream();
            clientSort2.PutStreamData(uid2, _unsortArr3);
            clientSort2.PutStreamData(uid2, _unsortArr1);
            var result2 = GetSortedStream(uid2, clientSort2).ToArray();
            clientSort2.EndStream(uid2);
            clientSort2.Close();
            TestCondition(_sortedArr2.SequenceEqual(result2), "uid2 arrays was sorted as expected.");

            // wait background thread to send it's data too.
            while (!_IsBGThreadSent)
                Thread.Sleep(10);

            // put last data to _uid1
            clientSort1.PutStreamData(_uid1, _unsortArr3);
            var result1 = GetSortedStream(_uid1, clientSort1).ToArray();
            clientSort1.EndStream(_uid1);
            clientSort1.Close();
            _IsClient1Ended = true;
            TestCondition(_sortedArr1.SequenceEqual(result1), "_uid1 arrays was sorted as expected.");

            // Wait Thread to finish. If no Exceptions so far - all tests passed
            sortThread.Join();
            Console.WriteLine("Successful end!");
            Console.ReadLine();
        }

        #region Static Helper Functions

        /// <summary>
        /// Puts data into Stream in a background thread. Takes parameters from statics (_uid1) and (_unsortArr2).
        /// Useful only for testing.
        /// </summary>
        protected static void PutStreamBGThread()
        {
            SortingServiceClient clientSortBGThread = new SortingServiceClient("BasicHttpBinding_ISortingService");

            // this is sent to _uid1 created in main thread
            clientSortBGThread.PutStreamData(_uid1, _unsortArr2);
            _IsBGThreadSent = true;

            // Wait Stream _uid1 to be closed in main thread
            while (!_IsClient1Ended)
                Thread.Sleep(10);

            // Code below should not retrieve data, because Stream _uid1 is already ended in main Thread
            var shouldBeEmpty = GetSortedStream(_uid1, clientSortBGThread).ToArray();
            clientSortBGThread.Close();

            TestCondition(shouldBeEmpty.Count() == 0, "Sorted stream _uid1 ended.");
        }

        /// <summary>
        /// Gets data from WCF Server - Sorted Stream of text lines
        /// </summary>
        /// <param name="UID">Guid of already sent sequences to WCF Server. </param>
        /// <param name="clientSort">Instance of SortingServiceClient to be used for connection.</param>
        /// <returns> Sorted data from WCF Service </returns>
        protected static IEnumerable<string>  GetSortedStream(Guid UID, SortingServiceClient clientSort)
        {
            using (Stream streamSorted = clientSort.GetSortedStream(UID))
            {
                using (StreamReader reader = new StreamReader(streamSorted))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        yield return line;
                    }
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Used for Application early end on wrong condition.
        /// </summary>
        /// <param name="isOk"> boolean or function with boolean result, to be tested </param>
        /// <param name="msg"> Message in Console on success. Also used for Exception message, on test IsOk fail. </param>
        protected static void TestCondition(bool isOk, string msg)
        {
            if (isOk)
            {
                Console.WriteLine(msg);
            }
            else
            {
                throw new Exception("FAILED: " + msg);
            }
        }

        #endregion
    }
}
