using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace PreluareText
{

    class TextMiner
    {
        public string stopWordsFilePath = @"F:\Information_retrieval\PreluareText\stop_words_english.txt";
        public string reuters34FilePath = @"F:\Information_retrieval\Reuters_34";
        public string reuters7083FilePath = @"F:\Information_retrieval\Reuters_7083";

        public List<string> allWords = new List<string>();
        public List<string> stpWords;
        public List<string> uniqueWords;
        public List<string> fileNames = new List<string>();
        public List<Dictionary<string, int>> documentsListsOfWords = new List<Dictionary<string, int>>();

        public int eliminatedStopwords = 0;


        public List<string> getStopwords(string filepath)
        {
            List<string> stopwords = new List<string>();
            string[] lines = File.ReadAllLines(filepath);

            foreach (string s in lines)
            {
                stopwords.Add(s);
            }
            return stopwords;
        }

        public void postText(List<string> s)
        {
            foreach (var cuv in s)
            {
                Console.WriteLine(cuv);
            }
        }

        public void words(string t, string tx)
        {
            char[] separators = new char[] { ' ', ',', '?', '!', '"', ':', ';', '{', '}', '(', ')',
                                                '/', '#', '@', '$', '%', '*', '-', '&', '^', '|', '\t',
                                                '\b','\r','\v','\f','\'','+','\\'
            };

            DirectoryInfo my_directory = new DirectoryInfo(reuters34FilePath);
            FileInfo[] file_info = my_directory.GetFiles("*.xml");
            XmlDocument my_xml_doc = new XmlDocument();

            foreach (FileInfo file in file_info)
            {
                this.fileNames.Add(file.Name);

                string[] wordsFromXML;
                string wordsWelded;

                my_xml_doc.Load(file.FullName);

                XmlNodeList titleNode = my_xml_doc.DocumentElement.GetElementsByTagName(t);
                XmlNodeList textNode = my_xml_doc.DocumentElement.GetElementsByTagName(tx);

                wordsWelded = titleNode[0].InnerText + textNode[0].InnerText;

                wordsFromXML = wordsWelded.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);

                List<string> fileWordsList = wordsFromXML.ToList();

                for (int i = 0; i < fileWordsList.Count(); i++)
                {
                    fileWordsList[i] = fileWordsList[i].ToLower().Trim();

                    fileWordsList[i] = Regex.Replace(fileWordsList[i], @"[\d-]", string.Empty);

                    if (fileWordsList[i] == "." || fileWordsList[i] == "..." || fileWordsList[i].Length <= 2)
                    {
                        fileWordsList[i] = string.Empty;
                    }

                    if (Regex.IsMatch(fileWordsList[i], @"(?:[a-zA-Z]\.){2,}"))
                    {
                        string temp = null;
                        for (int k = 0; k < fileWordsList[i].Length; k++)
                        {
                            if (!(fileWordsList[i][k] == '.'))
                            {
                                temp += fileWordsList[i][k];
                            }
                        }
                        fileWordsList[i] = temp;
                    }


                    if (!string.IsNullOrEmpty(fileWordsList[i]) && fileWordsList[i].Contains("."))
                    {

                        if (fileWordsList[i].IndexOf(".") == (fileWordsList[i].Length - 1))
                        {
                            fileWordsList[i] = fileWordsList[i].Substring(0, (fileWordsList[i].Length - 1));
                        }
                        else if (fileWordsList[i].IndexOf(".") == 0)
                        {
                            fileWordsList[i] = fileWordsList[i].Substring(1, (fileWordsList[i].Length - 1));
                        }
                        else
                        {
                            //Co.. eroare
                            //cand e punctul altundeva
                            var temp = fileWordsList[i].Split('.');

                            foreach (string s in temp)
                            {
                                if (s.Length > 1)
                                {
                                    fileWordsList[i] = string.Empty;
                                    for (int k = temp.Count() - 1; k >= 0; k--)
                                    {
                                        if (temp[k] != string.Empty && temp[k].Length > 2)
                                        {
                                            fileWordsList.Insert(i, temp[k]);
                                        }
                                    }
                                }
                            }
                        }
                    }

                  
                    if (!string.IsNullOrEmpty(fileWordsList[i]))
                    {
                        this.allWords.Add(fileWordsList[i]);
                    }
                }

                fileWordsList = fileWordsList.Where(w => !string.IsNullOrWhiteSpace(w)).ToList();
                List<string> uniqueWordsFromEachFile = new List<string>();
                uniqueWordsFromEachFile = getUniqueWords(fileWordsList);
                Dictionary<string, int> wordsAndFrequencyFromEachFile = new Dictionary<string, int>();

                for (int k = 0; k < uniqueWordsFromEachFile.Count(); k++)
                {
                    int numberOfAppearances = fileWordsList.Count(word => word == uniqueWordsFromEachFile[k]);
                    wordsAndFrequencyFromEachFile.Add(uniqueWordsFromEachFile[k], numberOfAppearances);
                }
                this.documentsListsOfWords.Add(wordsAndFrequencyFromEachFile);

            }
            this.allWords.Sort();           
        }

        public void removeStopWords(List<string> words, List<string> stopwords)
        {
            for (int i = 0; i < words.Count(); i++)
            {
                if (stopwords.IndexOf(words[i]) >= 0)
                {
                    eliminatedStopwords++;
                    words[i] = words[i].Replace(words[i], string.Empty);
                }
            }
            int size = words.Count() - 1;
            for (int i = size; i >= 0; i--)
            {
                if (words[i] == string.Empty)
                {
                    words.RemoveAt(i);
                }
            }
        }

        public void writeWordsToFile(List<string> words, string path)
        {
            using (StreamWriter outputFile = new StreamWriter(path))
                foreach (var s in words)
                {
                    outputFile.WriteLine(s);
                }
        }

        public List<string> getUniqueWords(List<string> wordsList)
        {
            List<string> unique = new List<string>();
            for (int i = 0; i < wordsList.Count(); i++)
            {
                if (!unique.Contains(wordsList[i]))
                {
                    unique.Add(wordsList[i]);
                }
            }
            unique.Sort();
            return unique;
        }

    }
}
