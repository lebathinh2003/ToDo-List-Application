using System.Diagnostics;
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

    // ***** THÊM PUBLIC PROPERTY CHO USERNAME *****
    private string _username = string.Empty;
    public string Username
    {
        get => _username;
        private set => SetProperty(ref _username, value); // Chỉ set từ trong ViewModel
    }
    // ***** KẾT THÚC THÊM PROPERTY USERNAME *****

    private string _fullName = string.Empty;
    public string FullName
    {
        get => _fullName;
        set { if (SetProperty(ref _fullName, value)) (UpdateProfileCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set { if (SetProperty(ref _email, value)) (UpdateProfileCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    private string? _address;
    public string? Address
    {
        get => _address;
        set { if (SetProperty(ref _address, value)) (UpdateProfileCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    private string _currentPassword = string.Empty;
    public string CurrentPassword
    {
        get => _currentPassword;
        set { if (SetProperty(ref _currentPassword, value)) (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    private string _newPassword = string.Empty;
    public string NewPassword
    {
        get => _newPassword;
        set { if (SetProperty(ref _newPassword, value)) (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    private string _confirmNewPassword = string.Empty;
    public string ConfirmNewPassword
    {
        get => _confirmNewPassword;
        set { if (SetProperty(ref _confirmNewPassword, value)) (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    private string? _message;
    public string? Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }
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

    public ProfileViewModel()
    {
        _userService = null;
        _authenticationService = null;
        _currentUserOriginal = new User(Guid.NewGuid(), "Designer", "designer@example.com", UserRole.Staff, "Design User", "123 Design St");
        _currentUserOriginal.PasswordHash = "design_hash";
        LoadUserProfileFromOriginal(); // Sẽ set Username
        Message = "Design-time Profile View";

        UpdateProfileCommand = new RelayCommand(async (_) => await Task.CompletedTask, (_) => false);
        ChangePasswordCommand = new RelayCommand(async (_) => await Task.CompletedTask, (_) => false);
        ShowChangePasswordCommand = new RelayCommand(_ => IsPasswordChangeVisible = !IsPasswordChangeVisible);
        if (IsInDesignModeStaticPVM())
        {
            Username = "design_user_prop"; // Dữ liệu mẫu cho property Username
            FullName = "Design Full Name";
            Email = "design@example.com";
            Address = "123 Design Address";
        }
    }

    private static bool IsInDesignModeStaticPVM()
    {
        return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;
    }


    public ProfileViewModel(IUserService userService, IAuthenticationService authenticationService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

        var user = _authenticationService.CurrentUser;
        if (user == null)
        {
            _currentUserOriginal = new User(Guid.NewGuid(), "Error", "Error", UserRole.Staff, "Error User");
            Message = "Error: No user logged in. Cannot load profile.";
            UpdateProfileCommand = new RelayCommand(async (_) => await UpdateProfileAsync(), (_) => false);
            ChangePasswordCommand = new RelayCommand(async (_) => await ChangePasswordAsync(), (_) => false);
        }
        else
        {
            Debug.WriteLine($"ProfileViewModel Constructor: Auth User ID: {user.Id}, Username: '{user.Username}', FullName: '{user.FullName}'");
            _currentUserOriginal = new User(
                user.Id,
                user.Username,
                user.Email,
                user.Role,
                user.FullName,
                user.Address,
                user.IsActive
            );
            _currentUserOriginal.PasswordHash = user.PasswordHash;
            Debug.WriteLine($"ProfileViewModel Constructor: _currentUserOriginal Username: '{_currentUserOriginal.Username}', FullName: '{_currentUserOriginal.FullName}'");
            LoadUserProfileFromOriginal();
            UpdateProfileCommand = new RelayCommand(async (_) => await UpdateProfileAsync(), CanUpdateProfile);
            ChangePasswordCommand = new RelayCommand(async (_) => await ChangePasswordAsync(), CanChangePassword);
        }

        ShowChangePasswordCommand = new RelayCommand(_ => IsPasswordChangeVisible = !IsPasswordChangeVisible);
    }

    private void LoadUserProfileFromOriginal()
    {
        if (_currentUserOriginal == null) return;

        // ***** GÁN GIÁ TRỊ CHO PROPERTY USERNAME *****
        Username = _currentUserOriginal.Username;
        // ***** KẾT THÚC GÁN USERNAME *****

        Debug.WriteLine($"ProfileViewModel.LoadUserProfileFromOriginal: Loading from _currentUserOriginal.Username: '{_currentUserOriginal.Username}', FullName: '{_currentUserOriginal.FullName}'");
        FullName = _currentUserOriginal.FullName;
        Email = _currentUserOriginal.Email;
        Address = _currentUserOriginal.Address;
        Message = null;
        IsPasswordChangeVisible = false;
        CurrentPassword = string.Empty;
        NewPassword = string.Empty;
        ConfirmNewPassword = string.Empty;
        (UpdateProfileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private bool CanUpdateProfile(object? arg)
    {
        if (IsUpdatingProfile || _authenticationService?.CurrentUser == null || _currentUserOriginal == null) return false;

        return !string.IsNullOrWhiteSpace(FullName) &&
               !string.IsNullOrWhiteSpace(Email) &&
               (FullName != _currentUserOriginal.FullName ||
                Email != _currentUserOriginal.Email ||
                Address != _currentUserOriginal.Address);
    }
    private async Task UpdateProfileAsync()
    {
        if (_userService == null) { Message = "User service not available."; return; }
        Message = null;
        if (!CanUpdateProfile(null))
        {
            Message = "No changes to save or invalid data.";
            return;
        }
        if (_currentUserOriginal == null)
        {
            Message = "Error: User data is missing.";
            return;
        }
        IsUpdatingProfile = true;

        User userToUpdate = new User
        {
            Id = _currentUserOriginal.Id,
            Username = _currentUserOriginal.Username, // Username không được cập nhật từ form này
            Email = this.Email,
            Role = _currentUserOriginal.Role,
            FullName = this.FullName,
            Address = this.Address,
            IsActive = _currentUserOriginal.IsActive
        };
        userToUpdate.PasswordHash = _currentUserOriginal.PasswordHash;

        bool success = false;
        try
        {
            success = await _userService.UpdateUserAsync(userToUpdate);
        }
        catch (Exception ex)
        {
            Message = "An error occurred while updating profile.";
        }
        finally { IsUpdatingProfile = false; }

        if (success)
        {
            _currentUserOriginal.FullName = FullName;
            _currentUserOriginal.Email = Email;
            _currentUserOriginal.Address = Address;

            var authUser = _authenticationService?.CurrentUser;
            if (authUser != null && authUser.Id == _currentUserOriginal.Id)
            {
                authUser.FullName = FullName;
                authUser.Email = Email;
                authUser.Address = Address;
            }

            Message = "Profile updated successfully!";
            (UpdateProfileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
        else
        {
            if (string.IsNullOrEmpty(Message)) Message = "Failed to update profile. Email might be in use, or another error occurred.";
        }
    }
    private bool CanChangePassword(object? arg)
    {
        if (IsChangingPassword || _authenticationService?.CurrentUser == null) return false;

        return !string.IsNullOrWhiteSpace(CurrentPassword) &&
               !string.IsNullOrWhiteSpace(NewPassword) &&
               NewPassword.Length >= 6 &&
               NewPassword == ConfirmNewPassword;
    }

    private async Task ChangePasswordAsync()
    {
        if (_userService == null || _authenticationService == null) { Message = "Service not available."; return; }
        Message = null;
        if (!CanChangePassword(null))
        {
            Message = "Please check password fields. New password must be at least 6 characters and match confirmation.";
            return;
        }
        var user = _authenticationService.CurrentUser;
        if (user == null)
        {
            Message = "Error: No user is currently logged in.";
            return;
        }
        IsChangingPassword = true;

        var changePasswordModel = new ChangePasswordModel
        {
            CurrentPassword = this.CurrentPassword,
            NewPassword = this.NewPassword
        };

        bool success = false;
        try
        {
            success = await _userService.ChangePasswordAsync(user.Id, changePasswordModel);
        }
        catch (Exception ex)
        {
            Message = "An error occurred while changing password.";
        }
        finally { IsChangingPassword = false; }

        if (success)
        {
            Message = "Password changed successfully!";
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
            IsPasswordChangeVisible = false;
        }
        else
        {
            if (string.IsNullOrEmpty(Message)) Message = "Failed to change password. Incorrect current password or API error.";
        }
         (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}