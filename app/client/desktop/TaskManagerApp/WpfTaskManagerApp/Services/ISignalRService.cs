using WpfTaskManagerApp.Models;

namespace WpfTaskManagerApp.Services;
// --- SignalR Service ---
public interface ISignalRService
{
    Task ConnectAsync();
    Task DisconnectAsync();
    bool IsConnected { get; }
    event Action<TaskItem>? NewTaskAssigned;
    event Action<TaskItem>? TaskStatusUpdated;
}