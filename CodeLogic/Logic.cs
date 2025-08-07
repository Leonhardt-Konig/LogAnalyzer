using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
namespace LogAnalyzer.CodeLogic
{
    public class Logic
    {
        
        public static void PrintPath(string? filePath)
        {
             Debug.WriteLine($"{filePath}");
        }
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
        /*public static Dictionary<string, List<int>> MkFilter(string filePath, List<long> byteOffSet)

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
        */
        
        public static List<long> GetByteOffSets(string filePath)
        {
            try
            {
                byte[] buffer = new byte[32768];
                List<long> offSetList = new List<long>();
                long currentOffset = 0;
                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
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

        public static List<int> ArbitrarySearch(List<long> byteOffSet,List<int> storedItems, string searchItem, string filePath)
        {
            //modify function to add items to a list as the user types the terms that are being looked for.
            try
            {
                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                using var sr = new StreamReader(fs);
                {
                    List<int> linesFound = new List<int>();
                
                    for (int i = 0; i < byteOffSet.Count; i++)
                    {
                        if (!(storedItems.Contains(i)))
                        {
                            fs.Seek(byteOffSet[i], SeekOrigin.Begin);
                            string? line = sr.ReadLine();
                            if (line != null && line.Contains(searchItem, StringComparison.OrdinalIgnoreCase))
                            {
                                //Debug.WriteLine($"The arbitrary term: {searchItem} was found in this line:\r\n{line}");
                                linesFound.Add(i);
                            }
                        }
                    }
                    fs.Dispose();
                    sr.Dispose();
                    return linesFound;
                }
                
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}


