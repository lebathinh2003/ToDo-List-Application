using WpfTaskManagerApp.DTOs;
using WpfTaskManagerApp.Models;
using TaskStatus = WpfTaskManagerApp.Core.TaskStatus;

namespace WpfTaskManagerApp.Utils;

// DTO to model mapper.
public static class TaskItemMapper
{
    // SignalR DTO to TaskItem.
    public static TaskItem FromDTO(SignalRTaskItemDTO dto)
    {
        // Parse status string to enum; defaults to ToDo if parse fails.
        Enum.TryParse<TaskStatus>(dto.Status, true, out var parsedStatus); // `true` for ignoreCase.

        return new TaskItem
        {
            Id = dto.Id,
            Code = dto.Code,
            Title = dto.Title,
            Description = dto.Description,
            AssigneeId = dto.AssigneeId,
            AssigneeName = dto.AssigneeName,
            AssigneeUsername = dto.AssigneeUsername,
            Status = parsedStatus, // If TryParse fails, parsedStatus is default(TaskStatus).
            CreatedDate = dto.CreatedDate,
            DueDate = dto.DueDate,
            IsActive = dto.IsActive
        };
    }
}