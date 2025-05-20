using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfTaskManagerApp.Converters;

// Converts boolean to Visibility. Allows inversion.
[ValueConversion(typeof(bool), typeof(Visibility))]
public class BooleanToVisibilityConverter : IValueConverter
{
    // Bool to Visibility. True = Visible, False = Collapsed.
    // Parameter "invert" or "inverted" flips logic.
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool boolValue = false;
        if (value is bool b)
        {
            boolValue = b;
        }

        string? paramString = parameter as string;
        bool inverted = !string.IsNullOrEmpty(paramString) &&
                        (paramString.Equals("invert", StringComparison.OrdinalIgnoreCase) ||
                         paramString.Equals("inverted", StringComparison.OrdinalIgnoreCase));

        if (inverted)
        {
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }
        return boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    // Visibility to bool. Visible = True, Collapsed = False.
    // Parameter "invert" or "inverted" flips logic.
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            string? paramString = parameter as string;
            bool inverted = !string.IsNullOrEmpty(paramString) &&
                            (paramString.Equals("invert", StringComparison.OrdinalIgnoreCase) ||
                             paramString.Equals("inverted", StringComparison.OrdinalIgnoreCase));

            if (inverted)
            {
                return visibility == Visibility.Collapsed;
            }
            return visibility == Visibility.Visible;
        }
        return false; // Default for non-Visibility.
    }
}