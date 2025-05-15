using System.Diagnostics;
using System.Windows.Input;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.Services;
using WpfTaskManagerApp.ViewModels.Common;

namespace WpfTaskManagerApp.ViewModels;
public class AddEditUserViewModel : ViewModelBase
{
    private readonly IUserService? _userService;
    private User _editingUserOriginal = new User(Guid.NewGuid(), "", "", UserRole.Staff, "");
    private bool _isEditMode;
    private bool _isSaving;
    public bool IsSaving
    {
        get => _isSaving;
        set { if (SetProperty(ref _isSaving, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    public string WindowTitle => _isEditMode ? "Edit Staff" : "Add New Staff";

    private string _username = string.Empty;
    public string Username { get => _username; set { if (SetProperty(ref _username, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

    private string _email = string.Empty;
    public string Email { get => _email; set { if (SetProperty(ref _email, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

    private string _password = string.Empty;
    public string Password { get => _password; set { if (SetProperty(ref _password, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

    private UserRole _selectedRole;
    public UserRole SelectedRole { get => _selectedRole; set { if (SetProperty(ref _selectedRole, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }
    public IEnumerable<UserRole> Roles => Enum.GetValues(typeof(UserRole)).Cast<UserRole>();

    private string _fullName = string.Empty;
    public string FullName { get => _fullName; set { if (SetProperty(ref _fullName, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

    private string? _address;
    public string? Address { get => _address; set { if (SetProperty(ref _address, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

    private bool _isActive;
    public bool IsActive { get => _isActive; set { if (SetProperty(ref _isActive, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

    public bool IsPasswordSectionVisible => !_isEditMode;

    private string? _errorMessage;
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    public ICommand SaveCommand { get; }
    public Action<bool>? CloseActionWithResult { get; set; }

    public AddEditUserViewModel()
    {
        _userService = null;
        Username = "design.user";
        Email = "design@example.com";
        FullName = "Design User";
        SelectedRole = UserRole.Staff;
        IsActive = true;
        SaveCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
    }

    public AddEditUserViewModel(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        SaveCommand = new RelayCommand(async (_) => await SaveAsync(), CanSave);
    }

    public void InitializeForAdd()
    {
        _isEditMode = false;
        _editingUserOriginal = new User(Guid.NewGuid(), "", "", UserRole.Staff, "", null, true);
        PopulateFieldsFromUser(_editingUserOriginal);
        Password = "";
        OnPropertyChanged(nameof(WindowTitle));
        OnPropertyChanged(nameof(IsPasswordSectionVisible));
        (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    public void InitializeForEdit(User userToEdit)
    {
        _isEditMode = true;
        _editingUserOriginal = new User(
            userToEdit.Id, userToEdit.Username, userToEdit.Email,
            userToEdit.Role, userToEdit.FullName, userToEdit.Address, userToEdit.IsActive
        );
        _editingUserOriginal.PasswordHash = userToEdit.PasswordHash;

        PopulateFieldsFromUser(_editingUserOriginal);
        Password = "";
        OnPropertyChanged(nameof(WindowTitle));
        OnPropertyChanged(nameof(IsPasswordSectionVisible));
        (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private void PopulateFieldsFromUser(User user)
    {
        Username = user.Username;
        Email = user.Email;
        FullName = user.FullName;
        SelectedRole = user.Role;
        Address = user.Address;
        IsActive = user.IsActive;
        ErrorMessage = null;
    }

    private bool CanSave(object? arg)
    {
        if (IsSaving) return false;

        if (string.IsNullOrWhiteSpace(Username) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(FullName))
            return false;

        if (!_isEditMode && string.IsNullOrWhiteSpace(Password))
            return false;

        if (_isEditMode)
        {
            return Username != _editingUserOriginal.Username ||
                   Email != _editingUserOriginal.Email ||
                   FullName != _editingUserOriginal.FullName ||
                   SelectedRole != _editingUserOriginal.Role ||
                   Address != _editingUserOriginal.Address ||
                   IsActive != _editingUserOriginal.IsActive;
        }
        return true;
    }

    private async Task SaveAsync()
    {
        if (_userService == null) { ErrorMessage = "User service not available."; return; }
        ErrorMessage = null;
        if (!CanSave(null))
        {
            ErrorMessage = "Please fill all required fields or make a change to save.";
            return;
        }
        IsSaving = true;

        User userToSave = new User
        {
            Id = _isEditMode ? _editingUserOriginal.Id : Guid.NewGuid(),
            Username = this.Username,
            Email = this.Email,
            Role = this.SelectedRole,
            FullName = this.FullName,
            Address = this.Address,
            IsActive = this.IsActive,
            PasswordHash = _isEditMode ? _editingUserOriginal.PasswordHash : null
        };

        User? result = null;
        bool success = false;
        try
        {
            if (_isEditMode)
            {
                success = await _userService.UpdateUserAsync(userToSave);
            }
            else
            {
                result = await _userService.AddUserAsync(userToSave, this.Password);
                success = result != null;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error saving user: {ex.Message}");
            ErrorMessage = "An unexpected error occurred while saving.";
            success = false;
        }
        finally
        {
            IsSaving = false;
        }

        CloseActionWithResult?.Invoke(success);
        if (!success && string.IsNullOrEmpty(ErrorMessage))
        {
            ErrorMessage = _isEditMode ? "Failed to update user. Username or Email might be in use, or no changes were detected."
                                       : "Failed to add user. Username or Email might already exist.";
        }
    }
}