using System.Collections.ObjectModel; 
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using WpfTaskManagerApp.Core; 
using WpfTaskManagerApp.Interfaces; 
using WpfTaskManagerApp.Models; 
using WpfTaskManagerApp.ViewModels.Common; 
using TaskStatus = WpfTaskManagerApp.Core.TaskStatus; 

namespace WpfTaskManagerApp.ViewModels;

// ViewModel for adding or editing tasks.
public class AddEditTaskViewModel : ViewModelBase
{
    private readonly ITaskService? _taskService;
    private readonly IUserService? _userService;
    private readonly IAuthenticationService? _authenticationService;
    private readonly ToastNotificationViewModel? _toastViewModel;
    // Stores original task data for edit comparison.
    private TaskItem _editingTaskOriginal = new TaskItem(Guid.NewGuid(), "", "");
    // Indicates if in edit mode.
    private bool _isEditMode;
    // Indicates if save operation is in progress.
    private bool _isSaving;
    public bool IsSaving
    {
        get => _isSaving;
        set
        {
            if (SetProperty(ref _isSaving, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    // Window title, changes based on edit/add mode.
    public string WindowTitle => _isEditMode ? "Edit Task" : "Add New Task";

    // Task title.
    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set
        {
            if (SetProperty(ref _title, value)) ValidateProperty(nameof(Title));
        }
    }

    // Task description.
    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set
        {
            if (SetProperty(ref _description, value)) ValidateProperty(nameof(Description));
        }
    }

    // Selected task status.
    private TaskStatus _selectedStatus;
    public TaskStatus SelectedStatus
    {
        get => _selectedStatus;
        set => SetProperty(ref _selectedStatus, value);
    }
    // Collection of all possible task statuses.
    public IEnumerable<TaskStatus> AllTaskStatuses => Enum.GetValues(typeof(TaskStatus)).Cast<TaskStatus>();

    // Full list of users who can be assigned tasks.
    private ObservableCollection<User> _allAssignableUsers = new ObservableCollection<User>();

    // Filtered list of assignable users for UI display.
    private ObservableCollection<User> _filteredAssignableUsers = new ObservableCollection<User>();
    public ObservableCollection<User> FilteredAssignableUsers
    {
        get => _filteredAssignableUsers;
        set => SetProperty(ref _filteredAssignableUsers, value);
    }

    // Search text for filtering assignees.
    private string _assigneeSearchText = string.Empty;
    // Flag to manage updates between search text and selection.
    private bool _isUpdatingTextFromSelection = false;

    public string AssigneeSearchText
    {
        get => _assigneeSearchText;
        set
        {
            if (_assigneeSearchText != value)
            {
                _assigneeSearchText = value;
                OnPropertyChanged();
                FilterAssignableUsers();
                if (SelectedAssignee != null && SelectedAssignee.FullName != _assigneeSearchText)
                {
                    SelectedAssignee = null;
                }
            }
        }
    }

    // Currently selected assignee for the task.
    private User? _selectedAssignee;
    public User? SelectedAssignee
    {
        get => _selectedAssignee;
        set
        {
            if (SetProperty(ref _selectedAssignee, value, nameof(SelectedAssignee)))
            {
                _isUpdatingTextFromSelection = true;
                AssigneeSearchText = _selectedAssignee?.FullName ?? string.Empty;
                _isUpdatingTextFromSelection = false;
                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    // Task due date.
    private DateTime? _dueDate;
    public DateTime? DueDate
    {
        get => _dueDate;
        set => SetProperty(ref _dueDate, value);
    }

    // Task active status.
    private bool _isActive = true;
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    // Indicates if only task status can be edited.
    private bool _isStatusOnlyEditMode = false;
    public bool IsStatusOnlyEditMode
    {
        get => _isStatusOnlyEditMode;
        private set => SetProperty(ref _isStatusOnlyEditMode, value);
    }

    public bool CanEditTitle => !_isStatusOnlyEditMode;
    public bool CanEditDescription => !_isStatusOnlyEditMode;
    public bool CanEditAssignee => !_isStatusOnlyEditMode;
    public bool CanEditDueDate => !_isStatusOnlyEditMode;
    public bool CanEditIsActive => !_isStatusOnlyEditMode;
    public bool CanEditStatus => _authenticationService?.CurrentUser?.Role == UserRole.Admin ||
                                 (_isEditMode && _editingTaskOriginal.AssigneeId == _authenticationService?.CurrentUser?.Id);

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    // Action to close the dialog window.
    public Action<bool>? CloseActionWithResult { get; set; }

    // Constructor for design-time support.
    public AddEditTaskViewModel() : this(null, null, null, null) { }
    // Main constructor.
    public AddEditTaskViewModel(ITaskService? taskService, IUserService? userService, ToastNotificationViewModel? toastViewModel, IAuthenticationService? authenticationService)
    {
        _taskService = taskService;
        _userService = userService;
        _authenticationService = authenticationService;
        _toastViewModel = toastViewModel;
        SaveCommand = new RelayCommand(async (_) => await SaveAsync(), CanSave);
        CancelCommand = new RelayCommand(_ => CloseActionWithResult?.Invoke(false));

        if (IsInDesignModeStatic()) // Setup for XAML designer.
        {
            Title = "Design Task";
            _allAssignableUsers.Add(new User(Guid.NewGuid(), "designer1", "d1@e.c", UserRole.Staff, "Designer Alice"));
            FilterAssignableUsers();
            SelectedAssignee = _allAssignableUsers.FirstOrDefault();
            SelectedStatus = TaskStatus.ToDo;
            DueDate = DateTime.Now.AddDays(7);
            IsStatusOnlyEditMode = false;
            UpdateReadOnlyStates();
        }
    }
    // Checks if running in design mode.
    private static bool IsInDesignModeStatic()
    {
        return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
    }

    // Loads users that can be assigned to tasks.
    public async Task LoadAssignableUsersAsync()
    {
        if (_userService == null) return;
        try
        {
            var paginatedResult = await _userService.GetUsersAsync(skip: 0, limit: 1000, includeInactive: false);
            _allAssignableUsers.Clear();
            if (paginatedResult?.PaginatedData != null)
            {
                foreach (var user in paginatedResult.PaginatedData.OrderBy(u => u.FullName)) _allAssignableUsers.Add(user);
            }

            if (_isEditMode && _editingTaskOriginal.AssigneeId.HasValue)
            {
                SelectedAssignee = _allAssignableUsers.FirstOrDefault(u => u.Id == _editingTaskOriginal.AssigneeId.Value);
            }
            else
            {
                AssigneeSearchText = string.Empty;
                SelectedAssignee = null;
                FilterAssignableUsers();
            }
        }
        catch (Exception ex)
        {
             Debug.WriteLine($"LoadAssignableUsersAsync Error: {ex}");
            _toastViewModel?.Show("Error loading users for assignment.", ToastType.Error);
        }
    }

    // Filters assignable users based on search text.
    private void FilterAssignableUsers()
    {
        if (string.IsNullOrWhiteSpace(AssigneeSearchText))
        {
            FilteredAssignableUsers = new ObservableCollection<User>(_allAssignableUsers.OrderBy(u => u.FullName));
        }
        else
        {
            var searchTextLower = AssigneeSearchText.ToLower();
            FilteredAssignableUsers = new ObservableCollection<User>(
                _allAssignableUsers.Where(u => (u.FullName != null && u.FullName.ToLower().Contains(searchTextLower)) ||
                    (u.Username != null && u.Username.ToLower().Contains(searchTextLower)))
                .OrderBy(u => u.FullName)
            );
        }
    }

    // Initializes ViewModel for adding a new task.
    public void InitializeForAdd()
    {
        _isEditMode = false;
        IsStatusOnlyEditMode = false;
        _editingTaskOriginal = new TaskItem(Guid.NewGuid(), "", "", TaskStatus.ToDo, true)
        {
            CreatedDate = DateTime.UtcNow
        };
        PopulateFieldsFromTask(_editingTaskOriginal);
        AssigneeSearchText = string.Empty;
        SelectedAssignee = null;
        SelectedStatus = TaskStatus.ToDo;
        IsActive = true;
        DueDate = null;
        OnPropertyChanged(nameof(WindowTitle));
        ClearAllErrors();
        (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
    // Initializes ViewModel for editing an existing task.
    public void InitializeForEdit(TaskItem taskToEdit)
    {
        _isEditMode = true;
        _editingTaskOriginal = new TaskItem(taskToEdit.Id, taskToEdit.Title, taskToEdit.Description, taskToEdit.Status, taskToEdit.IsActive)
        { AssigneeId = taskToEdit.AssigneeId, AssigneeName = taskToEdit.AssigneeName, AssigneeUsername = taskToEdit.AssigneeUsername, CreatedDate = taskToEdit.CreatedDate, DueDate = taskToEdit.DueDate };
        PopulateFieldsFromTask(_editingTaskOriginal);

        var currentUser = _authenticationService?.CurrentUser;
        if (currentUser?.Role == UserRole.Staff && currentUser.Id == taskToEdit.AssigneeId)
        {
            IsStatusOnlyEditMode = true;
            OnPropertyChanged(nameof(CanEditStatus));
        }
        else
        {
            IsStatusOnlyEditMode = false;
        }

        OnPropertyChanged(nameof(WindowTitle));
        ClearAllErrors();
        (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    // Updates properties related to read-only state of fields.
    private void UpdateReadOnlyStates()
    {
        OnPropertyChanged(nameof(CanEditTitle));
        OnPropertyChanged(nameof(CanEditDescription));
        OnPropertyChanged(nameof(CanEditAssignee));
        OnPropertyChanged(nameof(CanEditDueDate));
        OnPropertyChanged(nameof(CanEditIsActive));
        OnPropertyChanged(nameof(WindowTitle));
    }

    // Populates form fields from a TaskItem object.
    private void PopulateFieldsFromTask(TaskItem task)
    {
        Title = task.Title;
        Description = task.Description;
        SelectedStatus = task.Status;
        DueDate = task.DueDate;
        IsActive = task.IsActive;
    }

    // Validates a specific property.
    protected override void ValidateProperty(string? propertyName)
    {
        base.ValidateProperty(propertyName);
        ClearErrors(propertyName);
        switch (propertyName)
        {
            case nameof(Title):
                if (string.IsNullOrWhiteSpace(Title)) AddError(nameof(Title), "Title is required.");
                break;
            case nameof(Description):
                if (string.IsNullOrWhiteSpace(Description)) AddError(nameof(Description), "Description is required.");
                break;
            case nameof(SelectedAssignee):
                if (SelectedAssignee == null) AddError(nameof(SelectedAssignee), "Assignee is required.");
                break;
            case nameof(SelectedStatus):
                break;
        }
        (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
    // Validates all relevant properties.
    protected override void ValidateAllProperties()
    {
        if (!IsStatusOnlyEditMode)
        {
            ValidateProperty(nameof(Title));
            ValidateProperty(nameof(Description));
            ValidateProperty(nameof(SelectedAssignee));
        }
    }
    // Determines if the Save command can execute.
    private bool CanSave(object? parameter)
    {
        if (IsSaving) return false;
        if (IsStatusOnlyEditMode)
        {
            return SelectedStatus != _editingTaskOriginal.Status;
        }
        return !HasErrors && HasChangesForFullEdit();
    }

    // Checks if any changes were made in full edit mode.
    private bool HasChangesForFullEdit()
    {
        if (!_isEditMode) return true;
        return Title != _editingTaskOriginal.Title ||
            Description != _editingTaskOriginal.Description ||
            SelectedStatus != _editingTaskOriginal.Status ||
            SelectedAssignee?.Id != _editingTaskOriginal.AssigneeId ||
            DueDate != _editingTaskOriginal.DueDate ||
            IsActive != _editingTaskOriginal.IsActive;
    }

    // Saves the task (add or update).
    private async Task SaveAsync()
    {
        if (IsStatusOnlyEditMode)
        {
            if (SelectedStatus == _editingTaskOriginal.Status)
            {
                _toastViewModel?.Show("No changes made to the status.", ToastType.Information);
                CloseActionWithResult?.Invoke(false);
                return;
            }
        }
        else
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                _toastViewModel?.Show("Please correct the validation errors.", ToastType.Warning);
                return;
            }
        }

        if (_taskService == null || _toastViewModel == null)
        {
            _toastViewModel?.Show("Task service unavailable.", ToastType.Error);
            return;
        }

        IsSaving = true;
        bool success = false;
        string successMsg = "";
        string failureMsg = "";
        try
        {
            if (_isEditMode && IsStatusOnlyEditMode)
            {
                success = await _taskService.UpdateTaskStatusAsync(_editingTaskOriginal.Id, SelectedStatus);
                successMsg = $"Task '{_editingTaskOriginal.Title}' status updated.";
                failureMsg = $"Failed to update status for task '{_editingTaskOriginal.Title}'.";
            }
            else
            {
                TaskItem taskToSave = new TaskItem
                {
                    Id = _isEditMode ? _editingTaskOriginal.Id : Guid.NewGuid(),
                    Title = this.Title,
                    Description = this.Description,
                    Status = this.SelectedStatus,
                    IsActive = this.IsActive,
                    AssigneeId = SelectedAssignee!.Id,
                    DueDate = DueDate,
                    CreatedDate = _isEditMode ? _editingTaskOriginal.CreatedDate : DateTime.UtcNow
                };
                if (_isEditMode)
                {
                    success = await _taskService.UpdateTaskAsync(taskToSave);
                    successMsg = $"Task '{taskToSave.Title}' updated.";
                    failureMsg = $"Failed to update task '{taskToSave.Title}'.";
                }
                else
                {
                    TaskItem? addedTask = await _taskService.AddTaskAsync(taskToSave);
                    success = addedTask != null;
                    successMsg = $"Task '{taskToSave.Title}' added.";
                    failureMsg = $"Failed to add task '{taskToSave.Title}'.";
                }
            }
        }
        catch (Exception)
        {
            failureMsg = "Error saving task.";
            // Debug.WriteLine($"SaveTaskAsync Error: {ex}"); // Removed
            _toastViewModel?.Show(failureMsg, ToastType.Error); // Corrected toast call
            success = false;
        }
        finally
        {
            IsSaving = false;
        }

        if (success)
        {
            _toastViewModel.Show(successMsg, ToastType.Success); // Corrected toast call
            CloseActionWithResult?.Invoke(true);
        }
        else
        {
            _toastViewModel.Show(failureMsg, ToastType.Error); // Corrected toast call
        }
    }
    // Clears all validation errors for the form.
    private void ClearAllErrors()
    {
        ClearErrors(nameof(Title));
        ClearErrors(nameof(SelectedAssignee));
        ClearErrors(nameof(SelectedStatus));
    }
}