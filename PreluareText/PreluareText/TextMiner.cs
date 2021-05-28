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
        public int reuters34Number = 34;
        public int reuters7083Number = 7083;

        public List<string> allWords = new List<string>();
        public List<string> stpWords;
        public List<string> uniqueWords;
        public List<string> fileNames = new List<string>();
        public List<string> documentsTopics = new List<string>();
        public List<List<double>> normalisedVectors = new List<List<double>>();

        public List<int> documentsMaxValue = new List<int>();
        public List<double> similarity = new List<double>();

        public Dictionary<double, string> similarityAndDocName = new Dictionary<double, string>();

        public Dictionary<string, int> topicsAndNumberOfAppearances = new Dictionary<string, int>();
        public List<Dictionary<string, int>> documentsListsOfWords = new List<Dictionary<string, int>>();

        public double[,] frequencyMatrix;

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

        public void wordsFromTags(string tag1, string tag2)
        {
            char[] separators = new char[] { ' ', ',', '?', '!', '"', ':', ';', '{', '}', '(', ')',
                                                '/', '#', '@', '$', '%', '*', '-', '&', '^', '|', '\t',
                                                '\b','\r','\v','\f','\'','+','\\'
            };

            DirectoryInfo my_directory = new DirectoryInfo(reuters34FilePath);
            FileInfo[] file_info = my_directory.GetFiles("*.xml");
            XmlDocument my_xml_doc = new XmlDocument();

            this.stpWords = getStopwords(stopWordsFilePath);
            int contor = 0;
            foreach (FileInfo file in file_info)
            {
               
                this.fileNames.Add(file.Name);

                string[] wordsFromXML;
                string wordsWelded;

                my_xml_doc.Load(file.FullName);

                XmlNodeList titleNode = my_xml_doc.DocumentElement.GetElementsByTagName(tag1);
                XmlNodeList textNode = my_xml_doc.DocumentElement.GetElementsByTagName(tag2);

                //XmlNodeList topicNode = my_xml_doc.GetElementsByTagName("metadata");

                //foreach (XmlNode node in topicNode)
                //{
                //    var l = node.SelectNodes("/newsitem/metadata/codes");
                //    this.documentsTopics.Add(l[2].FirstChild.Attributes[0].Value);
                //}

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

                fileWordsList = fileWordsList.Where(word => !string.IsNullOrWhiteSpace(word)).ToList();

                removeStopWords(fileWordsList, this.stpWords);
                List<string> uniqueWordsFromEachFile = new List<string>();
                uniqueWordsFromEachFile = getUniqueWords(fileWordsList);
                Dictionary<string, int> wordsAndFrequencyFromEachFile = new Dictionary<string, int>();

                int maxValueFromDocument = -1;

                for (int k = 0; k < uniqueWordsFromEachFile.Count(); k++)
                {
                    int numberOfAppearances = fileWordsList.Count(word => word == uniqueWordsFromEachFile[k]);
                    wordsAndFrequencyFromEachFile.Add(uniqueWordsFromEachFile[k], numberOfAppearances);

                    if (numberOfAppearances > maxValueFromDocument)
                    {
                        maxValueFromDocument = numberOfAppearances;//max n(r,t) frecventa de aparitie cea mai mare din documentul respectiv
                    }

                }


                this.documentsMaxValue.Add(maxValueFromDocument);
                this.documentsListsOfWords.Add(wordsAndFrequencyFromEachFile);

                Console.WriteLine(contor++);

                ////normalizare
                //List<double> normalisedVectorForCurrentFile = new List<double>();
                //foreach (var line in wordsAndFrequencyFromEachFile)
                //{
                //    double value = (double)line.Value / maxValueFromDocument;
                //    normalisedVectorForCurrentFile.Add(value);
                //}
                //this.normalisedVectors.Add(normalisedVectorForCurrentFile);

            }

            //this.documentsTopics.Sort();
            //List<string> documentsTopicsUnique = getUniqueWords(documentsTopics);
            //for (int l = 0; l < documentsTopicsUnique.Count(); l++)
            //{
            //    int numberOfAppearances = documentsTopics.Count(word => word == documentsTopicsUnique[l]);
            //    this.topicsAndNumberOfAppearances.Add(documentsTopicsUnique[l], numberOfAppearances);
            //}

            removeStopWords(this.allWords, this.stpWords);
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

        public double[,] buildFrequencyMatrix(double[,] matrix)
        {
            int line = 0;

            foreach (var doc in this.documentsListsOfWords)
            {
                foreach (var row in doc)
                {
                    for (int column = 0; column < this.uniqueWords.Count(); column++)
                    {
                        if (row.Key == this.uniqueWords[column])
                        {
                            matrix[line, column] = row.Value;
                            break;
                        }
                    }
                }
                line++;
            }
            return matrix;
        }

        public void wordsFromQuery(string query)
        {
            char[] separators = new char[] { ' ', ',', '?', '!', '"', ':', ';', '{', '}', '(', ')',
                                                '/', '#', '@', '$', '%', '*', '-', '&', '^', '|', '\t',
                                                '\b','\r','\v','\f','\'','+','\\'
            };

            this.fileNames.Add("query1");

            string[] wordsFromQuery;

            wordsFromQuery = query.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);

            List<string> queryWordsList = wordsFromQuery.ToList();

            for (int i = 0; i < queryWordsList.Count(); i++)
            {
                queryWordsList[i] = queryWordsList[i].ToLower().Trim();

                queryWordsList[i] = Regex.Replace(queryWordsList[i], @"[\d-]", string.Empty);

                if (queryWordsList[i] == "." || queryWordsList[i] == "..." || queryWordsList[i].Length <= 2)
                {
                    queryWordsList[i] = string.Empty;
                }

                if (Regex.IsMatch(queryWordsList[i], @"(?:[a-zA-Z]\.){2,}"))
                {
                    string temp = null;
                    for (int k = 0; k < queryWordsList[i].Length; k++)
                    {
                        if (!(queryWordsList[i][k] == '.'))
                        {
                            temp += queryWordsList[i][k];
                        }
                    }
                    queryWordsList[i] = temp;
                }


                if (!string.IsNullOrEmpty(queryWordsList[i]) && queryWordsList[i].Contains("."))
                {

                    if (queryWordsList[i].IndexOf(".") == (queryWordsList[i].Length - 1))
                    {
                        queryWordsList[i] = queryWordsList[i].Substring(0, (queryWordsList[i].Length - 1));
                    }
                    else if (queryWordsList[i].IndexOf(".") == 0)
                    {
                        queryWordsList[i] = queryWordsList[i].Substring(1, (queryWordsList[i].Length - 1));
                    }
                    else
                    {
                        //Co.. eroare
                        //cand e punctul altundeva
                        var temp = queryWordsList[i].Split('.');

                        foreach (string s in temp)
                        {
                            if (s.Length > 1)
                            {
                                queryWordsList[i] = string.Empty;
                                for (int k = temp.Count() - 1; k >= 0; k--)
                                {
                                    if (temp[k] != string.Empty && temp[k].Length > 2)
                                    {
                                        queryWordsList.Insert(i, temp[k]);
                                    }
                                }
                            }
                        }
                    }
                }


                if (!string.IsNullOrEmpty(queryWordsList[i]))
                {
                    this.allWords.Add(queryWordsList[i]);
                }
            }

            queryWordsList = queryWordsList.Where(word => !string.IsNullOrWhiteSpace(word)).ToList();

            removeStopWords(queryWordsList, this.stpWords);
            List<string> uniqueWordsFromQuery = new List<string>();
            uniqueWordsFromQuery = getUniqueWords(queryWordsList);
            Dictionary<string, int> wordsAndFrequencyFromQuery = new Dictionary<string, int>();

            int maxValueFromQuery = -1;

            for (int k = 0; k < uniqueWordsFromQuery.Count(); k++)
            {
                int numberOfAppearances = queryWordsList.Count(word => word == uniqueWordsFromQuery[k]);
                wordsAndFrequencyFromQuery.Add(uniqueWordsFromQuery[k], numberOfAppearances);

                if (numberOfAppearances > maxValueFromQuery)
                {
                    maxValueFromQuery = numberOfAppearances;//max n(r,t) frecventa de aparitie cea mai mare din documentul respectiv
                }

            }


            this.documentsMaxValue.Add(maxValueFromQuery);
            this.documentsListsOfWords.Add(wordsAndFrequencyFromQuery);

            //normalizare
            //List<double> normalisedVectorForQuery = new List<double>();
            //foreach (var line in wordsAndFrequencyFromQuery)
            //{
            //    double value = (double)line.Value / maxValueFromQuery;
            //    normalisedVectorForQuery.Add(value);
            //}
            //this.normalisedVectors.Add(normalisedVectorForQuery);           

            removeStopWords(this.allWords, this.stpWords);
            this.allWords.Sort();

        }

        public double[,] normalise(double[,] matrix, int lines, int columns)
        {
            int line = 0;
            while (line < lines)
            {
                for (int i = 0; i < columns; i++)
                {
                    if (matrix[line, i] == 0)
                    {

                    }
                    else
                    {
                        matrix[line, i] = (double)matrix[line, i] / this.documentsMaxValue[line];
                    }

                }
                line++;
            }

            return matrix;

        }

        public void similarityCalculation(int lineOfQuery, int lines, int columns, double[,] matrix)
        {
            int lineMatrix = 0; //each line in the matrix is a document

            while (lineMatrix < lines - 1)
            {
                double sum = 0;
                for (int j = 0; j < columns; j++)
                {
                    sum += Math.Pow((matrix[lineMatrix, j] - matrix[lineOfQuery, j]), 2);
                }
                this.similarity.Add((double)Math.Sqrt(sum));
                lineMatrix++;
            }
        }

        public void orderRelevantDocuments()
        {
            for(int i = 0; i < this.similarity.Count(); i++)
            {
                this.similarityAndDocName.Add(similarity[i], fileNames[i]);
            }

            this.similarityAndDocName = this.similarityAndDocName.OrderBy(obj => obj.Key).ToDictionary(obj => obj.Key, obj => obj.Value);
        }

    }
}
