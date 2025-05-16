using System.Text.Json.Serialization;

namespace IdentityService.API.Request;
public class ChangePasswordRequest
{
    [JsonPropertyName("currentPassword")]
    public string CurrentPassword { get; set; } = string.Empty;
    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; } = string.Empty;
}