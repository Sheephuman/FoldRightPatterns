using RightFoldPatterns.InterfaceDomain;
using RightFoldPatterns.LogLines;
using System;
using System.Collections.Generic;
using System.Text;


namespace RightFoldPattens.Logging
{
    public interface ILogLine 
    {
        string Message { get; }
        LogSeverity Severity { get; }

        SpeedCategory SpeedCategory { get; set; } // 追加
    }

    public class LogLine: ILogLine
    {
        private readonly IEnumerable<ILineProcessor>? _pipeline;
        internal SpeedCategory _category;

        public LogLine(IEnumerable<ILineProcessor>? pipeline = null)
        {
            _pipeline = pipeline;
        }


        public int ParseSpeed(string line)
        {
            var fields = line.Split(',');
            if (fields.Length < 6) return -1;

            return int.TryParse(fields[5], out var speed)
                ? speed
                : -1;
        }

        private readonly string _message = string.Empty;
        public string Message { get; }
        public LogSeverity Severity { get; }
        public SpeedCategory SpeedCategory
        {
            get => _category;
            set => _category = value;
        }

        public LogLine(string message, LogSeverity severity)
        {
            Message = message;
            Severity = severity;
        }
        public override string ToString()
        {
            return Message;   // 必要なら Parse して整形
        }

        public ILogLine Process(ILogLine line)
        {
            ILogLine current = line;
            if(_pipeline is null)
                throw new InvalidOperationException("Pipeline is not set.");
            foreach (var processor in _pipeline)
                current = processor.LogProcess(current);
            return current;
        }
    }


    internal sealed class SlowLogLine : LogLine{
        private readonly string _line;
        public SlowLogLine(string line): base(string.Empty, LogSeverity.Warning)        
        {
            _line = line;
        }

     
        public int Speed => ParseSpeed(_line);


    }

    internal sealed class NormalLogLine  : LogLine
    {
        private readonly string _line;
        public NormalLogLine(string line): base(string.Empty, LogSeverity.Warning)        
        { _line = line;
        }
  
    }

    internal sealed class FastLogLine: LogLine
    {
        private readonly string _line;
        public FastLogLine(string line) : base(string.Empty, LogSeverity.Warning)
        { _line = line; }

    }
}

