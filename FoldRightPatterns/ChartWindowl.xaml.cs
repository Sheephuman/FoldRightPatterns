using OxyPlot;
using RightFoldPattens.ViewModels;
    using RightFoldPatterns.LogLines;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    namespace RightFoldPatterns
    {
        /// <summary>
        /// ChartWindowxaml.xaml の相互作用ロジック
        /// </summary>
        public partial class ChartWindow : Window,IDisposable
        {
            public ObservableCollection<CsvLogLine> Series { get; } = new ();


        LogViewerViewModel _viewModel = new();

            public ChartWindow()
            {
                InitializeComponent();

                DataContext = _viewModel ;

            }


        public ChartWindow(LogViewerViewModel vm)
        {
            InitializeComponent();
         

            _viewModel = vm;
            DataContext = _viewModel;
        }

        internal void SetPlotModel(PlotModel plotModel)
        {
            SpeedPlotView.Model = plotModel;
        }

        private bool _disposed = false;
        PlotModel _plotmodel;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // ファイナライザを抑制
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // 管理リソースの解放
                // 例: イベント解除、Timer停止、PlotModel破棄など
                SpeedPlotView.Model?.Series.Clear();
                SpeedPlotView.Model = null;
            }

            // アンマネージリソースの解放があればここに書く

            _disposed = true;
        }

        ~ChartWindow()
        {
            Dispose(false);
        }
    }
    }
