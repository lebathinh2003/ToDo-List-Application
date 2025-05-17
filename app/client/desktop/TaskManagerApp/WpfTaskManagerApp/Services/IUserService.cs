using WpfTaskManagerApp.Models;

namespace WpfTaskManagerApp.Services; 
public interface IUserService
{
    Task<PaginatedResult<User>?> GetUsersAsync(int skip = 0, int limit = 10, string? sortBy = null, string? sortOrder = null, string? keyword = null, bool includeInactive = false);
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> AddUserAsync(User user, string password);

    // ***** SỬA ĐỔI AdminUpdateUserAsync ĐỂ NHẬN MẬT KHẨU MỚI (TÙY CHỌN) *****
    Task<bool> AdminUpdateUserAsync(Guid userId, User userToUpdate, string? newPassword = null);
    // ***** KẾT THÚC SỬA ĐỔI *****

    Task<bool> UpdateCurrentUserProfileAsync(User userToUpdate);
    Task<bool> DeleteUserAsync(Guid userId);
    Task<bool> RestoreUserAsync(Guid userId);
    // AdminSetUserPasswordAsync đã được loại bỏ
}