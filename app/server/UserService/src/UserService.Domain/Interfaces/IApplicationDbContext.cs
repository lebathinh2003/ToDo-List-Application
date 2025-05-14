using Microsoft.EntityFrameworkCore;
using UserService.Domain.Models;
namespace UserService.Domain.Interfaces;
public interface IApplicationDbContext
{
    public DbSet<User> Users { get; set; }


}
