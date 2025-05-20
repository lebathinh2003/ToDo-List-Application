using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TaskStatus = WpfTaskManagerApp.Core.TaskStatus; // Alias for clarity

namespace WpfTaskManagerApp.Converters;

// Converts TaskStatus enum to a specific Brush.
public class TaskStatusToBrushConverter : IValueConverter
{
    // Converts TaskStatus to Brush.
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TaskStatus status)
        {
            switch (status)
            {
                case TaskStatus.ToDo:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F8C8D")); // Gray
                case TaskStatus.InProgress:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB")); // Blue
                case TaskStatus.Done:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71")); // Green
                case TaskStatus.Cancelled:
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C")); // Red
                default:
                    return Brushes.LightGray; // Default color.
            }
        }
        return Brushes.Transparent; // Fallback for invalid input.
    }

    // Not implemented.
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}