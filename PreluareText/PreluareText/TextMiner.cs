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

        public List<string> allWords;
        public List<string> stpWords;
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

        public List<string> words(string t, string tx)
        {
            char[] separators = new char[] { ' ', ',', '?', '!', '"', ':', ';', '{', '}', '(', ')',
                                                '/', '#', '@', '$', '%', '*', '-', '&', '^', '|', '\t',
                                                '\b','\r','\v','\f','\'','+'
            };

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
                    listTitle[i] = listTitle[i].ToLower().Trim();

                    listTitle[i] = Regex.Replace(listTitle[i], @"[\d-]", string.Empty);

                    if (listTitle[i] == "." || listTitle[i] == "..." || listTitle[i].Length <= 2)
                    {
                        listTitle[i] = string.Empty;
                    }

                    if (Regex.IsMatch(listTitle[i], @"(?:[a-zA-Z]\.){2,}"))
                    {
                        string temp = null;
                        for (int k = 0; k < listTitle[i].Length; k++)
                        {
                            if (!(listTitle[i][k] == '.'))
                            {
                                temp += listTitle[i][k];
                            }
                        }
                        listTitle[i] = temp;
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
                            //Co.. eroare
                            //cand e punctul altundeva
                            var temp = listTitle[i].Split('.');

                            foreach (string s in temp)
                            {
                                if (s.Length > 1)
                                {
                                    listTitle[i] = string.Empty;
                                    for (int k = temp.Count() - 1; k >= 0; k--)
                                    {
                                        if (temp[k] != string.Empty && temp[k].Length > 2)
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
                        allWords.Add(listTitle[i]);
                    }
                }

                for (int i = 0; i < listText.Count(); i++)
                {
                    listText[i] = listText[i].ToLower().Trim();

                    listText[i] = Regex.Replace(listText[i], @"[\d-]", string.Empty);

                    if (Regex.IsMatch(listText[i], @"(?:[a-zA-Z]\.){2,}"))
                    {
                        string temp = null;
                        for (int k = 0; k < listText[i].Length; k++)
                        {
                            if (!(listText[i][k] == '.'))
                            {
                                temp += listText[i][k];
                            }
                        }
                        listText[i] = temp;
                    }

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

                            foreach (string s in temp)
                            {
                                if (s.Length > 1)
                                {
                                    listText[i] = string.Empty;
                                    for (int k = temp.Count() - 1; k >= 0; k--)
                                    {
                                        if (temp[k] != string.Empty && temp[k].Length > 2)
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
                        allWords.Add(listText[i]);
                    }
                }

            }
            return allWords;
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

    }
}
