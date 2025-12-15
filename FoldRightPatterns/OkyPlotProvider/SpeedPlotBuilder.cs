using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using RightFoldPattens.Logging;
using RightFoldPatterns.InterfaceDomain;
using RightFoldPatterns.LogLines;
using System.Collections.Generic;
using System.Windows.Media.Animation;

namespace RightFoldPatterns.Plotting
{
    public class SpeedPlotBuilder : ILineProcessor
    {

        public SpeedPlotBuilder(params ILineProcessor[] pipeline)
            {


            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        }

        private readonly IEnumerable<ILineProcessor>  _pipeline;

        public PlotModel Build(IEnumerable<ILogLine> lines)
        {
            var model = new PlotModel { Title = "Speed Chart" };

            var category = new CategoryAxis { Position = AxisPosition.Bottom };
            category.Title = "Index";

            var value = new LinearAxis { Position = AxisPosition.Left };
            value.Title = "Speed";

            model.Axes.Add(category);
            model.Axes.Add(value);

            var series = new LineSeries();

            int index = 0;

            foreach (var line in lines)
            {
                if (line is CsvLogLine csv)
                {
                    category.Labels.Add(index.ToString());
                    series.Points.Add(new DataPoint(index, csv.Speed));
                    index++;
                }
            }

            model.Series.Add(series);
            return model;
        }


        public ILogLine Process(ILogLine line)
        {
            ILogLine current = line;
            foreach (var processor in _pipeline)
                current = processor.Process(current);
            return current;
        }

        // ここを追加
        public PlotModel UpdatePlot(IEnumerable<CsvLogLine> lines)
        {
            return Build(lines);
        }
    }
}
