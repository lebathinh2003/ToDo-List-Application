using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Interfaces;
using Task = TaskService.Domain.Models.Task;
namespace TaskService.Infrastructure.Persistence;
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    public DbSet<Task> Tasks { get; set; }
    public DbContext Instance => this;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Task>()
            .Property(t => t.Status)
            .HasConversion<string>(); 
    }
}
