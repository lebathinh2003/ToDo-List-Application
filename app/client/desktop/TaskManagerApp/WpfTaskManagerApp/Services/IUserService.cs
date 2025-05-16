using WpfTaskManagerApp.Models;

namespace WpfTaskManagerApp.Services;
public interface IUserService
{
    Task<IEnumerable<User>> GetUsersAsync(string? searchTerm = null, bool includeInactive = false);
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> AddUserAsync(User user, string password);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(Guid userId);
}