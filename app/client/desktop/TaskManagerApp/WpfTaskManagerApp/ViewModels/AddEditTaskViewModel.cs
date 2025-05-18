using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.Services;
using WpfTaskManagerApp.ViewModels.Common;
using TaskStatus = WpfTaskManagerApp.Core.TaskStatus;
namespace WpfTaskManagerApp.ViewModels;
public class AddEditTaskViewModel : ViewModelBase
{
    private readonly ITaskService? _taskService;
    private readonly IUserService? _userService;
    private readonly IAuthenticationService? _authenticationService;
    private readonly ToastNotificationViewModel? _toastViewModel;
    private TaskItem _editingTaskOriginal = new TaskItem(Guid.NewGuid(), "", "");
    private bool _isEditMode;
    private bool _isSaving;
    public bool IsSaving
    {
        get => _isSaving;
        set
        {
            if (SetProperty(ref _isSaving, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

    public string WindowTitle => _isEditMode ? "Edit Task" : "Add New Task";

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set
        {
            if (SetProperty(ref _title, value)) ValidateProperty(nameof(Title));
        }
    }

    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private TaskStatus _selectedStatus;
    public TaskStatus SelectedStatus
    {
        get => _selectedStatus;
        set => SetProperty(ref _selectedStatus, value);
    }
    public IEnumerable<TaskStatus> AllTaskStatuses => Enum.GetValues(typeof(TaskStatus)).Cast<TaskStatus>();

    private ObservableCollection<User> _allAssignableUsers = new ObservableCollection<User>();

    private ObservableCollection<User> _filteredAssignableUsers = new ObservableCollection<User>();
    public ObservableCollection<User> FilteredAssignableUsers
    {
        get => _filteredAssignableUsers;
        set => SetProperty(ref _filteredAssignableUsers, value);
    }

    private string _assigneeSearchText = string.Empty;
    private bool _isUpdatingTextFromSelection = false; // Cờ để biết Text đang được cập nhật từ SelectedItem

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

                // Nếu text thay đổi không khớp với selected user, set SelectedAssignee = null
                if (SelectedAssignee != null && SelectedAssignee.FullName != _assigneeSearchText)
                {
                    Debug.WriteLine($"AssigneeSearchText set SelectedAssignee to null: {_assigneeSearchText}");
                    SelectedAssignee = null;
                }
            }
        }
    }

