using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Input;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Interfaces;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.ViewModels.Common;

namespace WpfTaskManagerApp.ViewModels;

// ViewModel for adding or editing users.
public class AddEditUserViewModel : ViewModelBase
{
    // Services.
    private readonly IUserService? _userService;
    private readonly ToastNotificationViewModel? _toastViewModel;

    // State.
    private User _editingUserOriginal = new User(Guid.Empty, "", "", UserRole.Staff, ""); // Original user for edit.
    private bool _isEditMode; // True if editing an existing user.
    private bool _isSaving; // True while saving.
    public bool IsSaving
    {
        get => _isSaving;
        set
        {
            if (SetProperty(ref _isSaving, value))
            {
                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    // Window title.
    public string WindowTitle => _isEditMode ? "Edit Staff Member" : "Add New Staff Member";

    // User properties.
    private string _username = string.Empty;
    public string Username
    {
        get => _username;
        set { if (SetProperty(ref _username, value)) ValidateProperty(nameof(Username)); }
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set { if (SetProperty(ref _email, value)) ValidateProperty(nameof(Email)); }
    }

    private string _password = string.Empty;
    public string Password // New password or initial password.
    {
        get => _password;
        set { if (SetProperty(ref _password, value)) ValidateProperty(nameof(Password)); }
    }

    private UserRole _selectedRole;
    public UserRole SelectedRole // Fixed to Staff for this dialog.
    {
        get => _selectedRole;
        private set => SetProperty(ref _selectedRole, value);
    }

    private string _fullName = string.Empty;
    public string FullName
    {
        get => _fullName;
        set { if (SetProperty(ref _fullName, value)) ValidateProperty(nameof(FullName)); }
    }

    private string? _address;
    public string? Address
    {
        get => _address;
        set { if (SetProperty(ref _address, value)) ValidateProperty(nameof(Address)); }
    }

    private bool _isActive;
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    // UI display for password section.
    public bool IsPasswordSectionVisible => true; // Always visible.
    public string PasswordLabel => _isEditMode ? "New Password (leave blank to keep current):" : "Password:";

    // Commands.
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public Action<bool>? CloseActionWithResult { get; set; } // Action to close dialog.

    // Constructor for design-time.
    public AddEditUserViewModel() : this(null, null) { }

    // Main constructor.
    public AddEditUserViewModel(IUserService? userService, ToastNotificationViewModel? toastViewModel)
    {
        _userService = userService;
        _toastViewModel = toastViewModel;
        SaveCommand = new RelayCommand(async (_) => await SaveAsync(), CanSave);
        CancelCommand = new RelayCommand(_ => CloseActionWithResult?.Invoke(false));

        if (IsInDesignModeStatic()) SetupDesignData();
    }

    // Checks if running in design mode.
    private static bool IsInDesignModeStatic() => System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;

    // Sets up data for XAML designer.
    private void SetupDesignData()
    {
        Username = "design.user";
        Email = "design@example.com";
        FullName = "Designer User";
        Address = "123 Design St";
        SelectedRole = UserRole.Staff;
        IsActive = true;
        OnPropertyChanged(nameof(PasswordLabel));
    }

    // Initializes for adding a new user.
    public void InitializeForAdd()
    {
        _isEditMode = false;
        _editingUserOriginal = new User(Guid.NewGuid(), "", "", UserRole.Staff, "", null, true);
        PopulateFieldsFromUser(_editingUserOriginal);
        Password = ""; // Clear password field.
        SelectedRole = UserRole.Staff; // Default to Staff.
        IsActive = true;
        OnPropertyChanged(nameof(WindowTitle));
        OnPropertyChanged(nameof(PasswordLabel));
        ClearAllErrors();
        (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    // Initializes for editing an existing user.
    public void InitializeForEdit(User userToEdit)
    {
        _isEditMode = true;
        _editingUserOriginal = new User(userToEdit.Id, userToEdit.Username, userToEdit.Email, userToEdit.Role, userToEdit.FullName, userToEdit.Address, userToEdit.IsActive)
        { PasswordHash = userToEdit.PasswordHash }; // Keep original hash.
        PopulateFieldsFromUser(_editingUserOriginal);
        Password = ""; // Clear password field (for new password).
        SelectedRole = _editingUserOriginal.Role; // Use original role.
        OnPropertyChanged(nameof(WindowTitle));
        OnPropertyChanged(nameof(PasswordLabel));
        ClearAllErrors();
        (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    // Populates form fields from a User object.
    private void PopulateFieldsFromUser(User user)
    {
        Username = user.Username;
        Email = user.Email;
        FullName = user.FullName;
        Address = user.Address;
        IsActive = user.IsActive;
    }

    // Validates a specific property.
    protected override void ValidateProperty(string? propertyName)
    {
        base.ValidateProperty(propertyName);
        ClearErrors(propertyName);
        switch (propertyName)
        {
            case nameof(Username):
                if (string.IsNullOrWhiteSpace(Username)) AddError(nameof(Username), "Username is required.");
                else if (Username.Length < 3) AddError(nameof(Username), "Username must be at least 3 characters.");
                break;
            case nameof(Email):
                if (string.IsNullOrWhiteSpace(Email)) AddError(nameof(Email), "Email is required.");
                else if (!IsValidEmail(Email)) AddError(nameof(Email), "Invalid email format.");
                break;
            case nameof(FullName):
                if (string.IsNullOrWhiteSpace(FullName)) AddError(nameof(FullName), "Full name is required.");
                break;
            case nameof(Address):
                if (string.IsNullOrWhiteSpace(Address)) AddError(nameof(Address), "Address is required."); // Assuming address is mandatory.
                break;
            case nameof(Password):
                if (!_isEditMode) // New user.
                {
                    if (string.IsNullOrWhiteSpace(Password)) AddError(nameof(Password), "Password is required for new user.");
                    else ValidatePasswordRules(Password);
                }
                else if (!string.IsNullOrWhiteSpace(Password)) // Edit mode, but new password entered.
                {
                    ValidatePasswordRules(Password);
                }
                break;
        }
        (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    // Validates password complexity rules.
    private void ValidatePasswordRules(string passwordToValidate)
    {
        if (passwordToValidate.Length < 6) AddError(nameof(Password), "Password must be at least 6 characters.");
        if (!passwordToValidate.Any(char.IsDigit)) AddError(nameof(Password), "Password must include a digit.");
        if (!passwordToValidate.Any(char.IsLower)) AddError(nameof(Password), "Password must include a lowercase letter.");
        if (!passwordToValidate.Any(char.IsUpper)) AddError(nameof(Password), "Password must include an uppercase letter.");
        if (!passwordToValidate.Any(ch => !char.IsLetterOrDigit(ch))) AddError(nameof(Password), "Password must include a non-alphanumeric character.");
    }

    // Validates all relevant properties.
    protected override void ValidateAllProperties()
    {
        ValidateProperty(nameof(Username));
        ValidateProperty(nameof(Email));
        ValidateProperty(nameof(FullName));
        ValidateProperty(nameof(Address));
        // Password validated only if new user or if a new password is entered for existing user.
        if (!_isEditMode || !string.IsNullOrWhiteSpace(Password))
        {
            ValidateProperty(nameof(Password));
        }
    }

    // Determines if the Save command can execute.
    private bool CanSave(object? parameter)
    {
        if (IsSaving) return false;
        return !HasErrors; // Can save if not currently saving and no validation errors.
    }

    // Saves the user (add or update).
    private async Task SaveAsync()
    {
        ValidateAllProperties();
        if (HasErrors)
        {
            _toastViewModel?.Show("Please correct the validation errors.", ToastType.Warning);
            return;
        }

        if (_userService == null || _toastViewModel == null)
        {
            _toastViewModel?.Show("User service is unavailable.", ToastType.Error);
            return;
        }

        IsSaving = true;
        User userToSave = new User
        {
            Id = _editingUserOriginal.Id,
            Username = this.Username,
            Email = this.Email,
            Role = this.SelectedRole, // Role is fixed to Staff.
            FullName = this.FullName,
            Address = this.Address,
            IsActive = this.IsActive
        };

        bool success = false;
        string successMsg = "";
        string failureMsg = "";

        try
        {
            if (_isEditMode)
            {
                string? newPassword = string.IsNullOrWhiteSpace(Password) ? null : Password;
                success = await _userService.AdminUpdateUserAsync(_editingUserOriginal.Id, userToSave, newPassword);
                successMsg = $"User '{userToSave.Username}' profile updated.";
                if (newPassword != null && success) successMsg += " Password also updated.";
                else if (newPassword != null && !success) failureMsg += "Profile updated, but password change failed."; // Should not happen if API is atomic.

                if (!success && string.IsNullOrEmpty(failureMsg)) failureMsg = $"Failed to update user '{userToSave.Username}'.";
            }
            else // Add new user.
            {
                User? addedUser = await _userService.AddUserAsync(userToSave, this.Password);
                success = addedUser != null;
                if (success) userToSave.Id = addedUser!.Id; // Get ID from added user.
                successMsg = $"User '{userToSave.Username}' added successfully.";
                failureMsg = $"Failed to add user '{userToSave.Username}'.";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AddEditUserViewModel: Method: SaveAsync: {ex.Message}");
            _toastViewModel?.Show($"An error occurred while saving the user.", ToastType.Error);
            success = false;
        }
        finally
        {
            IsSaving = false;
        }

        if (success)
        {
            _toastViewModel?.Show(successMsg, ToastType.Success);
            CloseActionWithResult?.Invoke(true);
        }
        else
        {
            _toastViewModel?.Show(failureMsg, ToastType.Error);
        }
    }

    // Validates email format using Regex.
    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try
        {
            // Basic email regex.
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
    private void ClearAllErrors()
    {
        ClearErrors(nameof(Username));
        ClearErrors(nameof(Email));
        ClearErrors(nameof(FullName));
        ClearErrors(nameof(Address));
        ClearErrors(nameof(Password));
    }
}