using System.Text.RegularExpressions;
using System.Windows.Input;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Interfaces;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.ViewModels.Common;

namespace WpfTaskManagerApp.ViewModels;

// ViewModel for user profile management.
public class ProfileViewModel : ViewModelBase
{
    // Services.
    private readonly IUserService? _userService;
    private readonly IAuthenticationService? _authenticationService;
    private readonly ToastNotificationViewModel? _toastViewModel;

    // Original user data (for checking changes).
    private User? _currentUserOriginal;

    // Profile properties.
    private string _username = string.Empty;
    public string Username { get => _username; private set => SetProperty(ref _username, value); } // Read-only from UI.

    private string _fullName = string.Empty;
    public string FullName
    {
        get => _fullName;
        set { if (SetProperty(ref _fullName, value)) ValidateProperty(nameof(FullName)); }
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set { if (SetProperty(ref _email, value)) ValidateProperty(nameof(Email)); }
    }

    private string? _address;
    public string? Address
    {
        get => _address;
        set { if (SetProperty(ref _address, value)) ValidateProperty(nameof(Address)); }
    }

    // Password change properties.
    private string _currentPassword = string.Empty;
    public string CurrentPassword
    {
        get => _currentPassword;
        set { if (SetProperty(ref _currentPassword, value)) ValidateProperty(nameof(CurrentPassword)); }
    }

    private string _newPassword = string.Empty;
    public string NewPassword
    {
        get => _newPassword;
        set { if (SetProperty(ref _newPassword, value)) ValidateProperty(nameof(NewPassword)); }
    }

    private string _confirmNewPassword = string.Empty;
    public string ConfirmNewPassword
    {
        get => _confirmNewPassword;
        set { if (SetProperty(ref _confirmNewPassword, value)) ValidateProperty(nameof(ConfirmNewPassword)); }
    }

    // UI state.
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

    // Commands.
    public ICommand UpdateProfileCommand { get; }
    public ICommand ChangePasswordCommand { get; }
    public ICommand ShowChangePasswordCommand { get; }

    // Constructor for design-time.
    public ProfileViewModel() : this(null, null, null) { }

    // Main constructor.
    public ProfileViewModel(IUserService? userService, IAuthenticationService? authService, ToastNotificationViewModel? toastVM)
    {
        _userService = userService;
        _authenticationService = authService;
        _toastViewModel = toastVM;

        var user = _authenticationService?.CurrentUser;

        if (user != null)
        {
            // Clone current user for original state.
            _currentUserOriginal = new User(user.Id, user.Username, user.Email, user.Role, user.FullName, user.Address, user.IsActive) { PasswordHash = user.PasswordHash };
            LoadUserProfileFromOriginal();
            UpdateProfileCommand = new RelayCommand(async (_) => await UpdateProfileAsync(), (_) => CanUpdateProfile());
            ChangePasswordCommand = new RelayCommand(async (_) => await ChangePasswordAsync(), (_) => CanChangePassword());
        }
        else if (IsInDesignModeStaticPVM()) // Design-time data.
        {
            _currentUserOriginal = new User(Guid.NewGuid(), "designer.user", "designer@example.com", UserRole.Staff, "Designer Name", "123 Design St", true);
            LoadUserProfileFromOriginal();
            UpdateProfileCommand = new RelayCommand(_ => { }, _ => false); // No-op for design.
            ChangePasswordCommand = new RelayCommand(_ => { }, _ => false); // No-op for design.
        }
        else // No user logged in and not in design mode.
        {
            _currentUserOriginal = null; // Ensure it's null.
            ShowToastMessage("No user is currently logged in. Profile cannot be displayed.", ToastType.Error);
            UpdateProfileCommand = new RelayCommand(_ => { }, _ => false); // Disable command.
            ChangePasswordCommand = new RelayCommand(_ => { }, _ => false); // Disable command.
        }

        ShowChangePasswordCommand = new RelayCommand(_ => TogglePasswordSection());
    }

    // Checks if running in design mode.
    private static bool IsInDesignModeStaticPVM() => System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;

    // Shows a toast message via the toast view model.
    private void ShowToastMessage(string message, ToastType type, string? details = null)
    {
        _toastViewModel?.Show(message, type);
    }

