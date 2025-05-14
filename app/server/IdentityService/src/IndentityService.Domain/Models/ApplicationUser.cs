using Microsoft.AspNetCore.Identity;
namespace IndentityService.Domain.Models;
public class ApplicationUser : IdentityUser<Guid>
{
    public bool IsActive { get; set; }
}