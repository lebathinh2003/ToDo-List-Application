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
    private TaskItem _editingTaskOriginal = new TaskItem(Guid.NewGuid(), "", "");
    private bool _isEditMode;
    private bool _isSaving;
    public bool IsSaving
    {
        get => _isSaving;
        set { if (SetProperty(ref _isSaving, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }

    public string WindowTitle => _isEditMode ? "Edit Task" : "Add New Task";

    private string _title = string.Empty;
    public string Title { get => _title; set { if (SetProperty(ref _title, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

    private string _description = string.Empty;
    public string Description { get => _description; set { if (SetProperty(ref _description, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

    private TaskStatus _selectedStatus;
    public TaskStatus SelectedStatus { get => _selectedStatus; set { if (SetProperty(ref _selectedStatus, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }
    public IEnumerable<TaskStatus> AllTaskStatuses => Enum.GetValues(typeof(TaskStatus)).Cast<TaskStatus>();

    private User? _selectedAssignee;
    public User? SelectedAssignee
    {
        get => _selectedAssignee;
        set { if (SetProperty(ref _selectedAssignee, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    }
    public ObservableCollection<User> AssignableUsers { get; } = new();

    private DateTime? _dueDate;
    public DateTime? DueDate { get => _dueDate; set { if (SetProperty(ref _dueDate, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

    private bool _isActive;
    public bool IsActive { get => _isActive; set { if (SetProperty(ref _isActive, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }

    private string? _errorMessage;
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    public ICommand SaveCommand { get; }
    public Action<bool>? CloseActionWithResult { get; set; }

    public AddEditTaskViewModel() // Constructor cho Design-Time
    {
        _taskService = null;
        _userService = null;
        Title = "Design Task Title";
        Description = "Design task description.";
        SelectedStatus = TaskStatus.ToDo;
        AssignableUsers = new ObservableCollection<User> { new User(Guid.NewGuid(), "designer", "designer@example.com", UserRole.Staff, "Design Assignee") };
        SelectedAssignee = AssignableUsers.FirstOrDefault();
        SaveCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
    }

    public AddEditTaskViewModel(ITaskService taskService, IUserService userService)
    {
        _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        SaveCommand = new RelayCommand(async (_) => await SaveAsync(), CanSave);
        _ = LoadAssignableUsersAsync();
    }

    private async Task LoadAssignableUsersAsync()
    {
        if (_userService == null) return;
        try
        {
            // Gọi GetUsersAsync. Giả sử chúng ta muốn tất cả user active, không phân trang cho dropdown này.
            // API GetUsersAsync có thể cần được điều chỉnh để hỗ trợ việc lấy tất cả user
            // hoặc bạn có thể gọi với limit rất lớn.
            // Hiện tại, giả sử gọi với skip=0 và một limit đủ lớn hoặc API mặc định trả về tất cả nếu limit không được chỉ định.
            var paginatedResult = await _userService.GetUsersAsync(skip: 0, limit: 1000, includeInactive: false);

            AssignableUsers.Clear();
            if (paginatedResult?.PaginatedData != null) // ***** SỬA Ở ĐÂY *****
            {
                // ***** ÁP DỤNG OrderBy CHO paginatedResult.PaginatedData *****
                foreach (var user in paginatedResult.PaginatedData.OrderBy(u => u.FullName))
                {
                    AssignableUsers.Add(user);
                }
            }
            // ***** KẾT THÚC SỬA *****

            if (_isEditMode && _editingTaskOriginal.AssigneeId.HasValue)
            {
                SelectedAssignee = AssignableUsers.FirstOrDefault(u => u.Id == _editingTaskOriginal.AssigneeId.Value);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading assignable users in AddEditTaskViewModel: {ex.Message}");
            // Cân nhắc hiển thị lỗi cho người dùng nếu cần
        }
    }

    public void InitializeForAdd()
    {
        _isEditMode = false;
        _editingTaskOriginal = new TaskItem(Guid.NewGuid(), "", "", TaskStatus.ToDo, true) { CreatedDate = DateTime.UtcNow };
        PopulateFieldsFromTask(_editingTaskOriginal);
        OnPropertyChanged(nameof(WindowTitle));
        (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    public void InitializeForEdit(TaskItem taskToEdit)
    {
        _isEditMode = true;
        _editingTaskOriginal = new TaskItem(
            taskToEdit.Id, taskToEdit.Title, taskToEdit.Description, taskToEdit.Status, taskToEdit.IsActive)
        {
            AssigneeId = taskToEdit.AssigneeId,
            AssigneeName = taskToEdit.AssigneeName,
            AssigneeUsername = taskToEdit.AssigneeUsername,
            CreatedDate = taskToEdit.CreatedDate,
            DueDate = taskToEdit.DueDate
        };
        PopulateFieldsFromTask(_editingTaskOriginal);
        OnPropertyChanged(nameof(WindowTitle));
        (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private void PopulateFieldsFromTask(TaskItem task)
    {
        Title = task.Title;
        Description = task.Description;
        SelectedStatus = task.Status;
        DueDate = task.DueDate;
        IsActive = task.IsActive;
        SelectedAssignee = task.AssigneeId.HasValue ? AssignableUsers.FirstOrDefault(u => u.Id == task.AssigneeId.Value) : null;
        ErrorMessage = null;
    }

    private bool CanSave(object? arg)
    {
        if (IsSaving) return false;
        if (string.IsNullOrWhiteSpace(Title)) return false;

        if (_isEditMode)
        {
            return Title != _editingTaskOriginal.Title ||
                   Description != _editingTaskOriginal.Description ||
                   SelectedStatus != _editingTaskOriginal.Status ||
                   SelectedAssignee?.Id != _editingTaskOriginal.AssigneeId ||
                   DueDate != _editingTaskOriginal.DueDate ||
                   IsActive != _editingTaskOriginal.IsActive;
        }
        return true;
    }

    private async Task SaveAsync()
    {
        if (_taskService == null) { ErrorMessage = "Task service not available."; return; }
        ErrorMessage = null;
        if (!CanSave(null))
        {
            ErrorMessage = "Title is required, or no changes were made to save.";
            return;
        }
        IsSaving = true;

        TaskItem taskToSave = new TaskItem
        {
            Id = _isEditMode ? _editingTaskOriginal.Id : Guid.NewGuid(),
            Title = this.Title,
            Description = this.Description,
            Status = this.SelectedStatus,
            IsActive = this.IsActive,
            AssigneeId = SelectedAssignee?.Id,
            DueDate = DueDate,
            CreatedDate = _isEditMode ? _editingTaskOriginal.CreatedDate : DateTime.UtcNow
        };

        TaskItem? result = null;
        bool success = false;
        try
        {
            if (_isEditMode)
            {
                success = await _taskService.UpdateTaskAsync(taskToSave);
            }
            else
            {
                result = await _taskService.AddTaskAsync(taskToSave);
                success = result != null;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "An unexpected error occurred while saving task.";
        }
        finally
        {
            IsSaving = false;
        }

        CloseActionWithResult?.Invoke(success);
        if (!success && string.IsNullOrEmpty(ErrorMessage))
        {
            ErrorMessage = _isEditMode ? "Failed to update task." : "Failed to add task.";
        }
    }
}