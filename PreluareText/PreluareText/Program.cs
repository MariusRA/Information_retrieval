using System;
using System.Xml;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Porter2Stemmer;

namespace PreluareText
{
    class Program
    {

        //mai departe
        //ne intereseaza nodul topic
        //vedem entropia setului ca sa vedem daca putem reduce numarul de cuvinte
        static void Main(string[] args)
        {

            TextMiner textMiner = new TextMiner();

            textMiner.words("title", "text");

            //textMiner.stpWords = textMiner.getStopwords(textMiner.stopWordsFilePath);
            //textMiner.removeStopWords(textMiner.allWords, textMiner.stpWords);

            //EnglishPorter2Stemmer stemmer = new EnglishPorter2Stemmer();
            //for (int k = 0; k < textMiner.allWords.Count(); k++)
            //{
            //    textMiner.allWords[k] = stemmer.Stem(textMiner.allWords[k]).Value;
            //}

            textMiner.uniqueWords = textMiner.getUniqueWords(textMiner.allWords);


            int[,] freqM = new int[textMiner.reuters34Number, textMiner.uniqueWords.Count()];
            textMiner.frequencyMatrix = textMiner.buildFrequencyMatrix(freqM);

            textMiner.writeWordsToFile(textMiner.allWords, @"F:\Information_retrieval\PreluareText\cuvinte.txt");
            textMiner.writeWordsToFile(textMiner.uniqueWords, @"F:\Information_retrieval\PreluareText\cuvinte_unice.txt");

            Console.WriteLine(textMiner.allWords.Count());
            Console.WriteLine(textMiner.stpWords.Count());
            Console.WriteLine(textMiner.uniqueWords.Count());
            Console.WriteLine(textMiner.eliminatedStopwords);

            foreach(var x in textMiner.documentsTopics)
            {
                Console.WriteLine(x);
            }

        }
    }
}
