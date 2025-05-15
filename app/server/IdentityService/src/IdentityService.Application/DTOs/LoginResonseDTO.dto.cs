using System.Text.Json.Serialization;
namespace IdentityService.Application.DTOs;

public class LoginResonseDTO
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = null!;
    [JsonPropertyName("userId")]
    public Guid UserId { get; set; } 

    [JsonPropertyName("username")]
    public string Username { get; set; } = null!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("role")]
    public string Role { get; set; } = null!;
}
