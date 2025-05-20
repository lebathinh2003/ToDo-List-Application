using WpfTaskManagerApp.Models; 
namespace WpfTaskManagerApp.Interfaces;

// Service for user authentication.
public interface IAuthenticationService
{
    // Logs in a user.
    Task<User?> LoginAsync(string username, string password);

    // Logs out the current user.
    Task LogoutAsync();

    // Gets the currently logged-in user.
    User? CurrentUser { get; }

    // Checks if a user is authenticated.
    Task<bool> IsUserAuthenticatedAsync();

    // Changes user password.
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordModel changePasswordModel);
}