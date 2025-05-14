using Microsoft.EntityFrameworkCore;
namespace TaskService.Domain.Interfaces;
public interface IDbContext : IDisposable
{
    DbContext Instance { get; }
}
