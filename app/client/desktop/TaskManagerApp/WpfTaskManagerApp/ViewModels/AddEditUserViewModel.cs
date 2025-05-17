using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Input;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.Services;
using WpfTaskManagerApp.ViewModels.Common;

namespace WpfTaskManagerApp.ViewModels;
// --- AddEditUserViewModel ---
public class AddEditUserViewModel : ViewModelBase
{
    private readonly IUserService? _userService;
    private readonly ToastNotificationViewModel? _toastViewModel;
    private User _editingUserOriginal = new User(Guid.Empty, "", "", UserRole.Staff, "");
    private bool _isEditMode;
    private bool _isSaving;
    public bool IsSaving { get => _isSaving; set { if (SetProperty(ref _isSaving, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }
    public string WindowTitle => _isEditMode ? "Edit Staff Member" : "Add New Staff Member";
    private string _username = string.Empty;
    public string Username { get => _username; set { if (SetProperty(ref _username, value)) ValidateProperty(nameof(Username)); } }
    private string _email = string.Empty;
    public string Email { get => _email; set { if (SetProperty(ref _email, value)) ValidateProperty(nameof(Email)); } }
    private string _password = string.Empty;
    public string Password { get => _password; set { if (SetProperty(ref _password, value)) ValidateProperty(nameof(Password)); } }
    private UserRole _selectedRole;
    public UserRole SelectedRole { get => _selectedRole; private set => SetProperty(ref _selectedRole, value); }
    private string _fullName = string.Empty;
    public string FullName { get => _fullName; set { if (SetProperty(ref _fullName, value)) ValidateProperty(nameof(FullName)); } }
    private string? _address;
    public string? Address { get => _address; set { if (SetProperty(ref _address, value)) ValidateProperty(nameof(Address)); } }
    private bool _isActive;
    public bool IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }
    public bool IsPasswordSectionVisible => true;
    public string PasswordLabel => _isEditMode ? "New Password (leave blank to keep current):" : "Password:";
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public Action<bool>? CloseActionWithResult { get; set; }

    public AddEditUserViewModel() : this(null, null) { }
    public AddEditUserViewModel(IUserService? userService, ToastNotificationViewModel? toastViewModel)
    {
        _userService = userService; _toastViewModel = toastViewModel;
        SaveCommand = new RelayCommand(async (param) => await SaveAsync(), CanSave);
        CancelCommand = new RelayCommand(_ => CloseActionWithResult?.Invoke(false));
        if (IsInDesignModeStatic()) { Username = "d.user"; Email = "d@e.c"; FullName = "D User"; Address = "123 D St"; SelectedRole = UserRole.Staff; IsActive = true; OnPropertyChanged(nameof(PasswordLabel)); }
    }
    private static bool IsInDesignModeStatic() { return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime; }
    public void InitializeForAdd() { _isEditMode = false; _editingUserOriginal = new User(Guid.NewGuid(), "", "", UserRole.Staff, "", null, true); PopulateFieldsFromUser(_editingUserOriginal); Password = ""; SelectedRole = UserRole.Staff; IsActive = true; OnPropertyChanged(nameof(WindowTitle)); OnPropertyChanged(nameof(IsPasswordSectionVisible)); OnPropertyChanged(nameof(PasswordLabel)); ClearAllErrors(); (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    public void InitializeForEdit(User userToEdit) { _isEditMode = true; _editingUserOriginal = new User(userToEdit.Id, userToEdit.Username, userToEdit.Email, userToEdit.Role, userToEdit.FullName, userToEdit.Address, userToEdit.IsActive) { PasswordHash = userToEdit.PasswordHash }; PopulateFieldsFromUser(_editingUserOriginal); Password = ""; SelectedRole = _editingUserOriginal.Role; OnPropertyChanged(nameof(WindowTitle)); OnPropertyChanged(nameof(IsPasswordSectionVisible)); OnPropertyChanged(nameof(PasswordLabel)); ClearAllErrors(); (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    private void PopulateFieldsFromUser(User user) { Username = user.Username; Email = user.Email; FullName = user.FullName; Address = user.Address; IsActive = user.IsActive; }
    protected override void ValidateProperty(string? propertyName)
    {
        base.ValidateProperty(propertyName); ClearErrors(propertyName);
        switch (propertyName)
        {
            case nameof(Username): if (string.IsNullOrWhiteSpace(Username)) AddError(nameof(Username), "Username is required."); else if (Username.Length < 3) AddError(nameof(Username), "Min 3 characters."); break;
            case nameof(Email): if (string.IsNullOrWhiteSpace(Email)) AddError(nameof(Email), "Email is required."); else if (!IsValidEmail(Email)) AddError(nameof(Email), "Invalid email."); break;
            case nameof(FullName): if (string.IsNullOrWhiteSpace(FullName)) AddError(nameof(FullName), "Full name is required."); break;
            case nameof(Address): if (string.IsNullOrWhiteSpace(Address)) AddError(nameof(Address), "Address is required."); break;
            case nameof(Password): if (!_isEditMode) { if (string.IsNullOrWhiteSpace(Password)) AddError(nameof(Password), "Password required for new user."); else ValidatePasswordRules(Password); } else if (!string.IsNullOrWhiteSpace(Password)) { ValidatePasswordRules(Password); } break;
        }
        (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
    private void ValidatePasswordRules(string passwordToValidate) { if (passwordToValidate.Length < 6) AddError(nameof(Password), "Min 6 chars."); if (!passwordToValidate.Any(char.IsDigit)) AddError(nameof(Password), "Requires digit."); if (!passwordToValidate.Any(char.IsLower)) AddError(nameof(Password), "Requires lowercase."); if (!passwordToValidate.Any(char.IsUpper)) AddError(nameof(Password), "Requires uppercase."); if (!passwordToValidate.Any(ch => !char.IsLetterOrDigit(ch))) AddError(nameof(Password), "Requires non-alphanumeric."); }
    protected override void ValidateAllProperties() { ValidateProperty(nameof(Username)); ValidateProperty(nameof(Email)); ValidateProperty(nameof(FullName)); ValidateProperty(nameof(Address)); if (!_isEditMode || !string.IsNullOrWhiteSpace(Password)) ValidateProperty(nameof(Password)); }
    private bool CanSave(object? parameter) { if (IsSaving) return false; return !HasErrors; }
    private async Task SaveAsync()
    {
        ValidateAllProperties(); if (HasErrors) { _toastViewModel?.Show("Please correct validation errors.", ToastType.Warning); return; }
        if (_userService == null || _toastViewModel == null) { _toastViewModel?.Show("Service unavailable.", ToastType.Error); return; }
        IsSaving = true;
        User userToSave = new User { Id = _editingUserOriginal.Id, Username = this.Username, Email = this.Email, Role = this.SelectedRole, FullName = this.FullName, Address = this.Address, IsActive = this.IsActive };
        bool success = false; string successMsg = "", failureMsg = "";
        try
        {
            if (_isEditMode) { string? newPwd = string.IsNullOrWhiteSpace(Password) ? null : Password; success = await _userService.AdminUpdateUserAsync(_editingUserOriginal.Id, userToSave, newPwd); successMsg = $"User '{userToSave.Username}' updated."; if (newPwd != null && success) successMsg += " Password also updated."; else if (newPwd != null && !success) failureMsg += " Profile updated, but failed to set new password."; if (!success && string.IsNullOrEmpty(failureMsg)) failureMsg = $"Failed to update '{userToSave.Username}'."; }
            else { User? addedUser = await _userService.AddUserAsync(userToSave, this.Password); success = addedUser != null; if (success) userToSave.Id = addedUser!.Id; successMsg = $"User '{userToSave.Username}' added."; failureMsg = $"Failed to add '{userToSave.Username}'."; }
        }
        catch (Exception ex) { failureMsg = "Error saving user."; Debug.WriteLine($"SaveAsync Error: {ex}"); success = false; }
        finally { IsSaving = false; }
        if (success) { _toastViewModel.Show(successMsg, ToastType.Success); CloseActionWithResult?.Invoke(true); }
        else { _toastViewModel.Show(failureMsg, ToastType.Error); }
    }
    private bool IsValidEmail(string email) { if (string.IsNullOrWhiteSpace(email)) return false; try { return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)); } catch (RegexMatchTimeoutException) { return false; } }
    private void ClearAllErrors() { ClearErrors(nameof(Username)); ClearErrors(nameof(Email)); ClearErrors(nameof(FullName)); ClearErrors(nameof(Address)); ClearErrors(nameof(Password)); }
}
