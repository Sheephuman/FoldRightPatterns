using RightFoldPattens.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace RightFoldPatterns.LogLines
{
    public class ClassifiedLogLine : ILogLine
    {
        public string Message { get; set; }
        public LogSeverity Severity { get; set; }
        public SpeedCategory SpeedCategory { get; set; }

        public int speed { get; }
        SpeedCategory ILogLine.SpeedCategory { get => SpeedCategory; set => throw new NotImplementedException(); }

        public ClassifiedLogLine(string line)
        {
            Message = line;

        
        }
    }

    public enum SpeedCategory { Slow, Normal, Fast,
        ERROR
    }

}
