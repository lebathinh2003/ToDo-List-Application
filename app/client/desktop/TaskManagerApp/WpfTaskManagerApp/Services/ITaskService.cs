using WpfTaskManagerApp.Models;
using TaskStatus = WpfTaskManagerApp.Core.TaskStatus;
namespace WpfTaskManagerApp.Services;
// --- Task Service ---
public interface ITaskService
{
    Task<IEnumerable<TaskItem>> GetAllTasksAsync(string? searchTerm = null, TaskStatus? status = null, bool includeInactive = false);
    Task<IEnumerable<TaskItem>> GetTasksByAssigneeAsync(Guid assigneeId, string? searchTerm = null, TaskStatus? status = null, bool includeInactive = false);
    Task<TaskItem?> GetTaskByIdAsync(Guid taskId);
    Task<TaskItem?> AddTaskAsync(TaskItem task);
    Task<bool> UpdateTaskAsync(TaskItem task);
    Task<bool> DeleteTaskAsync(Guid taskId);
    Task<bool> UpdateTaskStatusAsync(Guid taskId, TaskStatus newStatus);
}