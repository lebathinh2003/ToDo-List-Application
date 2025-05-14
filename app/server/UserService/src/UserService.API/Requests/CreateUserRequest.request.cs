using System.ComponentModel.DataAnnotations;

namespace UserService.API.Requests;

public class CreateUserRequest
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string FullName { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Address { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Password { get; set; } = null!;
}
