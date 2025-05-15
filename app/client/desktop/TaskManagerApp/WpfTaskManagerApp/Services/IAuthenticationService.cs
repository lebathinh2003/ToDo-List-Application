using WpfTaskManagerApp.Models;
namespace WpfTaskManagerApp.Services;
public interface IAuthenticationService
{
    Task<User?> LoginAsync(string username, string password);
    Task LogoutAsync();
    User? CurrentUser { get; }
    Task<bool> IsUserAuthenticatedAsync();
}