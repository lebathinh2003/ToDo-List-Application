namespace WpfTaskManagerApp.DTOs;

// DTO for task items received via SignalR.
public class SignalRTaskItemDTO
{
    // Task ID.
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    // ID of the assigned user.
    public Guid AssigneeId { get; set; }
    public string AssigneeName { get; set; } = string.Empty;
    public string AssigneeUsername { get; set; } = string.Empty;
    public string Status { get; set; } = "ToDo";
    // Date task was created.
    public DateTime CreatedDate { get; set; }
    // Task due date (nullable).
    public DateTime? DueDate { get; set; }
    // Indicates if the task is active.
    public bool IsActive { get; set; }
}