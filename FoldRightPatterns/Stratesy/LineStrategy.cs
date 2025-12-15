using RightFoldPattens.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace RightFoldPatterns.Stratesy
{
    // Strategy を表す小さな型
    public sealed class LineStrategy
    {
        public Func<string, bool> Condition { get; }
        public Func<string, ILogLine> Factory { get; }

        public LineStrategy(Func<string, bool> condition, Func<string, ILogLine> factory)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
    }

}
