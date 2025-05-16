using WpfTaskManagerApp.Models;

namespace WpfTaskManagerApp.Services;
public interface IUserService
{
    // ***** CẬP NHẬT SIGNATURE CỦA GetUsersAsync *****
    Task<PaginatedResult<User>?> GetUsersAsync(
        int skip = 0,
        int limit = 10,
        string? sortBy = null,
        string? sortOrder = null, // "asc" hoặc "desc"
        string? keyword = null,
        bool includeInactive = false);
    // ***** KẾT THÚC CẬP NHẬT SIGNATURE *****

    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> AddUserAsync(User user, string password);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(Guid userId); // Sẽ được dùng cho soft delete
    Task<bool> RestoreUserAsync(Guid userId); // Thêm phương thức Restore
}