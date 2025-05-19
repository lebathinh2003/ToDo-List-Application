namespace WpfTaskManagerApp.DTOs;
public class SignalRTaskItemDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid AssigneeId { get; set; }
    public string AssigneeName { get; set; } = string.Empty;
    public string AssigneeUsername { get; set; } = string.Empty;
    public string Status { get; set; } = "ToDo";
    public DateTime CreatedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsActive { get; set; }
}
