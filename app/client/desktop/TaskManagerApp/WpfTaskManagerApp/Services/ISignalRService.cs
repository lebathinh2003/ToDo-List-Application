using WpfTaskManagerApp.Models;

namespace WpfTaskManagerApp.Services;
// --- SignalR Service ---
public interface ISignalRService
{
    Task ConnectAsync();
    Task DisconnectAsync();
    bool IsConnected { get; }
    event Action<TaskItem>? NewTaskAssigned; // Staff được gán task mới
    event Action<TaskItem>? TaskStatusUpdated; // Task được cập nhật (có thể dùng chung)
                                               // Thêm các event khác nếu cần, ví dụ: TaskDeleted, TaskCreatedByAdminForSomeoneElse
}