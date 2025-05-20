using WpfTaskManagerApp.Models;
namespace WpfTaskManagerApp.Interfaces;

// Service for managing users.
public interface IUserService
{
    // Gets users with pagination, sorting, filtering.
    Task<PaginatedResult<User>?> GetUsersAsync(int skip = 0, int limit = 10, string? sortBy = null, string? sortOrder = null, string? keyword = null, bool includeInactive = false);

    // Gets a user by ID.
    Task<User?> GetUserByIdAsync(Guid userId);

    // Gets a user by username.
    Task<User?> GetUserByUsernameAsync(string username);

    // Adds a new user with a password.
    Task<User?> AddUserAsync(User user, string password);

    // Admin updates user, optionally sets new password.
    Task<bool> AdminUpdateUserAsync(Guid userId, User userToUpdate, string? newPassword = null);

    // Current user updates their own profile.
    Task<bool> UpdateCurrentUserProfileAsync(User userToUpdate);

    // Deletes a user.
    Task<bool> DeleteUserAsync(Guid userId);

    // Restores a deleted user.
    Task<bool> RestoreUserAsync(Guid userId);
}