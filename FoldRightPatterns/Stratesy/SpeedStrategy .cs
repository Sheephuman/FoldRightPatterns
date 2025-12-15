using RightFoldPattens.Logging;
using RightFoldPatterns.InterfaceDomain;
using RightFoldPatterns.LogLines;
using System;
using System.Collections.Generic;
using System.Text;

namespace RightFoldPatterns.Stratesy
{

    public interface ILogStrategy : ILineProcessor
    {
        ILogLine Apply(ILogLine line);
       
    }

    // Speed を設定する Strategy
    public class SpeedStrategy : ILineProcessor
    {


        private readonly IEnumerable<ILineProcessor> _pipeline;
        public SpeedStrategy(params ILineProcessor[] pipeline)
        {
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        }

        public ILogLine Apply(ILogLine line)
        {
            var csv = (CsvLogLine)line;
            csv.Speed = ParseSpeed(csv.Message);
            return csv;
        }

        public int ParseSpeed(string line)
        {
            var f = line.Split(',');
            return int.TryParse(f[5], out var s) ? s : 0;
        }

        public ILogLine LogProcess(ILogLine line)
        {
            ILogLine current = line;
            foreach (var processor in _pipeline)
                current = processor.LogProcess(current);
            return current;
        }
    }

}
