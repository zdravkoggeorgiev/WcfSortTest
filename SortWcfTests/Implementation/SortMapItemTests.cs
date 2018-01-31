using NUnit.Framework;
using WcfSortTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SortClient.ConsoleClientServiceReference;
using System.IO;

namespace WcfSortTest.Tests
{
    [TestFixture()]
    public class SortMapItemTests
    {
        #region Private Fields (initializators)

        // Unsorted arrays of strings, to be sent to the service and to be sort
        private string[] _unsortArr1 = { "Searobin", "Bluefish", "Dolphin", "Gag Grouper", "Hogfish", "King Mackerel", "Knobbed Porgy",
            "Red Snapper", "Vermillion Snapper", "Wahoo", "Yellowedge Grouper", "4632", "Spottail Pinfish", "Yellowtail Snapper", "019294", "Red Grouper" };
        private string[] _unsortArr2 = { "White Grunt", "123", "43534", "Silver Snapper", "315", "Silver Snapper" };
        private string[] _unsortArr3 = { "Speckled Hind", "715", "Blue Marlin", "Sailfish", "White Marlin", "Arowana", "Bluegill",
            "Lampfish", "Minnow of the deep", "Goblin shark", "Flathead catfish", "Freshwater hatchetfish", "Duckbill", "Loach catfish", "29243", "Swordfish" };

        // Already sorted arrays, to be compared with service results
        private string[] _sortedArr1 = { "019294", "123", "29243", "315", "43534", "4632", "715", "Arowana", "Blue Marlin", "Bluefish", "Bluegill", "Dolphin", "Duckbill",
            "Flathead catfish", "Freshwater hatchetfish", "Gag Grouper", "Goblin shark", "Hogfish", "King Mackerel", "Knobbed Porgy", "Lampfish", "Loach catfish", "Minnow of the deep",
            "Red Grouper", "Red Snapper", "Sailfish", "Searobin", "Silver Snapper", "Silver Snapper", "Speckled Hind", "Spottail Pinfish", "Swordfish", "Vermillion Snapper", "Wahoo",
            "White Grunt", "White Marlin", "Yellowedge Grouper", "Yellowtail Snapper" };
        private string[] _sortedArr2 = { "019294", "29243", "4632", "715", "Arowana", "Blue Marlin", "Bluefish", "Bluegill", "Dolphin", "Duckbill", "Flathead catfish",
            "Freshwater hatchetfish", "Gag Grouper", "Goblin shark", "Hogfish", "King Mackerel", "Knobbed Porgy", "Lampfish", "Loach catfish", "Minnow of the deep", "Red Grouper",
            "Red Snapper", "Sailfish", "Searobin", "Speckled Hind", "Spottail Pinfish", "Swordfish", "Vermillion Snapper", "Wahoo", "White Marlin", "Yellowedge Grouper", "Yellowtail Snapper" };

        #endregion

        [Test()]
        public void AddItemsTest()
        {
            // TODO: Finish this test later
            Assert.Pass();
        }

        /// <summary>
        /// This is mostly integration test, using all methods at once.
        /// </summary>
        [Test()]
        public void GetSortedItemsTest()
        {
            SortMapItem sortMapItem = new SortMapItem();
            Assert.IsNotNull(sortMapItem.UID);
            Assert.AreNotEqual(Guid.Empty, sortMapItem.UID);

            sortMapItem.AddItems(_unsortArr1);
            var countLines = sortMapItem.CountLines();
            var expectedCount = _unsortArr1.Length;
            Assert.AreEqual(expectedCount, countLines);

            sortMapItem.AddItems(_unsortArr3);
            countLines = sortMapItem.CountLines();
            expectedCount += _unsortArr3.Length;
            Assert.AreEqual(expectedCount, countLines);

            sortMapItem.AddItems(_unsortArr2);
            countLines = sortMapItem.CountLines();
            expectedCount += _unsortArr2.Length;
            Assert.AreEqual(expectedCount, countLines);

            var resultsSorted = this.GetSortedStream(sortMapItem);
            Assert.IsTrue(_sortedArr1.SequenceEqual(resultsSorted));

            sortMapItem.Dispose();
            Assert.Pass("Arrays was sorted as expected");
        }

        [Test()]
        public void CountLinesTest()
        {
            // TODO: Finish this test later
            Assert.Pass();
        }

        [Test()]
        public void DisposeTest()
        {
            // TODO: Finish this test later
            Assert.Pass();
        }

        #region Private Helper Methods

        /// <summary>
        /// Gets data from SortMapItem - Sorted Stream of text lines, and coverts it to IEnumerable
        /// </summary>
        /// <param name="sortMapItem">Instance to read from. </param>
        /// <returns> Sorted data from instance </returns>
        private IEnumerable<string> GetSortedStream(SortMapItem sortMapItem)
        {
            using (Stream streamSorted = sortMapItem.GetSortedItems())
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

        #endregion
    }
}