using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LogAnalyzer.CodeLogic;

namespace LogAnalyzer.Views
{
    //FILTERING WON'T BE INCLUDED ANYMORE
    //OPTIMIZE TEXT RENDERING BY CREATING A "BUFFER" TO SHOW/LOAD A SPECIFIC AMOUNT OF ITEMS 
    //
    public partial class MainWindow : Window
    {
       public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj)
       where T : DependencyObject
       {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
       }
       public static childItem FindVisualChild<childItem>(DependencyObject obj)
       where childItem : DependencyObject
       {
            foreach (childItem child in FindVisualChildren<childItem>(obj))
            {
                return child;
            }
            return null;
       }
        public class ViewState
        {
            //this class will control the state of the ui
            //specifically, the text being shown (setting something or nothing);
        }



        private string? filePath = null;
        private Dictionary<string, List<int>> filterDict = [];
        //viewing logic.
        public ObservableCollection<string> LogEntries { get; set; } = [];
        public List<long> logOffsetsProcessed = [];
        public ObservableCollection<string> RenderedLines { get; set; } = [];
        public async Task FindFirstLines(List<long> OffSetList)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fs);
            foreach (var offset in OffSetList)
            {
                //seek to line and read it, add string to Observable collection
                logOffsetsProcessed.Add(offset);
                fs.Seek(offset, SeekOrigin.Begin);
                string? line = await sr.ReadLineAsync();
                if (line != null)
                {
                    LogEntries.Add(line);
                }
            }
            for (int i = 0; i < 100; i++)
            {
                RenderedLines.Add(LogEntries[i]);
            }
        }
        public async void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            filePath = Logic.GetWorkFile();
            await FindFirstLines(Logic.GetByteOffSets(filePath: filePath));
            
        }

        public void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            filePath = null;
            LogEntries = new ObservableCollection<string>();
            LineView.ItemsSource = LogEntries;
        }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
        public async Task SearchQuery(string query)
        {
            //
            Debug.WriteLine("User has typed.");

        }



        string[] terms;
        private void UserSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LineView.ItemsSource = foundEntries;
            foundEntries.Clear();
            Debug.WriteLine($"{UserSearchBox.Text}");
            string input = UserSearchBox.Text;
            terms = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (terms.Length > 0) 
            {
                List<int> tempSearchList = new List<int>();
                for (int i = 0; i < terms.Length; i++)
                {

                    tempSearchList = Logic.ArbitrarySearch(logOffsetsProcessed, tempSearchList, terms[i], filePath);
                    FindQueryLines(tempSearchList);
                    LineView.ItemsSource = foundEntries;
                }
            }
            else
            {
                LineView.ItemsSource = RenderedLines;
            }
        }
        //have an array/list to put the offsets specific to the arbitrary search
        //change the search to be done with TextChanged instead of a button. 
        //Add a debounce (delay/timer) to count down so that searches aren't done with every key press
        public ObservableCollection<string> foundEntries = [];
        private void FindQueryLines(List<int> lineIdx)
        {
            try
            {
                //use the indexes, seek to those specific lines and read them.
                
                for (int i = 0; i < lineIdx.Count; i++)
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        //modify function to not use indexes (as in 0,1,2 etc) but offsets!
                        fs.Seek(logOffsetsProcessed[lineIdx[i]], SeekOrigin.Begin);
                        string? lineFound = sr.ReadLine();
                        if (lineFound != null) 
                        {                            
                            foundEntries.Add(lineFound);
                        }

                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }
       

        
        public void ShowPosition()
        {
            var scrollViewer = FindVisualChild<ScrollViewer>(LineView);
        }
    }
}

