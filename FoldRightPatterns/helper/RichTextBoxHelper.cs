
namespace RightFoldPatterns.helper
{
    using RightFoldPattens.Logging;
    using RightFoldPatterns.LogLines;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    public static class RichTextBoxHelper
    {
        /// <summary>
        /// 参照があるので消さないこと
        /// </summary>
        /// <param name="box"></param>
        /// <param name="log"></param>
        public static void AppendColoredText(this RichTextBox box, CsvLogLine log)
        {
            Color color = log.Severity switch
            {
                LogSeverity.Error => Colors.Red,
                LogSeverity.Warning => Colors.Orange,
                _ => log.SpeedCategory switch
                {
                    SpeedCategory.Fast => Colors.Green,
                    SpeedCategory.Normal => Colors.Black,
                    SpeedCategory.Slow => Colors.Blue,
                    _ => Colors.Black
                }
            };

            var range = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd)
            {
                Text = log.Message + Environment.NewLine
            };
            range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
        }

        public static void AppendColoredText(this RichTextBox box, CsvLogLine log, string? displayText = null)
        {
            string text = displayText ?? log.Message;

            Color color = log.Severity switch
            {
                LogSeverity.Error => Colors.Red,
                LogSeverity.Warning => Colors.Orange,
                _ => log.SpeedCategory switch
                {
                    SpeedCategory.Fast => Colors.Green,
                    SpeedCategory.Normal => Colors.Black,
                    SpeedCategory.Slow => Colors.Blue,
                    _ => Colors.Black
                }
            };



            // UI スレッドで実行
            box.Dispatcher.Invoke(() =>
            {
                var run = new Run(text)
                {
                    Foreground = new SolidColorBrush(color)
                };

                var paragraph = new Paragraph(run)
                {
                    Margin = new Thickness(0, 0, 0, 2) // 行間調整
                };

                ; /* Collections のアイテムを追加しているだけでは
「毎回 Paragraph を追加する」
という保証はない。
そのため、２回目以降の追加で Run の後ろに続いてしまい、改行なしでくっつく ことが起きる。
                    明示的にParagraphを追加してやれば解決する
                    */


                box.Document.Blocks.Add(paragraph);
            });
        }
    }
}

