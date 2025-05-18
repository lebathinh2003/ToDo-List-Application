using System.Globalization;
using System.Windows.Data;
using TaskStatus = WpfTaskManagerApp.Core.TaskStatus;

namespace WpfTaskManagerApp.Converters;
public class TaskStatusToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) // Nếu giá trị là null (đại diện cho "All")
        {
            // Kiểm tra parameter để xem có muốn hiển thị chữ khác không, ví dụ "Tất cả trạng thái"
            return parameter?.ToString() ?? "All";
        }
        if (value is TaskStatus status)
        {
            // Tùy chỉnh cách hiển thị tên trạng thái nếu cần
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
                    return status.ToString(); // Trả về tên enum nếu không có case nào khớp
            }
        }
        return value?.ToString() ?? string.Empty; // Fallback
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Không cần thiết cho ComboBox chỉ hiển thị
        throw new NotImplementedException();
    }
}