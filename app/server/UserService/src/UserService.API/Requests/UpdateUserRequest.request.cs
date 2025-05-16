using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserService.API.Requests;

public class UpdateUserRequest
{
    [JsonPropertyName("fullName")]
    [MaxLength(50)]
    public string? FullName { get; set; }

    [JsonPropertyName("email")]
    [MaxLength(50)]
    public string? Email { get; set; }

    [JsonPropertyName("username")]
    [MaxLength(50)]
    public string? Username { get; set; }

    [JsonPropertyName("password")]
    [MaxLength(50)]
    public string? Password { get; set; }

    [JsonPropertyName("address")]
    [MaxLength(100)]
    public string? Address { get; set; }

    [JsonPropertyName("isActive")]
    public bool? IsActive { get; set; }
}
