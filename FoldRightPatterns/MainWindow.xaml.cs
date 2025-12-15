using RightFoldPattens.ViewModels;
using RightFoldPatterns.helper;
using RightFoldPatterns.Implements;
using RightFoldPatterns.InterfaceDomain;
using RightFoldPatterns.LogLines;
using RightFoldPatterns.Stratesy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace RightFoldPattens
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SpeedCategoryStrategy _speedStrategy = new SpeedCategoryStrategy();
        LazyLineEvaluator_FoldRightNonStrict _evaluator;
      
        public MainWindow()
        {
            InitializeComponent();
           var _logViewModel =  new LogViewerViewModel();
            DataContext = _logViewModel;

                _evaluator = new LazyLineEvaluator_FoldRightNonStrict(_logViewModel.CSVlogLines);
            


            _logViewModel.Lines.CollectionChanged += (s, e) =>
            {
                if (e.NewItems == null) return;
               
            

            foreach (CsvLogLine log in e.NewItems)
                {

                    // Strategy を通してカテゴリを設定
                    var processed = (CsvLogLine)_speedStrategy.Apply(log);

                    // 表示文字列にカテゴリを付与
                    var displayText = $"{processed.Message} : {processed.SpeedCategory}";

                 
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // RichTextBox に色付けして追加
                    //    LogBox.AppendColoredText(processed, displayText);


                        LogBox.AppendColoredText(log, $"{log.Message} : {log.SpeedCategory}");
                    });
                }
            };




        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
        LogBox.Document.Blocks.Clear();

        }

        

    }
}