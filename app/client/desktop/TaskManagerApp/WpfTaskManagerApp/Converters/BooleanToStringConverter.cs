using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfTaskManagerApp.Converters;

// Bool to string converter.
[ValueConversion(typeof(bool), typeof(string))]
public class BooleanToStringConverter : IValueConverter
{
    // Bool to string. Uses "TrueString|FalseString" parameter.
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is string paramString)
        {
            string[] parts = paramString.Split('|');
            if (parts.Length == 2)
            {
                return boolValue ? parts[0] : parts[1];
            }
        }
        return string.Empty; // Default or error.
    }

    // String to bool (not supported).
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // One-way.
        throw new NotImplementedException("Reverse conversion (string to boolean) is not supported.");
    }
}