using System.Text.Json.Serialization;

namespace WpfTaskManagerApp.Models;
public class ChangePasswordModel
{
    [JsonPropertyName("currentPassword")]
    public string CurrentPassword { get; set; } = string.Empty;
    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; } = string.Empty;
}

