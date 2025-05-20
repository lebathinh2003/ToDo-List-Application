using System;
using System.Text.Json.Serialization;
using WpfTaskManagerApp.Core;

namespace WpfTaskManagerApp.Models;

// Login request model.
public class LoginRequestModel
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}

// Login response model.
public class LoginResponseModel
{
    // Authentication token.
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public UserRole Role { get; set; }
}