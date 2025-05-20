using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfTaskManagerApp.Converters;

// Converts boolean to "Active" or "Inactive" string.
public class BooleanToStatusConverter : IValueConverter
{
    // Converts boolean to "Active" (true) or "Inactive" (false).
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isActive)
        {
            return isActive ? "Active" : "Inactive";
        }
        return string.Empty; // Default for non-boolean.
    }

    // Converts status string back to boolean.
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Not supported.
        throw new NotImplementedException("Reverse conversion (string to boolean) is not supported.");
    }
}