using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfTaskManagerApp.Converters;

// Converts message string to a Brush based on keywords.
[ValueConversion(typeof(string), typeof(Brush))]
public class MessageToBrushConverter : IValueConverter
{
    // Converts message string to Brush.
    // "success" -> Green, "error"/"fail" -> Red, else -> Black.
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string? message = value as string;
        if (string.IsNullOrEmpty(message))
        {
            return Brushes.Transparent; // Default for empty.
        }

        string lowerMessage = message.ToLowerInvariant();

        if (lowerMessage.Contains("success"))
        {
            return Brushes.Green;
        }
        if (lowerMessage.Contains("error") || lowerMessage.Contains("fail"))
        {
            return Brushes.Red;
        }
        // Default color.
        return Brushes.Black;
    }

    // Converts Brush back to string (not supported).
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // One-way.
        throw new NotImplementedException("Reverse conversion (Brush to string) is not supported.");
    }
}