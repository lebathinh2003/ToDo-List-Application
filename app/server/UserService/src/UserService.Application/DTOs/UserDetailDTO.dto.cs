using System.Text.Json.Serialization;

namespace UserService.Application.DTOs;
public class UserDetailDTO
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = null!;
    [JsonPropertyName("username")]
    public string Username { get; set; } = null!;
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;
    [JsonPropertyName("address")]
    public string Address { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
    [JsonPropertyName("role")]
    public string Role { get; set; } = null!;

}
