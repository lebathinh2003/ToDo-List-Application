using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Models;
namespace WpfTaskManagerApp.Interfaces;

// Service for managing tasks.
public interface ITaskService
{
    // Gets all tasks with pagination, sorting, filtering.
    Task<PaginatedResult<TaskItem>?> GetAllTasksAsync(
        int skip = 0,
        int limit = 10,
        string? sortBy = null,
        string? sortOrder = null,
        string? keyword = null,
        TaskStatusItem? status = null, // Filter by status.
        bool includeInactive = false);

    // Gets tasks by assignee with pagination, sorting, filtering.
    Task<PaginatedResult<TaskItem>?> GetTasksByAssigneeAsync(
        Guid assigneeId,
        int skip = 0,
        int limit = 10,
        string? sortBy = null,
        string? sortOrder = null,
        string? keyword = null,
        TaskStatusItem? status = null); // Filter by status.

    // Gets a specific task by its ID.
    Task<TaskItem?> GetTaskByIdAsync(Guid taskId);

    // Adds a new task.
    Task<TaskItem?> AddTaskAsync(TaskItem task);

    // Updates an existing task.
    Task<bool> UpdateTaskAsync(TaskItem task);

    // Deletes a task (soft delete usually).
    Task<bool> DeleteTaskAsync(Guid taskId);

    // Restores a previously deleted task.
    Task<bool> RestoreTaskAsync(Guid taskId);

    // Updates the status of a task (used by staff).
    Task<bool> UpdateTaskStatusAsync(Guid taskId, Core.TaskStatus newStatus);
}