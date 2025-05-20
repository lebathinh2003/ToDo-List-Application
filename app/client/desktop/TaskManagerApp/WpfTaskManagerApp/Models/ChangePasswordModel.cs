using System.Text.Json.Serialization;

namespace WpfTaskManagerApp.Models;

// Model for changing password.
public class ChangePasswordModel
{
    [JsonPropertyName("currentPassword")]
    public string CurrentPassword { get; set; } = string.Empty;

    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; } = string.Empty;
}