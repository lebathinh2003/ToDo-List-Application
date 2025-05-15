using System.Diagnostics;
using System.Windows.Input;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.Services;
using WpfTaskManagerApp.ViewModels.Common;

namespace WpfTaskManagerApp.ViewModels;
public class LoginViewModel : ViewModelBase
{
    private readonly IAuthenticationService? _authenticationService;

    private string _username = string.Empty;
    public string Username
    {
        get => _username;
        set { if (SetProperty(ref _username, value)) (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set { if (SetProperty(ref _password, value)) (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    private bool _isLoggingIn;
    public bool IsLoggingIn
    {
        get => _isLoggingIn;
        set { if (SetProperty(ref _isLoggingIn, value)) (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    public ICommand LoginCommand { get; }
    public Action<User>? LoginSuccessAction { get; set; }

    public LoginViewModel()
    {
        _authenticationService = null;
        LoginCommand = new RelayCommand(
            async (_) => await Task.CompletedTask,
            (_) =>
            {
                bool canLoginDesignTime = !IsLoggingIn &&
                               !string.IsNullOrWhiteSpace(Username) &&
                               !string.IsNullOrWhiteSpace(Password) &&
                               _authenticationService != null;
                return canLoginDesignTime;
            }
        );

        if (IsInDesignModeStatic())
        {
            Username = "designer";
            Password = "password";
            ErrorMessage = "This is a design-time error message.";
            IsLoggingIn = false;
        }
    }

    public LoginViewModel(IAuthenticationService authenticationService, INavigationService navigationService)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

        LoginCommand = new RelayCommand(
            async (_) => await LoginAsync(),
            (_) =>
            {
                bool canLoginRuntime = !IsLoggingIn &&
                               !string.IsNullOrWhiteSpace(Username) &&
                               !string.IsNullOrWhiteSpace(Password);
                return canLoginRuntime;
            }
        );
    }

    private static bool IsInDesignModeStatic()
    {
        return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;
    }


    private async Task LoginAsync()
    {
        if (_authenticationService == null)
        {
            ErrorMessage = "Authentication service is not available.";
            return;
        }
        if (string.IsNullOrEmpty(Password))
        {
            ErrorMessage = "Password is required.";
            (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
            return;
        }

        IsLoggingIn = true;
        ErrorMessage = null;
        try
        {
            Debug.WriteLine("LoginViewModel.LoginAsync: Attempting to login with API...");
            var user = await _authenticationService.LoginAsync(Username, this.Password);
            Debug.WriteLine($"LoginViewModel.LoginAsync: API call returned. User IsNull? {user == null}");
            if (user != null)
            {
                Debug.WriteLine($"LoginViewModel.LoginAsync: Login successful. User: {user.Username}. Checking if LoginSuccessAction is null: {LoginSuccessAction == null}");
                if (LoginSuccessAction != null)
                {
                    Debug.WriteLine($"LoginViewModel.LoginAsync: LoginSuccessAction Target: {LoginSuccessAction.Target?.GetType().FullName}, Method: {LoginSuccessAction.Method.Name}. Invoking...");
                    LoginSuccessAction.Invoke(user);
                    Debug.WriteLine("LoginViewModel.LoginAsync: LoginSuccessAction invoked.");
                }
                else
                {
                    Debug.WriteLine("LoginViewModel.LoginAsync: LoginSuccessAction IS NULL. Cannot invoke.");
                }
            }
            else
            {
                ErrorMessage = "Invalid username or password, or API error.";
                Debug.WriteLine("LoginViewModel.LoginAsync: Login failed (user is null).");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LoginViewModel.LoginAsync: Login failed with exception: {ex}");
            ErrorMessage = $"An error occurred during login. Please try again.";
        }
        finally
        {
            IsLoggingIn = false;
            Debug.WriteLine("LoginViewModel.LoginAsync: IsLoggingIn set to false.");
        }
    }
}