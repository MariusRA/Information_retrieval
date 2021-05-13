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

        //dictionar global <string, int>
        //list<dictionar<string,int>>

        //sau matrice.+vector de stringuri cu cuvintele

        //idee
        //matrice care are pe coloane cuvintele unice
        //o initializez cu 0
        //de fiecare data cand deschid un fisier salvez cuvintele intr o lista
        //                                fac o lista cu frecventele cuvintelor din document
        //pun frecventa cuvintelor din fiecare document
        //completez liniile din matrice cu lista de frecvente

        static void Main(string[] args)
        {

            TextMiner textMiner = new TextMiner();

            textMiner.words("title", "text");
            textMiner.stpWords = textMiner.getStopwords(textMiner.stopWordsFilePath);

            textMiner.removeStopWords(textMiner.allWords, textMiner.stpWords);

            //EnglishPorter2Stemmer stemmer = new EnglishPorter2Stemmer();
            //for (int k = 0; k < textMiner.allWords.Count(); k++)
            //{
            //    textMiner.allWords[k] = stemmer.Stem(textMiner.allWords[k]).Value;
            //}

            textMiner.uniqueWords = textMiner.getUniqueWords(textMiner.allWords);

            textMiner.writeWordsToFile(textMiner.allWords, @"F:\Information_retrieval\PreluareText\cuvinte.txt");
            textMiner.writeWordsToFile(textMiner.uniqueWords, @"F:\Information_retrieval\PreluareText\cuvinte_unice.txt");

            Console.WriteLine(textMiner.allWords.Count());
            Console.WriteLine(textMiner.stpWords.Count());
            Console.WriteLine(textMiner.uniqueWords.Count());

            Console.WriteLine(textMiner.eliminatedStopwords);

            foreach (var x in textMiner.documentsListsOfWords)
            {
                foreach (var y in x)
                {
                    Console.WriteLine(y);
                }
                Console.WriteLine();
            }

            //for (int i = 0; i < textMiner.uniqueWords.Count(); i++)
            //{
            //    int numberOfAppearances = textMiner.allWords.Count(word => word == textMiner.uniqueWords[i]);
            //    Console.WriteLine(textMiner.uniqueWords[i] + " " + numberOfAppearances);
            //}

        }
    }
}
