using RightFoldPattens.Logging;
using RightFoldPatterns.InterfaceDomain;
using RightFoldPatterns.LogLines;
using System;
using System.Collections.Generic;
using System.Text;

namespace RightFoldPatterns.Stratesy
{// Category を設定する Strategy
    public class SpeedCategoryStrategy : ILineProcessor
    {
        private readonly IEnumerable<ILineProcessor>? _pipeline;

        public SpeedCategoryStrategy(params ILineProcessor[] pipeline)
        {
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        }

        public ILogLine Apply(ILogLine line)
        {
            if (line is CsvLogLine csvLog)
            {
                csvLog.SpeedCategory = csvLog.Speed switch
                {
                    >= 100 => SpeedCategory.Fast,
                    >= 50 => SpeedCategory.Normal,
                    _ => SpeedCategory.Slow // それ以外
                };
                return csvLog;
            }

            // 他の ILogLine 型の場合は変更せず返す
            return line;
        }



        public ILogLine Process(ILogLine line)
        {
            ILogLine current = line;

            if(_pipeline is null)
                throw new InvalidOperationException("Pipeline is not set.");
            foreach (var processor in _pipeline)
                current = processor.Process(current);
            return current;
        }
    }
}