    private User? _selectedAssignee;
    public User? SelectedAssignee
    {
        get => _selectedAssignee;
        set
        {
            // Sử dụng SetProperty để gọi ValidateProperty và OnPropertyChanged
            if (SetProperty(ref _selectedAssignee, value, nameof(SelectedAssignee)))
            {
                _isUpdatingTextFromSelection = true;
                AssigneeSearchText = _selectedAssignee?.FullName ?? string.Empty;
                _isUpdatingTextFromSelection = false;

                // Sau khi chọn, danh sách filter có thể chỉ cần hiển thị item đã chọn hoặc giữ nguyên filter hiện tại
                // FilterAssignableUsers(); // Cân nhắc có nên filter lại ở đây không

                (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    private DateTime? _dueDate;
    public DateTime? DueDate
    {
        get => _dueDate;
        set => SetProperty(ref _dueDate, value);
    }

    private bool _isActive = true;
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    // ***** THUỘC TÍNH ĐỂ KIỂM SOÁT CHẾ ĐỘ CHỈ ĐỌC *****
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

    // SelectedStatus luôn có thể chỉnh sửa
    // ***** KẾT THÚC THUỘC TÍNH CHỈ ĐỌC *****

    public ICommand SaveCommand
    {
        get;
    }
    public ICommand CancelCommand
    {
        get;
    }
    public Action<bool>? CloseActionWithResult
    {
        get;
        set;
    }

    public AddEditTaskViewModel() : this(null, null, null, null) { }
    public AddEditTaskViewModel(ITaskService? taskService, IUserService? userService, ToastNotificationViewModel? toastViewModel, IAuthenticationService? authenticationService)
    {
        _taskService = taskService;
        _userService = userService;
        _authenticationService = authenticationService;
        _toastViewModel = toastViewModel;
        SaveCommand = new RelayCommand(async (_) => await SaveAsync(), CanSave);
        CancelCommand = new RelayCommand(_ => CloseActionWithResult?.Invoke(false));

        if (IsInDesignModeStatic())
        {
            Title = "Design Task";
            _allAssignableUsers.Add(new User(Guid.NewGuid(), "designer1", "d1@e.c", UserRole.Staff, "Designer Alice"));
            FilterAssignableUsers();
            SelectedAssignee = _allAssignableUsers.FirstOrDefault();
            SelectedStatus = TaskStatus.ToDo;
            DueDate = DateTime.Now.AddDays(7);
            IsStatusOnlyEditMode = false; // Mặc định cho design-time
            UpdateReadOnlyStates();
        }
    }
    private static bool IsInDesignModeStatic()
    {
        return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;
    }

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
                // Khi edit, set SelectedAssignee, điều này sẽ tự động cập nhật AssigneeSearchText
                SelectedAssignee = _allAssignableUsers.FirstOrDefault(u => u.Id == _editingTaskOriginal.AssigneeId.Value);
            }
            else
            {
                // Khi thêm mới, reset cả hai
                AssigneeSearchText = string.Empty;
                SelectedAssignee = null;
                FilterAssignableUsers(); // Hiển thị tất cả user ban đầu
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading assignable users: {ex.Message}");
            _toastViewModel?.Show("Error loading users for assignment.", ToastType.Error);
        }
    }

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

    public void InitializeForAdd()
    {
        _isEditMode = false;
        IsStatusOnlyEditMode = false; // Khi thêm mới, không phải chỉ sửa status
        _editingTaskOriginal = new TaskItem(Guid.NewGuid(), "", "", TaskStatus.ToDo, true)
        {
            CreatedDate = DateTime.UtcNow
        };
        PopulateFieldsFromTask(_editingTaskOriginal);
        AssigneeSearchText = string.Empty;
        Debug.Write("InitializeForAdd set assinee to null");
        SelectedAssignee = null;
        SelectedStatus = TaskStatus.ToDo;
        IsActive = true;
        DueDate = null;
        OnPropertyChanged(nameof(WindowTitle));
        ClearAllErrors();
        (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
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

    private void UpdateReadOnlyStates()
    {
        OnPropertyChanged(nameof(CanEditTitle));
        OnPropertyChanged(nameof(CanEditDescription));
        OnPropertyChanged(nameof(CanEditAssignee));
        OnPropertyChanged(nameof(CanEditDueDate));
        OnPropertyChanged(nameof(CanEditIsActive));
        OnPropertyChanged(nameof(WindowTitle));
    }

    private void PopulateFieldsFromTask(TaskItem task)
    {
        Title = task.Title;
        Description = task.Description;
        SelectedStatus = task.Status;
        DueDate = task.DueDate;
        IsActive = task.IsActive;
    }

    protected override void ValidateProperty(string? propertyName)
    {
        base.ValidateProperty(propertyName);
        ClearErrors(propertyName);
        switch (propertyName)
        {
            case nameof(Title):
                if (string.IsNullOrWhiteSpace(Title)) AddError(nameof(Title), "Title is required.");
                break;
            case nameof(SelectedAssignee):
                if (SelectedAssignee == null) AddError(nameof(SelectedAssignee), "Assignee is required.");
                break;
            case nameof(SelectedStatus): // Status luôn có thể thay đổi, không cần validation ở đây
                break;
        }
        (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
    protected override void ValidateAllProperties()
    {
        if (!IsStatusOnlyEditMode) // Chỉ validate các trường khác nếu không phải chế độ chỉ sửa status
        {
            ValidateProperty(nameof(Title));
            ValidateProperty(nameof(SelectedAssignee));
        }
    }
    private bool CanSave(object? parameter)
    {
        if (IsSaving) return false;

        // Cho phép lưu nếu chỉ thay đổi status khi là staff
        if (IsStatusOnlyEditMode)
        {
            return SelectedStatus != _editingTaskOriginal.Status;
        }

        return !HasErrors && HasChangesForFullEdit();
    }

    private bool HasChangesForFullEdit() // Kiểm tra xem có thay đổi nào không (cho chế độ edit đầy đủ)
    {
        if (!_isEditMode) return true; // Luôn cho phép lưu khi thêm mới (nếu không có lỗi)
        return Title != _editingTaskOriginal.Title ||
            Description != _editingTaskOriginal.Description ||
            SelectedStatus != _editingTaskOriginal.Status ||
            SelectedAssignee?.Id != _editingTaskOriginal.AssigneeId ||
            DueDate != _editingTaskOriginal.DueDate ||
            IsActive != _editingTaskOriginal.IsActive;
    }

    private async Task SaveAsync()
    {
        if (IsStatusOnlyEditMode) // Nếu chỉ sửa status, không cần validate các trường khác
        {
            if (SelectedStatus == _editingTaskOriginal.Status)
            {
                _toastViewModel?.Show("No changes made to the status.", ToastType.Information);
                CloseActionWithResult?.Invoke(false); // Không có gì để lưu
                return;
            }
        }
        else // Chế độ thêm mới hoặc admin sửa
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
            else // Admin edit hoặc Add new
            {
                TaskItem taskToSave = new TaskItem
                {
                    Id = _isEditMode ? _editingTaskOriginal.Id : Guid.NewGuid(),
                    Title = this.Title,
                    Description = this.Description,
                    Status = this.SelectedStatus,
                    IsActive = this.IsActive,
                    AssigneeId = SelectedAssignee!.Id, // Sẽ không null nếu không phải IsStatusOnlyEditMode và đã qua validation
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
        catch (Exception ex)
        {
            failureMsg = "Error saving task.";
            Debug.WriteLine($"SaveTaskAsync Error: {ex}");
            success = false;
        }
        finally
        {
            IsSaving = false;
        }

        if (success)
        {
            _toastViewModel.Show(successMsg, ToastType.Success);
            CloseActionWithResult?.Invoke(true);
        }
        else
        {
            _toastViewModel.Show(failureMsg, ToastType.Error);
        }
    }
    private void ClearAllErrors()
    {
        ClearErrors(nameof(Title));
        ClearErrors(nameof(SelectedAssignee));
        ClearErrors(nameof(SelectedStatus));
    }
}