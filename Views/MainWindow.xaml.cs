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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    //TODO: OPTIMIZE THE RENDERING, DON'T LOAD STRINGS TO RAM.
    public partial class MainWindow : Window

    {
        private string? filePath = null;
        public ObservableCollection<string> LogEntries { get; set; } = new ObservableCollection<string>();
        public async Task FindFirstLines(List<long> OffSetList)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(fs);
            foreach (var offset in OffSetList)
            {
                //seek to line and read it, add string to Observable collection
                fs.Seek(offset, SeekOrigin.Begin);
                string? line = await sr.ReadLineAsync();
                if (line != null)
                {
                    LogEntries.Add(line);
                };
            }
            
        }
        
        public async void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            filePath = Logic.GetWorkFile();
            await FindFirstLines(Logic.GetByteOffSets(filePath: filePath));
        }

        public void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //add a method to empty file path.
            Logic.PrintPath(filePath);
            
        } 
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            //Logic.LogAnalizerLogic();
            //Need to get the offsets.
        }
        
    }
    
}

