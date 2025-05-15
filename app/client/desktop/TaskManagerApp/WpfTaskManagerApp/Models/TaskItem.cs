using TaskStatus = WpfTaskManagerApp.Core.TaskStatus;
namespace WpfTaskManagerApp.Models;
// Model cho Công việc
public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public string? AssigneeUsername { get; set; }
    public TaskStatus Status { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }

    // Constructor không tham số
    public TaskItem() { }

    public TaskItem(Guid id, string title, string description, TaskStatus status = TaskStatus.ToDo, bool isActive = true)
    {
        Id = id;
        Title = title;
        Description = description;
        Status = status;
        IsActive = isActive;
        CreatedDate = DateTime.UtcNow;
    }
}
