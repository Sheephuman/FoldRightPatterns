using RightFoldPattens.Logging;
using System.IO;
using System.Windows.Shapes;

public static class CsvLazyReader
{
    public static IEnumerable<ILogLine> ReadCsvLazy(string path)
    {
        using var sr = new StreamReader(path);

        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            // 例として "severity,message" の簡易 CSV とする
            var parts = line.Split(',');

            var severity = parts[0] switch
            {
                "debug" => LogSeverity.Debug,
                "info" => LogSeverity.Info,
                "warn" => LogSeverity.Warning,
                "error" => LogSeverity.Error,
                "critical" => LogSeverity.Critical,
                _ => LogSeverity.Info
            };

            yield return new LogLine(parts[1], severity);
        }
    }
          public static IEnumerable<ILogLine> ReadCsvLazy(IEnumerable<string> lines)
    {

        using var enumerator = lines.GetEnumerator(); // IEnumerator を取得
        while (enumerator.MoveNext())                // 次の要素があるか確認
        {
            var line = enumerator.Current;          // 現在の要素を取得
            if (string.IsNullOrWhiteSpace(line)) continue; // 空行はスキップする例

            var parts = line.Split(',');

            var severity = parts[0] switch
            {
                "debug" => LogSeverity.Debug,
                "info" => LogSeverity.Info,
                "warn" => LogSeverity.Warning,
                "error" => LogSeverity.Error,
                "critical" => LogSeverity.Critical,
                _ => LogSeverity.Info
            };

            yield return new LogLine(parts[1], severity);
        }

    }
}
