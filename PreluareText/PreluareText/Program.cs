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

            textMiner.wordsFromTags("title", "text");

            string query = "It said Monday that more than one million" +
                " users have downloaded its latest Internet Explorer version in its first week of availability.";

            textMiner.wordsFromQuery(query);

            textMiner.uniqueWords = textMiner.getUniqueWords(textMiner.allWords);
       
            double[,] freqM = new double[textMiner.reuters7083Number+1, textMiner.uniqueWords.Count()];
            textMiner.frequencyMatrix = textMiner.buildFrequencyMatrix(freqM);

            Console.WriteLine("Total number of words: " + textMiner.allWords.Count());
            Console.WriteLine("Number of stopwords: " + textMiner.stpWords.Count());
            Console.WriteLine("Number of unique words: " + textMiner.uniqueWords.Count());
            Console.WriteLine("Number of eliminated stopwords: " + textMiner.eliminatedStopwords);

            textMiner.frequencyMatrix = textMiner.normalise(textMiner.frequencyMatrix, 7084, textMiner.uniqueWords.Count());

            textMiner.similarityCalculation(7083, 7084, textMiner.uniqueWords.Count(), textMiner.frequencyMatrix);

            for (int i = 0; i < textMiner.similarity.Count(); i++)
            {
                Console.WriteLine(textMiner.similarity[i]);
            }

            string path = @"F:\Information_retrieval\PreluareText\similaritati.txt";
            using (StreamWriter outputFile = new StreamWriter(path))
                foreach (var s in textMiner.similarity)
                {
                    outputFile.WriteLine(s);
                }

        }
    }
}
