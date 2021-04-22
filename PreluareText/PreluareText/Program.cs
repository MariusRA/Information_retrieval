using System;
using System.Xml;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PreluareText
{
    class Program
    {

        //dictionar global <string, int>
        //list<dictionar<string,int>>

        //sau matrice.+vector de stringuri cu cuvintele

       
        static void Main(string[] args)
        {
            TextMiner textMiner = new TextMiner();

            textMiner.allWords = textMiner.words("title","text");
            List<string> uniqueWords = new List<string>();

            textMiner.stpWords = textMiner.getStopwords(textMiner.stopWordsFilePath);

            textMiner.removeStopWords(textMiner.allWords, textMiner.stpWords);

            for (int i = 0; i < textMiner.allWords.Count(); i++)
            {
                if (!uniqueWords.Contains(textMiner.allWords[i]))
                {
                    uniqueWords.Add(textMiner.allWords[i]);
                }
            }

          // textMiner.writeWordsToFile(textMiner.allWords, @"F:\Information_retrieval\PreluareText\cuvinte.txt");
          //  textMiner.writeWordsToFile(uniqueWords, @"F:\Information_retrieval\PreluareText\cuvinte_unice.txt");            

          //  textMiner.postText(textMiner.allWords);

            Console.WriteLine(textMiner.allWords.Count());
            Console.WriteLine(textMiner.stpWords.Count());
            Console.WriteLine(uniqueWords.Count());
           
            Console.WriteLine(textMiner.eliminatedStopwords);

 
        }
    }
}
