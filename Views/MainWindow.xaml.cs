using System.Collections.ObjectModel;
using System.Diagnostics;
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
    

    public partial class MainWindow : Window

    {
        /*public ObservableCollection<string> LogEntries { get; set; } = new ObservableCollection<string>();
        foreach (var line in loglines) 
        {
            LogEntries.Add(line);
        }*/
        string? filePath = null;
        public void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
                filePath = Logic.GetWorkFile();
        }

        public void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //add a method to empty file path.
            Logic.PrintPath(filePath);
        } 
        public MainWindow()
        {
            InitializeComponent();
            //Logic.LogAnalizerLogic();
            //Need to get the offsets.
        }
        
    }
    
}

