using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RightFoldPattens.Logging
{
    public class LogColorizer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LogSeverity severity)
            {
                switch (severity)
                {
                    case LogSeverity.Debug:
                        return Brushes.Gray;

                    case LogSeverity.Info:
                        return Brushes.Black;

                    case LogSeverity.Warning:
                        return Brushes.Orange;

                    case LogSeverity.Error:
                        return Brushes.Red;

                    case LogSeverity.Critical:
                        return new SolidColorBrush(Color.FromRgb(128, 0, 0));

                    default:
                        return Brushes.Black;
                }
            }

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
