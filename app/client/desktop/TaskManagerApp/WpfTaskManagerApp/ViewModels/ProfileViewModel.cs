using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Input;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.Services;
using WpfTaskManagerApp.ViewModels.Common;
using WpfTaskManagerApp.ViewModels;
// --- ProfileViewModel ---
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
    public bool IsPasswordChangeVisible { get => _isPasswordChangeVisible; set => SetProperty(ref _isPasswordChangeVisible, value); }
    private bool _isUpdatingProfile;
    public bool IsUpdatingProfile { get => _isUpdatingProfile; set { if (SetProperty(ref _isUpdatingProfile, value)) (UpdateProfileCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }
    private bool _isChangingPassword;
    public bool IsChangingPassword { get => _isChangingPassword; set { if (SetProperty(ref _isChangingPassword, value)) (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }
    public ICommand UpdateProfileCommand { get; }
    public ICommand ChangePasswordCommand { get; }
    public ICommand ShowChangePasswordCommand { get; }

    public ProfileViewModel() : this(null, null, null) { }
    public ProfileViewModel(IUserService? userService, IAuthenticationService? authService, ToastNotificationViewModel? toastVM)
    {
        _userService = userService; _authenticationService = authService; _toastViewModel = toastVM;
        var user = _authenticationService?.CurrentUser;
        if (user == null && !IsInDesignModeStaticPVM())
        { _currentUserOriginal = new User(Guid.NewGuid(), "E", "E", UserRole.Staff, "Error"); ShowToastMessage("No user logged in.", ToastType.Error); UpdateProfileCommand = new RelayCommand( _ => { }, _ => false); ChangePasswordCommand = new RelayCommand( _ => { }, _ => false); }
        else if (user != null)
        { _currentUserOriginal = new User(user.Id, user.Username, user.Email, user.Role, user.FullName, user.Address, user.IsActive) { PasswordHash = user.PasswordHash }; LoadUserProfileFromOriginal(); UpdateProfileCommand = new RelayCommand(async (_) => await UpdateProfileAsync(), (_) => CanUpdateProfile()); ChangePasswordCommand = new RelayCommand(async (_) => await ChangePasswordAsync(), (_) => CanChangePassword()); }
        else { _currentUserOriginal = new User(Guid.NewGuid(), "D", "d@e.c", UserRole.Staff, "Designer"); LoadUserProfileFromOriginal(); UpdateProfileCommand = new RelayCommand( _ => { }, _ => false); ChangePasswordCommand = new RelayCommand( _ => { }, _ => false); }
        ShowChangePasswordCommand = new RelayCommand(_ => { IsPasswordChangeVisible = !IsPasswordChangeVisible; if (!IsPasswordChangeVisible) { CurrentPassword = ""; NewPassword = ""; ConfirmNewPassword = ""; ClearErrors(nameof(CurrentPassword)); ClearErrors(nameof(NewPassword)); ClearErrors(nameof(ConfirmNewPassword)); } else { ClearErrors(nameof(CurrentPassword)); ClearErrors(nameof(NewPassword)); ClearErrors(nameof(ConfirmNewPassword)); } (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged(); });
        if (IsInDesignModeStaticPVM() && _currentUserOriginal != null) { Username = _currentUserOriginal.Username; FullName = _currentUserOriginal.FullName; Email = _currentUserOriginal.Email; Address = _currentUserOriginal.Address; }
    }
    private static bool IsInDesignModeStaticPVM() { return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime; }
    private void ShowToastMessage(string message, ToastType type, int duration = 4) { MessageFromToast = message; _toastViewModel?.Show(message, type, duration); }
    private void LoadUserProfileFromOriginal() { if (_currentUserOriginal == null) return; Username = _currentUserOriginal.Username; FullName = _currentUserOriginal.FullName; Email = _currentUserOriginal.Email; Address = _currentUserOriginal.Address; MessageFromToast = null; IsPasswordChangeVisible = false; CurrentPassword = ""; NewPassword = ""; ConfirmNewPassword = ""; ClearErrors(nameof(FullName)); ClearErrors(nameof(Email)); ClearErrors(nameof(Address)); ClearErrors(nameof(CurrentPassword)); ClearErrors(nameof(NewPassword)); ClearErrors(nameof(ConfirmNewPassword)); (UpdateProfileCommand as RelayCommand)?.RaiseCanExecuteChanged(); (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    protected override void ValidateProperty(string? propertyName)
    {
        base.ValidateProperty(propertyName); ClearErrors(propertyName);
        switch (propertyName)
        {
            case nameof(FullName): if (string.IsNullOrWhiteSpace(FullName)) AddError(nameof(FullName), "Full name is required."); break;
            case nameof(Email): if (string.IsNullOrWhiteSpace(Email)) AddError(nameof(Email), "Email is required."); else if (!IsValidEmail(Email)) AddError(nameof(Email), "Invalid email format."); break;
            case nameof(Address): if (string.IsNullOrWhiteSpace(Address)) AddError(nameof(Address), "Address is required."); break;
            case nameof(CurrentPassword): if (IsPasswordChangeVisible && string.IsNullOrWhiteSpace(CurrentPassword)) AddError(nameof(CurrentPassword), "Current password is required."); break;
            case nameof(NewPassword):
                if (IsPasswordChangeVisible)
                {
                    if (string.IsNullOrWhiteSpace(NewPassword)) AddError(nameof(NewPassword), "New password is required.");
                    else { if (NewPassword.Length < 6) AddError(nameof(NewPassword), "Min 6 characters."); if (!NewPassword.Any(char.IsDigit)) AddError(nameof(NewPassword), "Requires digit."); if (!NewPassword.Any(char.IsLower)) AddError(nameof(NewPassword), "Requires lowercase."); if (!NewPassword.Any(char.IsUpper)) AddError(nameof(NewPassword), "Requires uppercase."); if (!NewPassword.Any(ch => !char.IsLetterOrDigit(ch))) AddError(nameof(NewPassword), "Requires non-alphanumeric."); if (!string.IsNullOrEmpty(CurrentPassword) && NewPassword == CurrentPassword && CurrentPassword != string.Empty) AddError(nameof(NewPassword), "Must be different from current."); }
                    ValidateProperty(nameof(ConfirmNewPassword));
                }
                break;
            case nameof(ConfirmNewPassword): if (IsPasswordChangeVisible) { if (string.IsNullOrWhiteSpace(ConfirmNewPassword)) AddError(nameof(ConfirmNewPassword), "Confirmation is required."); else if (NewPassword != ConfirmNewPassword) AddError(nameof(ConfirmNewPassword), "Passwords do not match."); } break;
        }
        (UpdateProfileCommand as RelayCommand)?.RaiseCanExecuteChanged(); (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
    protected override void ValidateAllProperties() { ValidateProperty(nameof(FullName)); ValidateProperty(nameof(Email)); ValidateProperty(nameof(Address)); if (IsPasswordChangeVisible) { ValidateProperty(nameof(CurrentPassword)); ValidateProperty(nameof(NewPassword)); ValidateProperty(nameof(ConfirmNewPassword)); } }
    private bool CanUpdateProfile() { if (IsUpdatingProfile || _authenticationService?.CurrentUser == null || _currentUserOriginal == null) return false; if (GetErrors(nameof(FullName)).Cast<string>().Any() || GetErrors(nameof(Email)).Cast<string>().Any() || GetErrors(nameof(Address)).Cast<string>().Any()) return false; return FullName != _currentUserOriginal.FullName || Email != _currentUserOriginal.Email || Address != _currentUserOriginal.Address; }
    private async Task UpdateProfileAsync()
    {
        ValidateProperty(nameof(FullName)); ValidateProperty(nameof(Email)); ValidateProperty(nameof(Address));
        if (GetErrors(nameof(FullName)).Cast<string>().Any() || GetErrors(nameof(Email)).Cast<string>().Any() || GetErrors(nameof(Address)).Cast<string>().Any()) { ShowToastMessage("Please correct profile errors.", ToastType.Warning); return; }
        if (!CanUpdateProfile()) { ShowToastMessage("No changes to save or form is invalid.", ToastType.Information); return; }
        if (_userService == null || _toastViewModel == null || _currentUserOriginal == null || _authenticationService == null) { ShowToastMessage("Service or user data unavailable.", ToastType.Error); return; }
        IsUpdatingProfile = true; User userToUpdate = new User { Id = _currentUserOriginal.Id, FullName = this.FullName, Email = this.Email, Address = this.Address };
        bool success = false; string? msg = null; try { success = await _userService.UpdateCurrentUserProfileAsync(userToUpdate); if (!success) msg = "Failed to update profile (API)."; } catch (Exception ex) { msg = "Error updating profile."; Debug.WriteLine($"UpdateProfileAsync Error: {ex}"); } finally { IsUpdatingProfile = false; }
        if (success) { _currentUserOriginal.FullName = FullName; _currentUserOriginal.Email = Email; _currentUserOriginal.Address = Address; var authUser = _authenticationService?.CurrentUser; if (authUser != null) { authUser.FullName = FullName; authUser.Email = Email; authUser.Address = Address; } ShowToastMessage("Profile updated!", ToastType.Success); } else { ShowToastMessage(msg ?? "Update failed.", ToastType.Error); }
    }
    private bool CanChangePassword() { if (IsChangingPassword || _authenticationService?.CurrentUser == null) return false; if (GetErrors(nameof(CurrentPassword)).Cast<string>().Any() || GetErrors(nameof(NewPassword)).Cast<string>().Any() || GetErrors(nameof(ConfirmNewPassword)).Cast<string>().Any()) return false; return !string.IsNullOrWhiteSpace(CurrentPassword) && !string.IsNullOrWhiteSpace(NewPassword) && !string.IsNullOrWhiteSpace(ConfirmNewPassword); }
    private async Task ChangePasswordAsync()
    {
        if (_authenticationService == null || _toastViewModel == null) { ShowToastMessage("Service unavailable.", ToastType.Error); return; }
        if (IsPasswordChangeVisible) { ValidateProperty(nameof(CurrentPassword)); ValidateProperty(nameof(NewPassword)); ValidateProperty(nameof(ConfirmNewPassword)); }
        if (GetErrors(nameof(CurrentPassword)).Cast<string>().Any() || GetErrors(nameof(NewPassword)).Cast<string>().Any() || GetErrors(nameof(ConfirmNewPassword)).Cast<string>().Any()) { ShowToastMessage("Please correct password errors.", ToastType.Warning); return; }
        var user = _authenticationService.CurrentUser; if (user == null) { ShowToastMessage("No user logged in.", ToastType.Error); return; }
        IsChangingPassword = true; var model = new ChangePasswordModel { CurrentPassword = this.CurrentPassword, NewPassword = this.NewPassword };
        bool success = false; string? apiMsg = null; try { success = await _authenticationService.ChangePasswordAsync(user.Id, model); if (!success) apiMsg = "Failed (API). Check current password."; } catch (Exception ex) { apiMsg = "Error changing password."; Debug.WriteLine($"ChangePasswordAsync Error: {ex}"); } finally { IsChangingPassword = false; }
        if (success) { ShowToastMessage("Password changed!", ToastType.Success); CurrentPassword = ""; NewPassword = ""; ConfirmNewPassword = ""; IsPasswordChangeVisible = false; ClearErrors(nameof(CurrentPassword)); ClearErrors(nameof(NewPassword)); ClearErrors(nameof(ConfirmNewPassword)); } else { ShowToastMessage(apiMsg ?? "Change password failed.", ToastType.Error); }
         (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
    private bool IsValidEmail(string email) { if (string.IsNullOrWhiteSpace(email)) return false; try { return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)); } catch (RegexMatchTimeoutException) { return false; } }
}