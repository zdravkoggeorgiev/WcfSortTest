using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SortOnFileSystem
{
    public class SortBigData
    {
        private string[] _dictionaryGeneration;
        private string tmpPath = Directory.GetCurrentDirectory() + "\\AppData\\";

        public void GenerateFakeData(long blockSize, int blocksCount, string dictionaryFileName, int numberOfGenWordsInLine)
        {
            LoadDictionaryForGenerator(dictionaryFileName);
            for (int i = 0; i < blocksCount; i++)
            {
                GenerateSingleBlock(blockSize, numberOfGenWordsInLine, i);
            }
        }

        private void GenerateSingleBlock(long blockSize, int numberOfGenWordsInLine, int currNum)
        {
            long currentSize = 0;
            string targetFileName = $"{tmpPath}temp_{currNum:D3}.txt";

            List<string> listStrings = new List<string>(6000000); // ZDGV Remove the magic number later

            while(currentSize < blockSize)
            {
                listStrings.Add(GenerateLine(numberOfGenWordsInLine));
            }

            listStrings.Sort();

            File.WriteAllLines(targetFileName, listStrings);
        }

        private string GenerateLine(int numberOfGenWordsInLine)
        {
            StringBuilder sb = new StringBuilder(numberOfGenWordsInLine);
            Random rnd = new Random();
            int maxDict = _dictionaryGeneration.Length - 1;

            for (int i = 0; i < numberOfGenWordsInLine; i++)
            {
                sb.Append(_dictionaryGeneration[rnd.Next(0, maxDict)] + " ");
            }
            sb.AppendLine();

            return sb.ToString();
        }

        private void LoadDictionaryForGenerator(string dictionaryFileName)
        {
            // Stream would be faster here, but we load small file only once
            var dictFile = File.ReadAllLines(dictionaryFileName);
            var listDictFile = new List<string>(dictFile);
            _dictionaryGeneration = listDictFile.ToArray();
            Array.Sort(_dictionaryGeneration);
        }



        public void SortOnFileSystem(string targetFileName)
        {

        }

    }
}