    // Loads user profile data from the original (cloned) user object.
    private void LoadUserProfileFromOriginal()
    {
        if (_currentUserOriginal == null) return;

        Username = _currentUserOriginal.Username;
        FullName = _currentUserOriginal.FullName;
        Email = _currentUserOriginal.Email;
        Address = _currentUserOriginal.Address;

        IsPasswordChangeVisible = false; // Hide password section initially.
        CurrentPassword = "";
        NewPassword = "";
        ConfirmNewPassword = "";

        ClearAllValidationErrors(); // Clear any previous errors.
        (UpdateProfileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    // Toggles visibility of the password change section.
    private void TogglePasswordSection()
    {
        IsPasswordChangeVisible = !IsPasswordChangeVisible;
        if (!IsPasswordChangeVisible) // If hiding, clear password fields and errors.
        {
            CurrentPassword = "";
            NewPassword = "";
            ConfirmNewPassword = "";
            ClearErrors(nameof(CurrentPassword));
            ClearErrors(nameof(NewPassword));
            ClearErrors(nameof(ConfirmNewPassword));
        }
        else // If showing, ensure fields are clear and ready for input.
        {
            ClearErrors(nameof(CurrentPassword)); // Clear potential previous errors.
            ClearErrors(nameof(NewPassword));
            ClearErrors(nameof(ConfirmNewPassword));
        }
        (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    // Validates a specific property.
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
                if (string.IsNullOrWhiteSpace(Address)) AddError(nameof(Address), "Address is required."); // Assuming mandatory.
                break;
            case nameof(CurrentPassword):
                if (IsPasswordChangeVisible && string.IsNullOrWhiteSpace(CurrentPassword))
                    AddError(nameof(CurrentPassword), "Current password is required.");
                break;
            case nameof(NewPassword):
                if (IsPasswordChangeVisible)
                {
                    if (string.IsNullOrWhiteSpace(NewPassword)) AddError(nameof(NewPassword), "New password is required.");
                    else
                    {
                        // Password complexity rules.
                        if (NewPassword.Length < 6) AddError(nameof(NewPassword), "Min 6 characters.");
                        if (!NewPassword.Any(char.IsDigit)) AddError(nameof(NewPassword), "Requires a digit.");
                        if (!NewPassword.Any(char.IsLower)) AddError(nameof(NewPassword), "Requires a lowercase letter.");
                        if (!NewPassword.Any(char.IsUpper)) AddError(nameof(NewPassword), "Requires an uppercase letter.");
                        if (!NewPassword.Any(ch => !char.IsLetterOrDigit(ch))) AddError(nameof(NewPassword), "Requires a non-alphanumeric character.");
                        if (!string.IsNullOrEmpty(CurrentPassword) && NewPassword == CurrentPassword)
                            AddError(nameof(NewPassword), "New password must be different from current.");
                    }
                    ValidateProperty(nameof(ConfirmNewPassword)); // Re-validate confirmation.
                }
                break;
            case nameof(ConfirmNewPassword):
                if (IsPasswordChangeVisible)
                {
                    if (string.IsNullOrWhiteSpace(ConfirmNewPassword)) AddError(nameof(ConfirmNewPassword), "Password confirmation is required.");
                    else if (NewPassword != ConfirmNewPassword) AddError(nameof(ConfirmNewPassword), "Passwords do not match.");
                }
                break;
        }
        (UpdateProfileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    // Validates all relevant properties.
    protected override void ValidateAllProperties()
    {
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

    // Determines if the Update Profile command can execute.
    private bool CanUpdateProfile()
    {
        if (IsUpdatingProfile || _authenticationService?.CurrentUser == null || _currentUserOriginal == null) return false;
        // Check for validation errors in profile fields.
        if (GetErrors(nameof(FullName)).Cast<string>().Any() ||
            GetErrors(nameof(Email)).Cast<string>().Any() ||
            GetErrors(nameof(Address)).Cast<string>().Any()) return false;
        // Check if any profile field has changed.
        return FullName != _currentUserOriginal.FullName ||
               Email != _currentUserOriginal.Email ||
               Address != _currentUserOriginal.Address;
    }

    // Updates the user's profile.
    private async Task UpdateProfileAsync()
    {
        ValidateProperty(nameof(FullName));
        ValidateProperty(nameof(Email));
        ValidateProperty(nameof(Address));

        if (GetErrors(nameof(FullName)).Cast<string>().Any() ||
            GetErrors(nameof(Email)).Cast<string>().Any() ||
            GetErrors(nameof(Address)).Cast<string>().Any())
        {
            ShowToastMessage("Please correct profile errors before saving.", ToastType.Warning);
            return;
        }

        if (!CanUpdateProfile()) // Handles no changes or initial invalid state.
        {
            ShowToastMessage("No changes detected or form is invalid.", ToastType.Information);
            return;
        }

        if (_userService == null || _toastViewModel == null || _currentUserOriginal == null || _authenticationService == null)
        {
            ShowToastMessage("Service or user data is unavailable.", ToastType.Error);
            return;
        }

        IsUpdatingProfile = true;
        // Create a User object with only the updatable fields.
        User userToUpdate = new User
        {
            Id = _currentUserOriginal.Id, // Important: ensure ID is for the correct user.
            FullName = this.FullName,
            Email = this.Email,
            Address = this.Address
            // Other fields like Username, Role, IsActive are not updated here.
        };

        bool success = false;
        string? failureDetails = null;
        try
        {
            success = await _userService.UpdateCurrentUserProfileAsync(userToUpdate);
            if (!success) failureDetails = "API request to update profile failed.";
        }
        catch (Exception ex)
        {
            failureDetails = ex.Message;
            success = false;
        }
        finally
        {
            IsUpdatingProfile = false;
        }

        if (success)
        {
            // Update original and current authenticated user details in memory.
            _currentUserOriginal.FullName = FullName;
            _currentUserOriginal.Email = Email;
            _currentUserOriginal.Address = Address;

            var authUser = _authenticationService.CurrentUser;
            if (authUser != null)
            {
                authUser.FullName = FullName;
                authUser.Email = Email;
                authUser.Address = Address;
            }
            ShowToastMessage("Profile updated successfully!", ToastType.Success);
        }
        else
        {
            ShowToastMessage("Failed to update profile.", ToastType.Error, failureDetails);
        }
        (UpdateProfileCommand as RelayCommand)?.RaiseCanExecuteChanged(); // Re-evaluate CanExecute.
    }

    // Determines if the Change Password command can execute.
    private bool CanChangePassword()
    {
        if (IsChangingPassword || _authenticationService?.CurrentUser == null || !IsPasswordChangeVisible) return false;
        // Check for validation errors in password fields.
        if (GetErrors(nameof(CurrentPassword)).Cast<string>().Any() ||
            GetErrors(nameof(NewPassword)).Cast<string>().Any() ||
            GetErrors(nameof(ConfirmNewPassword)).Cast<string>().Any()) return false;
        // All password fields must be non-empty.
        return !string.IsNullOrWhiteSpace(CurrentPassword) &&
               !string.IsNullOrWhiteSpace(NewPassword) &&
               !string.IsNullOrWhiteSpace(ConfirmNewPassword);
    }

    // Changes the user's password.
    private async Task ChangePasswordAsync()
    {
        if (_authenticationService == null || _toastViewModel == null)
        {
            ShowToastMessage("Authentication service is unavailable.", ToastType.Error);
            return;
        }

        // Ensure all password fields are validated if visible.
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
            ShowToastMessage("Please correct password validation errors.", ToastType.Warning);
            return;
        }

        var user = _authenticationService.CurrentUser;
        if (user == null)
        {
            ShowToastMessage("No user is currently logged in.", ToastType.Error);
            return;
        }

        IsChangingPassword = true;
        var model = new ChangePasswordModel { CurrentPassword = this.CurrentPassword, NewPassword = this.NewPassword };

        bool success = false;
        string? failureDetails = null;
        try
        {
            success = await _authenticationService.ChangePasswordAsync(user.Id, model);
            if (!success) failureDetails = "API request failed. Please check your current password.";
        }
        catch (Exception ex)
        {
            failureDetails = ex.Message;
            success = false;
        }
        finally
        {
            IsChangingPassword = false;
        }

        if (success)
        {
            ShowToastMessage("Password changed successfully!", ToastType.Success);
            // Reset password fields and hide section.
            CurrentPassword = "";
            NewPassword = "";
            ConfirmNewPassword = "";
            IsPasswordChangeVisible = false; // Optionally hide after success.
            ClearErrors(nameof(CurrentPassword));
            ClearErrors(nameof(NewPassword));
            ClearErrors(nameof(ConfirmNewPassword));
        }
        else
        {
            ShowToastMessage("Failed to change password.", ToastType.Error, failureDetails);
        }
        (ChangePasswordCommand as RelayCommand)?.RaiseCanExecuteChanged(); // Re-evaluate CanExecute.
    }

    // Validates email format using Regex.
    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false; // Regex timed out.
        }
    }

    // Clears all validation errors.
    private void ClearAllValidationErrors()
    {
        ClearErrors(nameof(FullName));
        ClearErrors(nameof(Email));
        ClearErrors(nameof(Address));
        ClearErrors(nameof(CurrentPassword));
        ClearErrors(nameof(NewPassword));
        ClearErrors(nameof(ConfirmNewPassword));
    }
}