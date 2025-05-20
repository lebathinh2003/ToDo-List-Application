using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TaskService.Domain.Models;
[Table("Tasks")]
public class Task
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = null!;
    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = null!;
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = null!;
    [Required]
    public Guid AssigneeId { get; set; }
    [Required]
    public TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    [Required]
    public bool IsActive { get; set; }
}
public enum TaskStatus
{
    ToDo,
    InProgress,
    Done,
    Cancelled
}