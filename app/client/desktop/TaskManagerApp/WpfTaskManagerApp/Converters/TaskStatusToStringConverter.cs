using System;
using System.Globalization;
using System.Windows.Data;
using TaskStatus = WpfTaskManagerApp.Core.TaskStatus;

namespace WpfTaskManagerApp.Converters;

// Converts TaskStatus enum to a display string.
public class TaskStatusToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) // Null represents "All".
        {
            // Use parameter for "All" text, or default to "All".
            return parameter?.ToString() ?? "All";
        }
        if (value is TaskStatus status)
        {
            // Custom display names for statuses.
            switch (status)
            {
                case TaskStatus.ToDo:
                    return "To Do";
                case TaskStatus.InProgress:
                    return "In Progress";
                case TaskStatus.Done:
                    return "Done";
                case TaskStatus.Cancelled:
                    return "Cancelled";
                default:
                    return status.ToString(); // Enum name as fallback.
            }
        }
        return value?.ToString() ?? string.Empty; // General fallback.
    }

    // Not needed for display-only.
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}