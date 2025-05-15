using IndentityService.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IndentityService.Domain.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ApplicationUser> Users { get; }
    DbSet<IdentityUserRole<Guid>> UserRoles { get; }
    DbSet<IdentityRole<Guid>> Roles { get; }
}
