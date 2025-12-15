using RightFoldPattens.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace RightFoldPatterns.Implements
{
    public class SimpleLogLine :LogLine, ILogLine
    {
        public SimpleLogLine(string message, LogSeverity severity):base(message, severity)
        {
            _message = message;
            _severity = severity;
        }

        public string _message { get; }
        public LogSeverity _severity { get; }

       
    }
}
