using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfTaskManagerApp.Converters;

// Converts string to Visibility.
[ValueConversion(typeof(string), typeof(Visibility))]
public class StringToVisibilityConverter : IValueConverter
{
    // Visible if string is not null/empty.
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !string.IsNullOrEmpty(value as string) ? Visibility.Visible : Visibility.Collapsed;
    }

    // Not implemented.
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}