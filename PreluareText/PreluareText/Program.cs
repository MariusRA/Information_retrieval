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

        static string stopWordsFilePath = @"F:\Information_retrieval\PreluareText\stop_words_english.txt";
        static string reuters34FilePath = @"F:\Information_retrieval\Reuters_34";
        static string reuters7083FilePath = @"F:\Information_retrieval\Reuters_7083";


        static char[] separators = new char[] { ' ', ',', '?', '!', '"', ':', ';', '{', '}', '(', ')',
                                                '/', '#', '@', '$', '%', '*', '-', '&', '^', '|', '\t',
                                                '\b','\r','\v','\f','\'','+'};

        public static void post_text(List<string> s)
        {
            foreach (var cuv in s)
            {
                Console.WriteLine(cuv);
            }
        }

        public static List<string> words(string t, string tx)
        {
            DirectoryInfo my_directory = new DirectoryInfo(reuters34FilePath);
            FileInfo[] file_info = my_directory.GetFiles("*.xml");
            XmlDocument my_xml_doc = new XmlDocument();

            List<string> allWords = new List<string>();

            foreach (FileInfo file in file_info)
            {
                string[] titleWords, textWords;

                my_xml_doc.Load(file.FullName);

                XmlNode titleNode = my_xml_doc.DocumentElement.SelectSingleNode(t);
                XmlNode textNode = my_xml_doc.DocumentElement.SelectSingleNode(tx);

                titleWords = titleNode.InnerText.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
                textWords = textNode.InnerText.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);

                List<string> listTitle = titleWords.ToList();
                List<string> listText = textWords.ToList();

                for (int i = 0; i < listTitle.Count(); i++)
                {

                    listTitle[i] = Regex.Replace(listTitle[i], @"[\d-]", string.Empty);

                    if (listTitle[i] == "." || listTitle[i] == "..." || listTitle[i].Length <= 2)
                    {
                        listTitle[i] = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(listTitle[i]) && listTitle[i].Contains("."))
                    {

                        if (listTitle[i].IndexOf(".") == (listTitle[i].Length - 1))
                        {
                            listTitle[i] = listTitle[i].Substring(0, (listTitle[i].Length - 1));
                        }
                        else if (listTitle[i].IndexOf(".") == 0)
                        {
                            listTitle[i] = listTitle[i].Substring(1, (listTitle[i].Length - 1));
                        }
                        else
                        {
                            //cand e punctul altundeva
                            var temp = listTitle[i].Split('.');
                            //bool flag = true;

                            foreach (string s in temp)
                            {
                                if (s.Length > 1)
                                {
                                    listTitle.RemoveAt(i);
                                    for (int k = temp.Count() - 1; k >= 0; k--)
                                    {
                                        if (temp[k] != string.Empty)
                                        {
                                            listTitle.Insert(i, temp[k]);
                                        }
                                    }
                                }
                            }
                        }
                    }


                    if (!string.IsNullOrEmpty(listTitle[i]))
                    {
                        allWords.Add(listTitle[i].ToLower());
                    }
                }

                for (int i = 0; i < listText.Count(); i++)
                {
                    listText[i] = Regex.Replace(listText[i], @"[\d-]", string.Empty);


                    if (listText[i] == "." || listText[i] == "..." || listText[i].Length <= 2)
                    {
                        listText[i] = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(listText[i]) && listText[i].Contains("."))
                    {
                        if (listText[i].IndexOf(".") == (listText[i].Length - 1))
                        {
                            listText[i] = listText[i].Substring(0, (listText[i].Length - 1));
                        }
                        else if (listText[i].IndexOf(".") == 0)
                        {
                            listText[i] = listText[i].Substring(1, (listText[i].Length - 1));
                        }
                        else
                        {
                            var temp = listText[i].Split('.');
                            //bool flag = true;

                            foreach (string s in temp)
                            {
                                if (s.Length > 1)
                                {
                                    listText.RemoveAt(i);
                                    for (int k = temp.Count() - 1; k >= 0; k--)
                                    {
                                        if (temp[k] != string.Empty)
                                        {
                                            listText.Insert(i, temp[k]);
                                        }
                                    }
                                }
                            }
                        }

                    }

                    if (!string.IsNullOrEmpty(listText[i]))
                    {
                        allWords.Add(listText[i].ToLower());
                    }
                }

            }
            return allWords;
        }

        public static List<string> getStopwords(string filepath)
        {
            List<string> stopwords = new List<string>();
            string[] lines = File.ReadAllLines(filepath);

            foreach (string s in lines)
            {
                stopwords.Add(s);
            }
            return stopwords;
        }

        static void Main(string[] args)
        {
            string title_node = "title";
            string text_node = "text";

            List<string> allWords = words(title_node, text_node);
            List<string> uniqueWords = new List<string>();
            //post_text(allWords);
            List<string> stpWords = new List<string>();

            stpWords = getStopwords(stopWordsFilePath);

            for (int i = 0; i < allWords.Count(); i++)
            {
                if (stpWords.IndexOf(allWords[i]) >= 0)
                {
                    allWords[i] = allWords[i].Replace(allWords[i], string.Empty);
                }
            }
            int size = allWords.Count() - 1;
            for (int i = size; i >= 0; i--)
            {
                if (allWords[i] == string.Empty)
                {
                    allWords.RemoveAt(i);
                }
            }

            for (int i = 0; i < allWords.Count(); i++)
            {
                if (!uniqueWords.Contains(allWords[i]))
                {
                    uniqueWords.Add(allWords[i]);
                }
            }

            using (StreamWriter outputFile = new StreamWriter(@"F:\Information_retrieval\PreluareText\cuvinte.txt"))
                foreach (var s in allWords)
                {
                    outputFile.WriteLine(s);
                }
            using (StreamWriter outputFile = new StreamWriter(@"F:\Information_retrieval\PreluareText\cuvinte_unice.txt"))
                foreach (var s in uniqueWords)
                {
                    outputFile.WriteLine(s);
                }

            post_text(allWords);

        }
    }
}
