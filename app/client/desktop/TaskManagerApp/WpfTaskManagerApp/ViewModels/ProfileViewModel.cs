using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Input;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.Services;
using WpfTaskManagerApp.ViewModels.Common;
namespace WpfTaskManagerApp.ViewModels;
public class ProfileViewModel : ViewModelBase
{
    private readonly IUserService? _userService;
    private readonly IAuthenticationService? _authenticationService;
    private User? _currentUserOriginal;
    private readonly ToastNotificationViewModel? _toastViewModel;

    private string _username = string.Empty;
    public string Username { get => _username; private set => SetProperty(ref _username, value); }

    private string _fullName = string.Empty;
    public string FullName { get => _fullName; set { if (SetProperty(ref _fullName, value)) (UpdateProfileCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

    private string _email = string.Empty;
    public string Email { get => _email; set { if (SetProperty(ref _email, value)) (UpdateProfileCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

    private string? _address;
    public string? Address { get => _address; set { if (SetProperty(ref _address, value)) (UpdateProfileCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

    private string _currentPassword = string.Empty;
    public string CurrentPassword { get => _currentPassword; set { if (SetProperty(ref _currentPassword, value)) (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

    private string _newPassword = string.Empty;
    public string NewPassword { get => _newPassword; set { if (SetProperty(ref _newPassword, value)) (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

    private string _confirmNewPassword = string.Empty;
    public string ConfirmNewPassword { get => _confirmNewPassword; set { if (SetProperty(ref _confirmNewPassword, value)) (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

    private string? _messageFromToast;
    public string? MessageFromToast { get => _messageFromToast; set => SetProperty(ref _messageFromToast, value); }

    private bool _isPasswordChangeVisible;
    public bool IsPasswordChangeVisible
    {
        get => _isPasswordChangeVisible;
        set => SetProperty(ref _isPasswordChangeVisible, value);
    }
    private bool _isUpdatingProfile;
    public bool IsUpdatingProfile
    {
        get => _isUpdatingProfile;
        set { if (SetProperty(ref _isUpdatingProfile, value)) (UpdateProfileCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }
    private bool _isChangingPassword;
    public bool IsChangingPassword
    {
        get => _isChangingPassword;
        set { if (SetProperty(ref _isChangingPassword, value)) (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    public ICommand UpdateProfileCommand { get; }
    public ICommand ChangePasswordCommand { get; }
    public ICommand ShowChangePasswordCommand { get; }

    public ProfileViewModel() : this(null, null, null) { }

    public ProfileViewModel(IUserService? userService,
                            IAuthenticationService? authenticationService,
                            ToastNotificationViewModel? toastViewModel)
    {
        _userService = userService;
        _authenticationService = authenticationService;
        _toastViewModel = toastViewModel;

        var user = _authenticationService?.CurrentUser;
        if (user == null && !IsInDesignModeStaticPVM())
        {
            _currentUserOriginal = new User(Guid.NewGuid(), "Error", "Error", UserRole.Staff, "Error User");
            ShowToastMessage("Error: No user logged in. Cannot load profile.", ToastType.Error);
            UpdateProfileCommand = new RelayCommand(async (_) => await UpdateProfileAsync(), (_) => CanUpdateProfile());
            ChangePasswordCommand = new RelayCommand(async (_) => await ChangePasswordAsync(), (_) => CanChangePassword());
        }
        else if (user != null)
        {
            _currentUserOriginal = new User(
                user.Id, user.Username, user.Email, user.Role,
                user.FullName, user.Address, user.IsActive
            )
            { PasswordHash = user.PasswordHash };
            LoadUserProfileFromOriginal();
            UpdateProfileCommand = new RelayCommand(async (_) => await UpdateProfileAsync(), (_) => CanUpdateProfile());
            ChangePasswordCommand = new RelayCommand(async (_) => await ChangePasswordAsync(), (_) => CanChangePassword());
        }
        else
        {
            _currentUserOriginal = new User(Guid.NewGuid(), "Designer", "designer@example.com", UserRole.Staff, "Design User", "123 Design St");
            _currentUserOriginal.PasswordHash = "design_hash";
            LoadUserProfileFromOriginal();
            UpdateProfileCommand = new RelayCommand(async (_) => await Task.CompletedTask, (_) => false);
            ChangePasswordCommand = new RelayCommand(async (_) => await Task.CompletedTask, (_) => false);
        }

        ShowChangePasswordCommand = new RelayCommand(_ =>
        {
            IsPasswordChangeVisible = !IsPasswordChangeVisible;
            if (!IsPasswordChangeVisible)
            {
                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmNewPassword = string.Empty;
                ClearErrors(nameof(CurrentPassword));
                ClearErrors(nameof(NewPassword));
                ClearErrors(nameof(ConfirmNewPassword));
            }
            else
            {
                // Khi hiện ra, chỉ xóa lỗi, không validate ngay. 
                // Validation sẽ xảy ra khi người dùng nhập liệu hoặc nhấn Save.
                ClearErrors(nameof(CurrentPassword));
                ClearErrors(nameof(NewPassword));
                ClearErrors(nameof(ConfirmNewPassword));
            }
            (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged();
        });

        if (IsInDesignModeStaticPVM() && _currentUserOriginal != null)
        {
            Username = _currentUserOriginal.Username;
            FullName = _currentUserOriginal.FullName;
            Email = _currentUserOriginal.Email;
            Address = _currentUserOriginal.Address;
        }
    }

    private static bool IsInDesignModeStaticPVM()
    {
        return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;
    }

    private void ShowToastMessage(string message, ToastType type, int duration = 4)
    {
        MessageFromToast = message;
        _toastViewModel?.Show(message, type, duration);
    }

    private void LoadUserProfileFromOriginal()
    {
        if (_currentUserOriginal == null) return;
        Username = _currentUserOriginal.Username;
        FullName = _currentUserOriginal.FullName;
        Email = _currentUserOriginal.Email;
        Address = _currentUserOriginal.Address;
        MessageFromToast = null;
        IsPasswordChangeVisible = false;

        CurrentPassword = string.Empty;
        NewPassword = string.Empty;
        ConfirmNewPassword = string.Empty;
        ClearErrors(nameof(CurrentPassword));
        ClearErrors(nameof(NewPassword));
        ClearErrors(nameof(ConfirmNewPassword));

        ClearErrors(nameof(FullName));
        ClearErrors(nameof(Email));
        ClearErrors(nameof(Address));

        (UpdateProfileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    protected override void ValidateProperty(string? propertyName)
    {
        base.ValidateProperty(propertyName);
        ClearErrors(propertyName);

        switch (propertyName)
        {
            case nameof(FullName):
                if (string.IsNullOrWhiteSpace(FullName)) AddError(nameof(FullName), "Full name is required.");
                break;
            case nameof(Email):
                if (string.IsNullOrWhiteSpace(Email)) AddError(nameof(Email), "Email is required.");
                else if (!IsValidEmail(Email)) AddError(nameof(Email), "Invalid email format.");
                break;
            case nameof(Address):
                if (string.IsNullOrWhiteSpace(Address)) AddError(nameof(Address), "Address is required.");
                break;
            case nameof(CurrentPassword):
                if (IsPasswordChangeVisible && string.IsNullOrWhiteSpace(CurrentPassword)) AddError(nameof(CurrentPassword), "Current password is required.");
                break;
            case nameof(NewPassword):
                if (IsPasswordChangeVisible)
                {
                    if (string.IsNullOrWhiteSpace(NewPassword)) AddError(nameof(NewPassword), "New password is required.");
                    else
                    {
                        if (NewPassword.Length < 6) AddError(nameof(NewPassword), "Must be at least 6 characters.");
                        if (!NewPassword.Any(char.IsDigit)) AddError(nameof(NewPassword), "Must contain at least one digit ('0'-'9').");
                        if (!NewPassword.Any(char.IsLower)) AddError(nameof(NewPassword), "Must contain at least one lowercase ('a'-'z').");
                        if (!NewPassword.Any(char.IsUpper)) AddError(nameof(NewPassword), "Must contain at least one uppercase ('A'-'Z').");
                        if (!NewPassword.Any(ch => !char.IsLetterOrDigit(ch))) AddError(nameof(NewPassword), "Must contain at least one non-alphanumeric character.");
                        if (!string.IsNullOrEmpty(CurrentPassword) && NewPassword == CurrentPassword && CurrentPassword != string.Empty) AddError(nameof(NewPassword), "New password must be different from current password.");
                    }
                    ValidateProperty(nameof(ConfirmNewPassword));
                }
                break;
            case nameof(ConfirmNewPassword):
                if (IsPasswordChangeVisible)
                {
                    if (string.IsNullOrWhiteSpace(ConfirmNewPassword)) AddError(nameof(ConfirmNewPassword), "Confirmation password is required.");
                    else if (NewPassword != ConfirmNewPassword) AddError(nameof(ConfirmNewPassword), "Passwords do not match.");
                }
                break;
        }
        (UpdateProfileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    // ValidateAllProperties không còn được gọi trực tiếp từ các Command Execute nữa,
    // nhưng vẫn có thể hữu ích cho các kịch bản khác hoặc để đảm bảo trạng thái lỗi tổng thể.
    protected override void ValidateAllProperties()
    {
        base.ValidateAllProperties();
        ValidateProperty(nameof(FullName));
        ValidateProperty(nameof(Email));
        ValidateProperty(nameof(Address));
        if (IsPasswordChangeVisible)
        {
            ValidateProperty(nameof(CurrentPassword));
            ValidateProperty(nameof(NewPassword));
            ValidateProperty(nameof(ConfirmNewPassword));
        }
    }

    private bool CanUpdateProfile()
    {
        if (IsUpdatingProfile || _authenticationService?.CurrentUser == null || _currentUserOriginal == null) return false;

        // Chỉ kiểm tra lỗi của các trường profile
        if (GetErrors(nameof(FullName)).Cast<string>().Any() ||
            GetErrors(nameof(Email)).Cast<string>().Any() ||
            GetErrors(nameof(Address)).Cast<string>().Any())
        {
            return false;
        }
        // Và phải có sự thay đổi
        return FullName != _currentUserOriginal.FullName ||
               Email != _currentUserOriginal.Email ||
               Address != _currentUserOriginal.Address;
    }

    private async Task UpdateProfileAsync()
    {
        // ***** VALIDATE CHỈ CÁC TRƯỜNG PROFILE *****
        ValidateProperty(nameof(FullName));
        ValidateProperty(nameof(Email));
        ValidateProperty(nameof(Address));

        if (GetErrors(nameof(FullName)).Cast<string>().Any() ||
            GetErrors(nameof(Email)).Cast<string>().Any() ||
            GetErrors(nameof(Address)).Cast<string>().Any())
        {
            ShowToastMessage("Please correct the profile validation errors.", ToastType.Warning);
            return;
        }
        // ***** KẾT THÚC VALIDATE CỤ THỂ *****

        // CanUpdateProfile() đã bao gồm kiểm tra lỗi cho các trường profile
        if (!CanUpdateProfile())
        {
            ShowToastMessage("No changes to save or form is invalid.", ToastType.Information);
            return;
        }

        if (_userService == null || _toastViewModel == null) { ShowToastMessage("User service not available.", ToastType.Error); return; }
        if (_currentUserOriginal == null) { ShowToastMessage("Error: User data is missing.", ToastType.Error); return; }

        IsUpdatingProfile = true;
        User userToUpdate = new User
        {
            Id = _currentUserOriginal.Id,
            Username = _currentUserOriginal.Username,
            Email = this.Email,
            Role = _currentUserOriginal.Role,
            FullName = this.FullName,
            Address = this.Address,
            IsActive = _currentUserOriginal.IsActive,
            PasswordHash = _currentUserOriginal.PasswordHash
        };

        bool success = false; string? operationMessage = null;
        try
        {
            success = await _userService.UpdateUserAsync(userToUpdate);
            if (!success) operationMessage = "Failed to update profile. API error or data conflict.";
        }
        catch (Exception ex) { operationMessage = "An error occurred while updating profile."; Debug.WriteLine($"Error updating profile: {ex}"); }
        finally { IsUpdatingProfile = false; }

        if (success)
        {
            _currentUserOriginal.FullName = FullName; _currentUserOriginal.Email = Email; _currentUserOriginal.Address = Address;
            var authUser = _authenticationService?.CurrentUser;
            if (authUser != null && authUser.Id == _currentUserOriginal.Id) { authUser.FullName = FullName; authUser.Email = Email; authUser.Address = Address; }
            ShowToastMessage("Profile updated successfully!", ToastType.Success);
        }
        else { ShowToastMessage(operationMessage ?? "Failed to update profile.", ToastType.Error); }
    }

    private bool CanChangePassword()
    {
        if (IsChangingPassword || _authenticationService?.CurrentUser == null) return false;

        // Chỉ kiểm tra lỗi của các trường password
        if (GetErrors(nameof(CurrentPassword)).Cast<string>().Any() ||
            GetErrors(nameof(NewPassword)).Cast<string>().Any() ||
            GetErrors(nameof(ConfirmNewPassword)).Cast<string>().Any())
        {
            return false;
        }
        // Các điều kiện cơ bản là các trường không rỗng (đã được xử lý bởi validation rules trong ValidateProperty)
        return !string.IsNullOrWhiteSpace(CurrentPassword) &&
               !string.IsNullOrWhiteSpace(NewPassword) &&
               !string.IsNullOrWhiteSpace(ConfirmNewPassword);
    }

    private async Task ChangePasswordAsync()
    {
        if (_authenticationService == null || _toastViewModel == null)
        { ShowToastMessage("Authentication service not available.", ToastType.Error); return; }

        // ***** VALIDATE CHỈ CÁC TRƯỜNG PASSWORD *****
        if (IsPasswordChangeVisible)
        {
            ValidateProperty(nameof(CurrentPassword));
            ValidateProperty(nameof(NewPassword));
            ValidateProperty(nameof(ConfirmNewPassword));
        }

        if (GetErrors(nameof(CurrentPassword)).Cast<string>().Any() ||
            GetErrors(nameof(NewPassword)).Cast<string>().Any() ||
            GetErrors(nameof(ConfirmNewPassword)).Cast<string>().Any())
        {
            ShowToastMessage("Please correct the password validation errors.", ToastType.Warning);
            return;
        }
        // ***** KẾT THÚC VALIDATE CỤ THỂ *****


        var user = _authenticationService.CurrentUser;
        if (user == null) { ShowToastMessage("Error: No user is currently logged in.", ToastType.Error); return; }

        IsChangingPassword = true;
        var changePasswordModel = new ChangePasswordModel { CurrentPassword = this.CurrentPassword, NewPassword = this.NewPassword };
        bool success = false; string? apiErrorMessage = null;
        try
        {
            success = await _authenticationService.ChangePasswordAsync(user.Id, changePasswordModel);
            if (!success) apiErrorMessage = "Failed to change password. Incorrect current password or API error.";
        }
        catch (Exception ex) { apiErrorMessage = "An unexpected error occurred while changing password."; Debug.WriteLine($"Error changing password: {ex}"); }
        finally { IsChangingPassword = false; }

        if (success)
        {
            ShowToastMessage("Password changed successfully!", ToastType.Success);
            CurrentPassword = string.Empty; NewPassword = string.Empty; ConfirmNewPassword = string.Empty;
            IsPasswordChangeVisible = false;
            ClearErrors(nameof(CurrentPassword));
            ClearErrors(nameof(NewPassword));
            ClearErrors(nameof(ConfirmNewPassword));
        }
        else { ShowToastMessage(apiErrorMessage ?? "Failed to change password.", ToastType.Error); }
         (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try { return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)); }
        catch (RegexMatchTimeoutException) { return false; }
    }
}