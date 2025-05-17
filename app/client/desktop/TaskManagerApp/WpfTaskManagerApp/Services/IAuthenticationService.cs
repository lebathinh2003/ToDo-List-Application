using WpfTaskManagerApp.Models;
namespace WpfTaskManagerApp.Services;
// --- Authentication Service ---
public interface IAuthenticationService
{
    Task<User?> LoginAsync(string username, string password);
    Task LogoutAsync();
    User? CurrentUser { get; }
    Task<bool> IsUserAuthenticatedAsync();
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordModel changePasswordModel);
}