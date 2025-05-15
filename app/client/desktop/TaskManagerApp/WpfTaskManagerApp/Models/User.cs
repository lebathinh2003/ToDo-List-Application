using System.Text.Json.Serialization;
using WpfTaskManagerApp.Core;

namespace WpfTaskManagerApp.Models;
public class User
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    // PasswordHash không nên được client quản lý hoặc gửi đi dưới dạng này trong hầu hết các trường hợp.
    // API sẽ xử lý việc hash và lưu trữ mật khẩu.
    // Client chỉ gửi mật khẩu dạng plain text khi tạo user hoặc đổi mật khẩu.
    [JsonIgnore] // Không serialize/deserialize trực tiếp từ/đến API trừ khi API có mục đích đặc biệt
    public string? PasswordHash { get; set; }
    [JsonPropertyName("role")]

    public UserRole Role { get; set; }
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;
    [JsonPropertyName("address")]
    public string? Address { get; set; }
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;

    // Constructor không tham số cần thiết cho deserialization JSON
    public User() { }

    // Constructor cho việc tạo User trong code client (ví dụ, dữ liệu mẫu hoặc khi tạo mới trước khi gửi API)
    // Mật khẩu plain text sẽ được xử lý riêng khi gửi đến API, không lưu trong PasswordHash ở client.
    public User(Guid id, string username, string email, /* string plainPassword (not stored directly), */ UserRole role, string fullName, string? address = null, bool isActive = true)
    {
        Id = id;
        Username = username;
        Email = email;
        Role = role;
        FullName = fullName;
        Address = address;
        IsActive = isActive;
        // PasswordHash sẽ được set bởi backend.
    }
}