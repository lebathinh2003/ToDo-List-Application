using System.ComponentModel.DataAnnotations;
namespace TaskService.API.Requests;
public class UpdateTaskRequest
{
    [Required]
    public Guid AssigneeId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Description { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = null!;

    [Required]
    public bool IsActive { get; set; }

    public DateTime? DueDate { get; set; } = null;
}
