using WpfTaskManagerApp.DTOs;
using WpfTaskManagerApp.Models;
using TaskStatus = WpfTaskManagerApp.Core.TaskStatus;

namespace WpfTaskManagerApp.Utils;

public static class TaskItemMapper
{
    public static TaskItem FromDTO(SignalRTaskItemDTO dto)
    {
        return new TaskItem
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            AssigneeId = dto.AssigneeId,
            AssigneeName = dto.AssigneeName,
            AssigneeUsername = dto.AssigneeUsername,
            Status = Enum.TryParse<TaskStatus>(dto.Status, out var status)
                        ? status
                        : TaskStatus.ToDo,
            CreatedDate = dto.CreatedDate,
            DueDate = dto.DueDate,
            IsActive = dto.IsActive
        };
    }
}
