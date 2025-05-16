using System.Diagnostics;
using System.Windows.Input;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.Services;
using WpfTaskManagerApp.ViewModels.Common;

namespace WpfTaskManagerApp.ViewModels;
public class LoginViewModel : ViewModelBase
{
    private readonly IAuthenticationService? _authenticationService;
    private readonly ToastNotificationViewModel? _toastViewModel;

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

    public LoginViewModel() // Constructor cho Design-Time
    {
        _authenticationService = null;
        _toastViewModel = null;
        LoginCommand = new RelayCommand(
            async (_) => await Task.CompletedTask,
            (_) => CanLogin() // Gọi CanLogin() cho design-time
        );

        if (IsInDesignModeStatic())
        {
            Username = "designer";
            Password = "password";
            // ValidateAllProperties(); // Không gọi ở đây nữa
        }
    }

    public LoginViewModel(IAuthenticationService authenticationService,
                          ToastNotificationViewModel toastViewModel,
                          INavigationService navigationService)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _toastViewModel = toastViewModel ?? throw new ArgumentNullException(nameof(toastViewModel));

        LoginCommand = new RelayCommand(
            async (_) => await LoginAsync(),
            (_) => CanLogin() // CanExecute sẽ dựa vào !HasErrors và !IsLoggingIn
        );
        // ValidateAllProperties(); // ***** KHÔNG GỌI ValidateAllProperties TRONG CONSTRUCTOR RUNTIME NỮA *****
        // Validation sẽ xảy ra khi người dùng nhập liệu
    }

    private static bool IsInDesignModeStatic()
    {
        return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;
    }

    protected override void ValidateProperty(string? propertyName)
    {
        base.ValidateProperty(propertyName);
        ClearErrors(propertyName);

        switch (propertyName)
        {
            case nameof(Username):
                if (string.IsNullOrWhiteSpace(Username))
                {
                    AddError(nameof(Username), "Username is required.");
                }
                break;
            case nameof(Password):
                if (string.IsNullOrWhiteSpace(Password))
                {
                    AddError(nameof(Password), "Password is required.");
                }
                break;
        }
        (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    protected override void ValidateAllProperties()
    {
        base.ValidateAllProperties();
        // Gọi ValidateProperty cho từng trường để đảm bảo tất cả lỗi được cập nhật
        ValidateProperty(nameof(Username));
        ValidateProperty(nameof(Password));
    }

    private bool CanLogin()
    {
        // Nút Login sẽ enable nếu không có lỗi validation và không đang trong quá trình login
        // HasErrors sẽ được cập nhật bởi ValidateProperty khi người dùng gõ
        return !HasErrors && !IsLoggingIn;
    }

    private async Task LoginAsync()
    {
        // Validate lại tất cả các trường trước khi thực hiện hành động submit
        ValidateAllProperties();
        if (HasErrors) // Nếu vẫn còn lỗi sau khi validate tất cả
        {
            // Lấy lỗi đầu tiên để hiển thị hoặc một thông báo chung
            var firstError = GetErrors(null)?.Cast<string>().FirstOrDefault();
            _toastViewModel?.Show(firstError ?? "Please correct the validation errors.", ToastType.Warning);
            return;
        }

        if (_authenticationService == null || _toastViewModel == null)
        {
            _toastViewModel?.Show("Core services are not available.", ToastType.Error);
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
                // ErrorMessage = "Invalid username or password, or API error.";
                _toastViewModel.Show("Invalid username or password, or API error.", ToastType.Error);
            }
        }
        catch (Exception ex)
        {
            // ErrorMessage = $"An error occurred during login. Please try again.";
            _toastViewModel.Show($"An error occurred during login. Please try again.", ToastType.Error);
        }
        finally
        {
            IsLoggingIn = false;
        }
    }
}