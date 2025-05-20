using System.Windows.Input;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Interfaces;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.ViewModels.Common;
namespace WpfTaskManagerApp.ViewModels;

// Handles login logic and state
public class LoginViewModel : ViewModelBase
{
    // Auth service
    private readonly IAuthenticationService? _authenticationService;
    // Toast notification VM
    private readonly ToastNotificationViewModel? _toastViewModel;

    // Username backing field
    private string _username = string.Empty;
    // Username property
    public string Username
    {
        get => _username;
        set
        {
            if (SetProperty(ref _username, value))
                (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    // Password backing field
    private string _password = string.Empty;
    // Password property
    public string Password
    {
        get => _password;
        set
        {
            if (SetProperty(ref _password, value))
                (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    // Error message backing field
    private string? _errorMessage;
    // Error message property
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    // Login state backing field
    private bool _isLoggingIn;
    // Indicates if logging in
    public bool IsLoggingIn
    {
        get => _isLoggingIn;
        set
        {
            if (SetProperty(ref _isLoggingIn, value))
                (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    // Login command
    public ICommand LoginCommand { get; }
    // Action on successful login
    public Action<User>? LoginSuccessAction { get; set; }

    // Default constructor (design-time)
    public LoginViewModel()
    {
        _authenticationService = null;
        _toastViewModel = null;
        LoginCommand = new RelayCommand(async (_) => await Task.CompletedTask, (_) => CanLogin());
        if (IsInDesignModeStatic())
        {
            Username = "designer";
            Password = "password";
        }
    }

    // Runtime constructor
    public LoginViewModel(IAuthenticationService authService, ToastNotificationViewModel toastVM, INavigationService navService)
    {
        _authenticationService = authService;
        _toastViewModel = toastVM;
        LoginCommand = new RelayCommand(async (_) => await LoginAsync(), (_) => CanLogin());
    }

    // Checks design mode
    private static bool IsInDesignModeStatic()
    {
        return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;
    }

    // Validates a property
    protected override void ValidateProperty(string? propertyName)
    {
        base.ValidateProperty(propertyName);
        ClearErrors(propertyName);
        switch (propertyName)
        {
            case nameof(Username):
                if (string.IsNullOrWhiteSpace(Username))
                    AddError(nameof(Username), "Username is required.");
                break;
            case nameof(Password):
                if (string.IsNullOrWhiteSpace(Password))
                    AddError(nameof(Password), "Password is required.");
                break;
        }
        (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    // Validates all properties
    protected override void ValidateAllProperties()
    {
        base.ValidateAllProperties();
        ValidateProperty(nameof(Username));
        ValidateProperty(nameof(Password));
    }

    // Can execute login
    private bool CanLogin()
    {
        return !HasErrors && !IsLoggingIn;
    }

    // Handles login process
    private async Task LoginAsync()
    {
        ValidateAllProperties();
        if (HasErrors)
        {
            _toastViewModel?.Show(GetErrors(null)?.Cast<string>().FirstOrDefault() ?? "Please correct errors.", ToastType.Warning);
            return;
        }
        if (_authenticationService == null || _toastViewModel == null)
        {
            _toastViewModel?.Show("Core services unavailable.", ToastType.Error);
            return;
        }
        IsLoggingIn = true;
        ErrorMessage = null;
        try
        {
            var user = await _authenticationService.LoginAsync(Username, this.Password);
            if (user != null)
            {
                _toastViewModel.Show("Login successful!", ToastType.Success);
                LoginSuccessAction?.Invoke(user);
            }
            else
            {
                _toastViewModel.Show("Invalid username or password.", ToastType.Error);
            }
        }
        catch (Exception)
        {
            _toastViewModel.Show("An error occurred during login.", ToastType.Error);
        }
        finally
        {
            IsLoggingIn = false;
        }
    }
}
