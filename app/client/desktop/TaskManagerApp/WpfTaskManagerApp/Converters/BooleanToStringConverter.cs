using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfTaskManagerApp.Converters;

[ValueConversion(typeof(bool), typeof(string))]
public class BooleanToStringConverter : IValueConverter
{
    // Parameter format: "TrueString|FalseString"
    // Ví dụ: ConverterParameter='Hide Password Section|Change Password'
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
        return string.Empty; // Hoặc giá trị mặc định
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
