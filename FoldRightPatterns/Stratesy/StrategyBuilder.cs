using RightFoldPatterns.Implements;
using RightFoldPatterns.LogLines;
using System;
using System.Collections.Generic;
using System.Text;

namespace RightFoldPatterns.Stratesy
{
    public static class LogStrategyBuilder
    {
        public static LazyLineEvaluator_FoldRightNonStrict Build(List<string> lines)
        {
            var eval = new LazyLineEvaluator_FoldRightNonStrict(
                lines             
            );

            eval.AddStrategy(
                line =>
                {
                    var f = line.Split(',');
                    return f.Length >= 6 && int.TryParse(f[5], out var sp) && sp >= 75;
                },
                line => new CsvLogLine(line)
            );

            eval.AddStrategy(
                line =>
                {
                    var f = line.Split(',');
                    return f.Length >= 6 && int.TryParse(f[5], out var sp) && sp <= 40;
                },
                line => new CsvLogLine(line)
            );

            eval.AddStrategy(
                line => line.Contains("ERROR", StringComparison.OrdinalIgnoreCase),
                line => new CsvLogLine(line)
            );

            eval.AddStrategy(
                line => true,
                line => new CsvLogLine(line)
            );

            return eval;
        }
    }

}
