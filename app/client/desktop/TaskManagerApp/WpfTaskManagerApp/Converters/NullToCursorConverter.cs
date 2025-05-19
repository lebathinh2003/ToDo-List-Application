using System.Globalization;
using System.Windows.Data;
using Cursor = System.Windows.Input.Cursor;
using CursorConverter = System.Windows.Input.CursorConverter;
using Cursors = System.Windows.Input.Cursors;

namespace WpfTaskManagerApp.Converters
{
    public class NullToCursorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Nếu value (AssociatedTask) là null, cursor sẽ là giá trị trong parameter (Arrow)
            // Nếu value không null (có task để click), cursor sẽ là Hand

            string defaultCursorName = parameter as string ?? "Arrow";
            Cursor? defaultCursor = Cursors.Arrow;

            try
            {
                defaultCursor = (Cursor?)new CursorConverter().ConvertFromString(defaultCursorName);
            }
            catch
            {
                // fallback to Arrow if conversion fails
                defaultCursor = Cursors.Arrow;
            }

            return value != null ? Cursors.Hand : defaultCursor ?? Cursors.Arrow;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
