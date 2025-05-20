using WpfTaskManagerApp.Models;
namespace WpfTaskManagerApp.Interfaces;

// Service for SignalR real-time communication.
public interface ISignalRService
{
    // Connects to the SignalR hub.
    Task ConnectAsync();

    // Disconnects from the SignalR hub.
    Task DisconnectAsync();

    // True if connected to SignalR.
    bool IsConnected { get; }

    // Fired when a new task is assigned.
    event Action<TaskItem>? NewTaskAssigned;

    // Fired when a task's status is updated.
    event Action<TaskItem>? TaskStatusUpdated;

    // Fired when a force logout is requested.
    event Action<string?>? ForceLogoutReceived;

    // Fired when a general reload is requested.
    event Action? ReloadReceived;
}