using WpfTaskManagerApp.Models;

namespace WpfTaskManagerApp.Services;
// --- User Service ---
public interface IUserService
{
    Task<PaginatedResult<User>?> GetUsersAsync(int skip = 0, int limit = 10, string? sortBy = null, string? sortOrder = null, string? keyword = null, bool includeInactive = false);
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> AddUserAsync(User user, string password);
    Task<bool> AdminUpdateUserAsync(Guid userId, User userToUpdate);
    Task<bool> UpdateCurrentUserProfileAsync(User userToUpdate);
    Task<bool> DeleteUserAsync(Guid userId);
    Task<bool> RestoreUserAsync(Guid userId);
}