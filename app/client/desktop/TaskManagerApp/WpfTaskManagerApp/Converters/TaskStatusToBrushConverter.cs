using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TaskStatus = WpfTaskManagerApp.Core.TaskStatus;

namespace WpfTaskManagerApp.Converters;
public class TaskStatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TaskStatus status)
        {
            switch (status)
            {
                case TaskStatus.ToDo:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F8C8D")); // Xám
                case TaskStatus.InProgress:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB")); // Xanh dương
                case TaskStatus.Done:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71")); // Xanh lá
                case TaskStatus.Cancelled:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C")); // Đỏ
                default:
                    return Brushes.LightGray;
            }
        }
        return Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}