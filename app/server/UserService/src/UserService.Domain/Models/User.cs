using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace UserService.Domain.Models;
[Table("Users")]
public class User
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string FullName { get; set; } = default!;
    [Required]
    [MaxLength(100)]
    public string Address { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    [Required]
    public bool IsActive { get; set; } = true;
    [Required]
    public bool IsAdmin { get; set; } = false;
}