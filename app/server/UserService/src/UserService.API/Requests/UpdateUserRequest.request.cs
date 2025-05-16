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

    [JsonPropertyName("address")]
    [MaxLength(100)]
    public string? Address { get; set; }
}
