using Microsoft.EntityFrameworkCore;

namespace UserService.Domain.Interfaces;
public interface IDbContext : IDisposable
{
    DbContext Instance { get; }
}
