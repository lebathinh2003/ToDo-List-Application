using Microsoft.EntityFrameworkCore;
using Task = TaskService.Domain.Models.Task;
namespace TaskService.Domain.Interfaces;
public interface IApplicationDbContext : IDbContext
{
    public DbSet<Task> Tasks { get; set; }

}
