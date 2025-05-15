namespace IdentityService.Application.DTOs;
public class AccountDTO
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsActive { get; set; }
    public string Role { get; set; } = null!;

}
