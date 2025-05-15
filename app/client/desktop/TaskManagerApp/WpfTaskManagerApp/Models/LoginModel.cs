using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WpfTaskManagerApp.Core;

namespace WpfTaskManagerApp.Models;
public class LoginRequestModel
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}

// Model cho Login Response (nhận từ API)
// Giả sử API trả về token và thông tin cơ bản của User
public class LoginResponseModel
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    // Các thông tin user có thể được trả về trực tiếp hoặc lấy từ payload của JWT
    // Hoặc bạn có thể cần gọi một endpoint /api/users/me sau khi login để lấy thông tin chi tiết
    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public UserRole Role { get; set; }

}
