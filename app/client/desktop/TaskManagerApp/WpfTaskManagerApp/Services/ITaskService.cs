using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Models;
using TaskStatus = WpfTaskManagerApp.Core.TaskStatus;
namespace WpfTaskManagerApp.Services;
public interface ITaskService
{
    // ***** CẬP NHẬT SIGNATURE CHO Get...TasksAsync *****
    Task<PaginatedResult<TaskItem>?> GetAllTasksAsync(
        int skip = 0,
        int limit = 10,
        string? sortBy = null,
        string? sortOrder = null,
        string? keyword = null,
        TaskStatusItem? status = null, // Thêm filter theo status
        bool includeInactive = false);

    Task<PaginatedResult<TaskItem>?> GetTasksByAssigneeAsync(
        Guid assigneeId,
        int skip = 0,
        int limit = 10,
        string? sortBy = null,
        string? sortOrder = null,
        string? keyword = null,
        TaskStatusItem? status = null);
    // ***** KẾT THÚC CẬP NHẬT SIGNATURE *****

    Task<TaskItem?> GetTaskByIdAsync(Guid taskId);
    Task<TaskItem?> AddTaskAsync(TaskItem task);
    Task<bool> UpdateTaskAsync(TaskItem task);
    Task<bool> DeleteTaskAsync(Guid taskId);
    Task<bool> RestoreTaskAsync(Guid taskId);
    Task<bool> UpdateTaskStatusAsync(Guid taskId, TaskStatus newStatus);
}