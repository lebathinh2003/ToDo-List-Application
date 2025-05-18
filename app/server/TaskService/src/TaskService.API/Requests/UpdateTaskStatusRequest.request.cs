using System.ComponentModel.DataAnnotations;
namespace TaskService.API.Requests;
public class UpdateTaskStatusRequest
{
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = null!;
}
