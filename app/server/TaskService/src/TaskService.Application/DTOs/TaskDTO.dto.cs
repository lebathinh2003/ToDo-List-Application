using System.Text.Json.Serialization;

namespace TaskService.Application.DTOs;
public class TaskDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Guid AssigneeId { get; set; }
    public string AssigneeName { get; set; } = null!;
    public string AssigneeUsername { get; set; } = null!;
    public string Status { get; set; } = null!;
    [JsonPropertyName("createdDate")]
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsActive { get; set; }
}
