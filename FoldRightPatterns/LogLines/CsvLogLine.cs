using RightFoldPattens.Logging;

namespace RightFoldPatterns.LogLines
{
    public class CsvLogLine : ILogLine
    {
        private readonly string _line;
        private SpeedCategory _category;


        // X軸用
        public DateTime Timestamp { get; }


        public CsvLogLine(string line)
        {
            _line = line;

            var fields = line.Split(',');
            Speed = int.TryParse(fields[5], out var s) ? s : 0;


            if (DateTime.TryParse(fields[2], out var ts))
                Timestamp = ts;
            else
                Timestamp = DateTime.MinValue;


            // 条件に合わせて SpeedCategory と Severity を設定
            Category = Speed >= 60 ? SpeedCategory.Fast :
                       Speed <= 20 ? SpeedCategory.ERROR :  // 20 以下を Error に
                       Speed <= 40 ? SpeedCategory.Slow :
                                     SpeedCategory.Normal;

            Severity = Speed >= 60 ? LogSeverity.Warning :
                       Speed <= 20 ? LogSeverity.Error :
                       LogSeverity.Normal;
        }

        public string Message => _line;
        public int Speed { get;set;   }


        public SpeedCategory Category { get; set; }
        public LogSeverity Severity { get; set; }
        public SpeedCategory SpeedCategory
        {
            get => _category;
            set => _category = value;
        }
    }
}
