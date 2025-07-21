using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
//TODO: WORK WITH THE LOGIC AND MODIFY IT TO WORK WITH WPF;

namespace LogAnalyzer.CodeLogic
{
    public class Logic
    {
        
        public static void PrintPath(string? filePath)
        {
             Debug.WriteLine($"{filePath}");
        }


        /*public static void LogAnalizerLogic()
        {
            Debug.WriteLine("Hello");
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                foreach (var index in ArbitrarySearch(byteOffSet, "this is red", filePath))
                {
                    fs.Seek(byteOffSet[index], SeekOrigin.Begin);
                    string? line = sr.ReadLine();
                    Console.WriteLine($"{index} | {line}");
                }
            }
        }*/

        public static string? GetWorkFile()
        {
            var fileDialog = new OpenFileDialog
            {
                Title = "Select a log file.",
                Filter = "Log Files (*.log; *.txt) | *.log; *.txt| All files (*.*)|*.*",
                CheckFileExists = true,
                Multiselect = false,
            };

            return fileDialog.ShowDialog() == true ? fileDialog.FileName : null;
        }
        //just the file's name.
        //string fileName = fileDialog.SafeFileName


        private static Dictionary<string, List<int>> MkFilter(string filePath, List<long> byteOffSet)

        {
            var keywordMap = new Dictionary<string, List<int>> {

            { "ERROR", new List<int>() {}},
            { "DEBUG", new List<int>() {}},
            { "FATAL", new List<int>() {}},
            { "WARNING", new List<int>() {}},
            { "INFO", new List<int>() {}},
            };
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                try
                {
                    for (int i = 0; i < byteOffSet.Count; i++)
                    {
                        fs.Seek(byteOffSet[i], SeekOrigin.Begin);
                        string? line = sr.ReadLine();
                        foreach (string key in Keywords)
                        {
                            if (line != null && line.Contains(key))
                            {
                                keywordMap[key].Add(i);
                            }
                        }
                    }
                    return keywordMap;
                }
                catch (System.Exception)
                {

                    throw;
                }
            }
        }

        private static HashSet<string> Keywords = new HashSet<string> { "ERROR", "DEBUG", "FATAL", "INFO", "WARNING" };
        public static List<long> GetByteOffSets(string filePath)
        {
            try
            {
                byte[] buffer = new byte[32768];
                List<long> offSetList = new List<long>();
                long currentOffset = 0;
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    int bytesRead;
                    offSetList.Add(0);
                    while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        for (int i = 0; i < bytesRead; i++)
                        {
                            if (buffer[i] == 13 && i + 1 < buffer.Length && buffer[i + 1] == 10)
                            {
                                i++;

                                currentOffset += 2;
                                offSetList.Add(currentOffset);
                            }
                            else
                            {
                                currentOffset += 1;
                            }
                        }
                    }
                    fs.Dispose();
                    return offSetList;
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public static List<int> ArbitrarySearch(List<long> byteOffSet, string searchItem, string filePath)
        {
            //modify function to add items to a list as the user types the terms that are being looked for.
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                List<int> linesFound = new List<int>();
                try
                {
                    for (int i = 0; i < byteOffSet.Count; i++)
                    {
                        fs.Seek(byteOffSet[i], SeekOrigin.Begin);
                        string? line = sr.ReadLine();
                        if (line != null && line.Contains(searchItem, StringComparison.OrdinalIgnoreCase))
                        {
                            //Console.WriteLine($"The arbitrary term: {searchItem} was found in this line:\r\n{line}");
                            linesFound.Add(i);
                        }
                    }
                    return linesFound;
                }
                catch (System.Exception)
                {
                    throw;
                }
            }
        }


    }
}


