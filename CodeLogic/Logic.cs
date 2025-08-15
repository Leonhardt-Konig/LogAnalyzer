using System.IO;
using Microsoft.Win32;
namespace LogAnalyzer.CodeLogic
{
    public class Logic
    {
        public static string? GetWorkFile()
        {
            var fileDialog = new OpenFileDialog
            {
                Title = "Select a log file.",
                Filter = "Log Files (*.log; *.txt) | *.log; *.txt| All Files (*.*)|*.*",
                CheckFileExists = true,
                Multiselect = false,
            };

            return fileDialog.ShowDialog() == true ? fileDialog.FileName : null;
        }
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


