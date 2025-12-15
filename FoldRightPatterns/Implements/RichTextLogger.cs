using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace RightFoldPatterns.Implements
{
    public class RichTextLogger
    {
        private readonly RichTextBox _richTextBox;

        public RichTextLogger(RichTextBox richTextBox)
        {
            _richTextBox = richTextBox;
        }

        public void Log(string line, string color = "Black")
        {
           
                var paragraph = new Paragraph();
                var brushObj = new BrushConverter().ConvertFromString(color);
                var run = new Run(line)
                {
                    Foreground = (brushObj as SolidColorBrush) ?? Brushes.Black
                };
                /*BrushConverter.ConvertFromString の戻り値型は objectだが、
仕様上 null を戻す可能性がある扱いなので、
直接 (SolidColorBrush) にキャストすると、C# の null 許容解析の網に引っかかる。
                 * 
                 * 
                */
                paragraph.Inlines.Add(run);
                _richTextBox.Document.Blocks.Add(paragraph);
            
        }
    }

}
