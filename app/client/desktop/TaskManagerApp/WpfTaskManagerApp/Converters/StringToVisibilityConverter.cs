using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfTaskManagerApp.Converters // Đảm bảo namespace là WpfTaskManagerApp.Converters
{
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Hiển thị nếu string không null và không rỗng
            return !string.IsNullOrEmpty(value as string) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ConvertBack thường không cần thiết cho trường hợp này
            throw new NotImplementedException();
        }
    }
}
