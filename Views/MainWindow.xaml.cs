using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
    //TODO-LATER: OPTIMIZE THE RENDERING, DON'T LOAD STRINGS TO RAM?
    //TODO: ADD SEARCH AND FILTERING FUNCTIONS.
    public partial class MainWindow : Window
    {
        private string? filePath = null;
        //viewing logic.
        public ObservableCollection<string> LogEntries { get; set; } = new ObservableCollection<string>();
        public List<long> logOffsetsProcessed = new List<long>();
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
                ;
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
            Debug.WriteLine($"{UserSearchBox.Text}");
            string input = UserSearchBox.Text;
            terms = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
        //have an array/list to put the offsets specific to the arbitrary search
        
        private void Button_Click(object sender, RoutedEventArgs e)
        { 
            for (int i = 0; i < terms.Length; i++)
            {
                List<int> tempSearchList = Logic.ArbitrarySearch(logOffsetsProcessed, terms[i], filePath);
                foreach (int term in tempSearchList) 
                {
                    Debug.WriteLine($"Index {term} for term: {terms[i]}");
                }
            }
            /*
            {
                
                foreach (var integer in tempList)
                {
                    searchItemsFound.Add(integer);
                    tempList.Remove(integer);
                }
            }
            FindFirstLines(searchItemsFound);*/
        }
        
        /*List<int> searchItemsFound = new List<int>();
        public List<long> GetNewOffsets(List<int> indexes)
        {
            //loop over the processed offset list 
            foreach (var index in indexes)
            {
                //loop over offsets, seek to offset and return line, add line to string list.
            }
        }*/
    }
    
}

