using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfTaskManagerApp.Converters;

// Converts status (boolean) to a Brush.
public class StatusToBrushConverter : IValueConverter
{
    // Converts bool to Brush: true -> Green, false -> Red.
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isActive)
        {
            return isActive ? Brushes.Green : Brushes.Red;
        }
        return Brushes.Black; // Default.
    }

    // Converts Brush to bool (not supported).
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}