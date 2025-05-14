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
    public string UserName { get; set; } = default!;
    [Required]
    [MaxLength(50)]
    public string Email { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    [Required]
    public string PasswordHash { get; set; } = null!;
    [Required]
    public bool IsActive { get; set; }
}