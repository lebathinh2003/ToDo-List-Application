using WpfTaskManagerApp.Models;

namespace WpfTaskManagerApp.Services;

public interface ISignalRService
{
    Task ConnectAsync();
    Task DisconnectAsync();
    bool IsConnected { get; }

    // Định nghĩa các sự kiện mà ViewModel có thể đăng ký
    event Action<TaskItem>? NewTaskAssigned;
    event Action<TaskItem>? TaskStatusUpdated;
    // Thêm các sự kiện khác nếu cần (ví dụ: TaskDeleted, UserProfileUpdated, etc.)
}
