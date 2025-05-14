namespace UserService.Application.DTOs;
public class UserDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsAdmin { get; set; }

}
