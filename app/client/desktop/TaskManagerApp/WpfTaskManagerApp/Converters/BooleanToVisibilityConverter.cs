using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace WpfTaskManagerApp.Converters;

[ValueConversion(typeof(bool), typeof(Visibility))]
public class BooleanToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean value to a Visibility value.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="targetType">The type of the binding target property (Visibility).</param>
    /// <param name="parameter">An optional parameter. If "invert" or "inverted", the logic is inverted.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>Visibility.Visible if true (or false if inverted), otherwise Visibility.Collapsed.</returns>
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

    /// <summary>
    /// Converts a Visibility value back to a boolean value.
    /// </summary>
    /// <param name="value">The Visibility value to convert.</param>
    /// <param name="targetType">The type of the binding target property (bool).</param>
    /// <param name="parameter">An optional parameter. If "invert" or "inverted", the logic is inverted.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>True if Visibility.Visible (or false if inverted), otherwise false.</returns>
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
        return false;
    }
}
