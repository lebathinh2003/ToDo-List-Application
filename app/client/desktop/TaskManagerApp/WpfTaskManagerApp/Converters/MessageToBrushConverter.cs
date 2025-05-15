using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfTaskManagerApp.Converters
{
    [ValueConversion(typeof(string), typeof(Brush))]
    public class MessageToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? message = value as string;
            if (string.IsNullOrEmpty(message))
            {
                return Brushes.Transparent; // Hoặc một màu mặc định nếu không có message
            }

            if (message.ToLower().Contains("success"))
            {
                return Brushes.Green;
            }
            if (message.ToLower().Contains("error") || message.ToLower().Contains("fail"))
            {
                return Brushes.Red;
            }
            // Màu mặc định cho các thông báo khác (ví dụ: thông tin)
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
