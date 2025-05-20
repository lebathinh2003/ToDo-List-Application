using System;
using System.Globalization;
using System.Windows.Data;
using Cursor = System.Windows.Input.Cursor;
using CursorConverter = System.Windows.Input.CursorConverter;
using Cursors = System.Windows.Input.Cursors;

namespace WpfTaskManagerApp.Converters;

// Converts null/non-null to a Cursor. Non-null -> Hand; Null -> parameter-defined or Arrow.
public class NullToCursorConverter : IValueConverter
{
    // Converts object's null state to Cursor.
    // Non-null: Hand. Null: Parameter or Arrow.
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string defaultCursorName = parameter as string ?? "Arrow";
        Cursor? defaultCursor = Cursors.Arrow;

        try
        {
            if (new CursorConverter().ConvertFromString(defaultCursorName) is Cursor convertedCursor)
            {
                defaultCursor = convertedCursor;
            }
        }
        catch
        {
            // Fallback on error.
            defaultCursor = Cursors.Arrow;
        }

        return value != null ? Cursors.Hand : defaultCursor ?? Cursors.Arrow;
    }

    // Converts Cursor back to object (not supported).
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // One-way.
        throw new NotImplementedException("Reverse conversion (Cursor to object) is not supported.");
    }
}