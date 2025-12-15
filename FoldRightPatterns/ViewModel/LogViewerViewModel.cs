using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using RightFoldPatterns;
using RightFoldPatterns.Implements;
using RightFoldPatterns.InterfaceDomain;
using RightFoldPatterns.LogLines;
using RightFoldPatterns.Plotting;
using RightFoldPatterns.Stratesy;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace RightFoldPattens.ViewModels
{
    public class LogViewerViewModel : BindableBase
    {
        private LazyLineEvaluator_FoldRightNonStrict? _evaluator;

        private List<string> _loadedLines = new();
        private readonly List<CsvLogLine> _csvLogLine = new();

        public IEnumerable<string> CSVlogLines => _loadedLines;

        public ObservableCollection<CsvLogLine> Lines { get; }
            = new ObservableCollection<CsvLogLine>();


        private readonly SpeedPlotBuilder _plotBuilder = new SpeedPlotBuilder();
        private readonly ILineProcessor _viewstrategy;


        public ICommand LoadNextCommand { get; }
        public ICommand LoadAllCommand { get; }
        public ICommand FileSelectorCommand { get; }
        public ICommand ComplexibleConditionCommand { get; }

        private CancellationTokenSource? _cts;
  
        public LogViewerViewModel()
        {
            LoadNextCommand = new DelegateCommand(LoadNext);
            LoadAllCommand = new DelegateCommand(LoadAll);
            FileSelectorCommand = new DelegateCommand(SelectFile);
            ComplexibleConditionCommand = new DelegateCommand(CallExecuteStreaming);



            //   PlotModel = new PlotModel { Title = "Speed Chart" };

            _viewstrategy = new CompositeStrategy(
        new SpeedStrategy(),
        new SpeedCategoryStrategy(),
         new SpeedPlotBuilder()

         );
        }

        public string? DisplayText
        {
            get => field;
            set => SetProperty(ref field, value);
        }


        public PlotModel? _PlotModel
        {
            get => field;

            set {
                // 既存の PlotModel がまだ残っている場合は、新しい PlotModel に差し替えない
                if (value != null && field == null)
                {
                    SetProperty(ref field, value);
                }
                // もしくは必ず新しい PlotModel にしたい場合は
               
            }

        } 





        // -------------------------------------------------------------
        // ファイル読み込み
        // -------------------------------------------------------------
        private void SelectFile()
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".csv",
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                DisplayText = dialog.FileName;

                _loadedLines = LoadCsv(DisplayText);
                _evaluator = LogStrategyBuilder.Build(_loadedLines);

                Lines.Clear();
                _csvLogLine.Clear();
            }
        }

        private List<string> LoadCsv(string path)
        {
            if (string.IsNullOrEmpty(path)) return new();
            return File.ReadLines(path).ToList();
        }


        // -------------------------------------------------------------
        // 右畳み込み：「最初の一致だけ」
        // -------------------------------------------------------------
        private void LoadNext()
        {
            if (_evaluator is null) return;

            _evaluator.EvaluateFirst(
                line =>
                {
                    var f = line.Split(',');
                    return f.Length >= 6 &&
                           int.TryParse(f[5], out int speed) &&
                           speed >= 20;
                },
                line =>
                {
                    var classified = new CsvLogLine(line);
                    Lines.Add(classified);
                }
            );
        }


        // -------------------------------------------------------------
        // 右畳み込み：「条件に合うすべて」
        // -------------------------------------------------------------
        private void LoadAll()
        {
            if (_evaluator is null) return;

            Lines.Clear();

            Func<string, bool> condition = line =>
            {
                var f = line.Split(',');
                return f.Length >= 6 &&
                       int.TryParse(f[5], out int speed) &&
                       speed >= 70;
            };

            Func<string, CsvLogLine> factory = line => new CsvLogLine(line);

            foreach (var log in _evaluator.EvaluateAllRight(condition, factory))
            {
                Lines.Add(new CsvLogLine(log.Message));
            }
        }


        // -------------------------------------------------------------
        // Strategy ベースの複雑条件処理（EvaluateAll を使用）
        // -------------------------------------------------------------
        public async Task<PlotModel> ExecuteStreamingWithPlotModel(
      Func<string, bool> condition,
      Func<string, CsvLogLine> factory,
      Action<CsvLogLine> onItem, CancellationToken token)
        {
            var plotModel = new PlotModel { Title = "Speed Chart" };
            var series = new LineSeries { Title = "Speed" };
            plotModel.Series.Add(series);
            plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "HH:mm:ss" });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Speed" });

            if (_evaluator == null || _viewstrategy == null)
                return plotModel;

            await Task.Run(() =>
            {

                ///入力ストリームを左から消費し、意味付けは右側で確定する
                foreach (var log in _evaluator.EvaluateAllRight(condition, factory))
                {
                    token.ThrowIfCancellationRequested();  // キャンセル判定
                    ///外部から計算全体を切断する。

                    var processed = _viewstrategy.LogProcess(log) as CsvLogLine;
                    if (processed is null)
                        continue;

                    // UI スレッドで処理

                    onItem(processed);
                    series.Points.Add(new DataPoint(DateTimeAxis.ToDouble(processed.Timestamp), processed.Speed));
                    plotModel.InvalidatePlot(false);
                }

            });

            return plotModel;
        }


        ChartWindow? _chartWindow;
        public async void CallExecuteStreaming()
        {
            try
            {

                // 既存の処理があればキャンセル
                if (_cts != null)
                {
                    _cts.Cancel();   // 既存の処理をキャンセル
                 

                }
               
                _cts = new CancellationTokenSource();
                var token = _cts.Token;

                if (_chartWindow != null)
                {
                    _chartWindow?.SpeedPlotView.Model = null;
                    _chartWindow?.Close();
                }

                _chartWindow = new ChartWindow(this);
                _chartWindow.Show();

                ///_chartWindow.Dispose() を呼ぶと Binding が壊れる!


                Application.Current.DispatcherUnhandledException += (sender, e) =>
                {
                    e.Handled = true; // true にするとアプリは終了せずに継続

                    _chartWindow?.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("エラーが発生しましたわ: " + e.Exception.Message);
                        Lines.Clear();
                       
                        return;
                    });
                };

                // Series と PlotModel を準備
                var series = new LineSeries { Title = "Speed" };
              var plotModel = new PlotModel { Title = "Speed Chart" };
                plotModel.Series.Add(series);
                plotModel.Axes.Add(new DateTimeAxis
                {
                    Position = AxisPosition.Bottom,
                    StringFormat = "HH:mm:ss",
                    Title = "Time"
                });
                plotModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Speed"
                });


                Func<string, CsvLogLine> factory = line => new CsvLogLine(line);

                plotModel = await ExecuteStreamingWithPlotModel(
            line =>
            {
                var f = line.Split(',');
                return f.Length >= 6 && int.TryParse(f[5], out int speed) && speed >= 70;
            },
            factory,
            logLine =>
            {

                Lines.Add(logLine); // ViewModel 側の ObservableCollection に追加
                series.Points.Add(new DataPoint(
                DateTimeAxis.ToDouble(logLine.Timestamp),
                logLine.Speed));
                plotModel.InvalidatePlot(false); // 描画更新

            }, token);

            
                _PlotModel = plotModel;





            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("前の処理を女性的にキャンセルしましたわ");
            }
            finally
            {
              
            }

        }

    }
}