using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserService.API.Requests;

public class CreateUserRequest
{
    [Required]
    [MaxLength(50)]
    [JsonPropertyName("username")]
    public string Username { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    [JsonPropertyName("address")]
    public string Address { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;

}
