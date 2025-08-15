using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LogAnalyzer.CodeLogic;

namespace LogAnalyzer.Views
{
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
        private string? filePath = null;
        
        public ObservableCollection<string> LogEntries { get; set; } = [];
        public ObservableCollection<string> RenderedLines { get; set; } = [];
        
        public List<long> logOffsetsProcessed = [];
        public async Task FindFirstLines(List<long> OffSetList)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fs);
            foreach (var offset in OffSetList)
            {
                logOffsetsProcessed.Add(offset);
                fs.Seek(offset, SeekOrigin.Begin);
                string? line = await sr.ReadLineAsync();
                if (line != null)
                {
                    LogEntries.Add(line);
                }
            }
            fs.Dispose();
            sr.Dispose();
            
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
            LogEntries.Clear();
            RenderedLines.Clear();
            logOffsetsProcessed.Clear();
            UserSearchBox.Clear();
        }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        string[] terms;
        private async void UserSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LineView.ItemsSource = foundEntries;
            foundEntries.Clear();
            string input = UserSearchBox.Text;
            terms = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (terms.Length > 0)
            {
                List<int> tempSearchList = new List<int>();
                for (int i = 0; i < terms.Length; i++)
                {

                    tempSearchList = Logic.ArbitrarySearch(logOffsetsProcessed, tempSearchList, terms[i], filePath);
                    await FindQueryLines(tempSearchList);
                    LineView.ItemsSource = foundEntries;
                }
            }
            else
            {
                LineView.ItemsSource = RenderedLines;
            }
        }
        public ObservableCollection<string> foundEntries = [];
        private async Task FindQueryLines(List<int> lineIdx)
        {
            try
            {
                for (int i = 0; i < lineIdx.Count; i++)
                {
                    using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    using var sr = new StreamReader(fs);
                    {
                        fs.Seek(logOffsetsProcessed[lineIdx[i]], SeekOrigin.Begin);
                        string? lineFound = await sr.ReadLineAsync();
                        if (lineFound != null)
                        {
                            foundEntries.Add(lineFound);
                        }
                    }
                    fs.Dispose();
                    sr.Dispose();
                }
                
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private async void LineView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            double lineHeight = (16 * 1.25);
            int firstVisibleLine = (int)(e.VerticalOffset / lineHeight);
            int visibleLines = (int)(e.ViewportHeight / lineHeight);
            int anchor = firstVisibleLine;
            
            if (e.VerticalChange > 0 && firstVisibleLine > (RenderedLines.Count / 2))
            {
                await UpdateBuffer(firstVisibleLine, anchor, (visibleLines * 2));
            }
        }

        private async Task UpdateBuffer(int skipIndex, int control, int takeAmount)
        {
            if (control < 0)
            {
                control = 0;
            }
            
            List<string> controlList = [];
            
            foreach (var entry in LogEntries) 
            {
                if (controlList.Contains(entry)) continue;
                
                if (!(RenderedLines.Contains(entry)))
                {

                    controlList.Add(entry);
                }
            }
            foreach (var item in controlList.Take(takeAmount))
            {
                RenderedLines.Add(item);     
            }
        }

    }
}