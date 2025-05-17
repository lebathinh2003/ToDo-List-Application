using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.Services;
using WpfTaskManagerApp.ViewModels.Common;
using TaskStatus = WpfTaskManagerApp.Core.TaskStatus;

namespace WpfTaskManagerApp.ViewModels;
// --- AddEditTaskViewModel ---
public class AddEditTaskViewModel : ViewModelBase
{
    private readonly ITaskService? _taskService; 
    private readonly IUserService? _userService; 
    private TaskItem _editingTaskOriginal = new TaskItem(Guid.NewGuid(), "", ""); 
    private bool _isEditMode;
    private bool _isSaving;
    public bool IsSaving { get => _isSaving; set { if(SetProperty(ref _isSaving, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }
    public string WindowTitle => _isEditMode ? "Edit Task" : "Add New Task";
    private string _title = string.Empty;
    public string Title { get => _title; set { if(SetProperty(ref _title, value)) ValidateProperty(nameof(Title)); } }
    private string _description = string.Empty;
    public string Description { get => _description; set => SetProperty(ref _description, value); }
    private TaskStatus _selectedStatus;
    public TaskStatus SelectedStatus { get => _selectedStatus; set => SetProperty(ref _selectedStatus, value); }
    public IEnumerable<TaskStatus> AllTaskStatuses => Enum.GetValues(typeof(TaskStatus)).Cast<TaskStatus>();
    private User? _selectedAssignee; 
    public User? SelectedAssignee { get => _selectedAssignee; set => SetProperty(ref _selectedAssignee, value); }
    public ObservableCollection<User> AssignableUsers { get; } = new(); 
    private DateTime? _dueDate;
    public DateTime? DueDate { get => _dueDate; set => SetProperty(ref _dueDate, value); }
    private bool _isActive; 
    public bool IsActive { get => _isActive; set => SetProperty(ref _isActive, value); } 
    private string? _errorMessage;
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public ICommand SaveCommand { get; }
    public Action<bool>? CloseActionWithResult { get; set; } 

    public AddEditTaskViewModel() : this(null, null) { }
    public AddEditTaskViewModel(ITaskService? taskService, IUserService? userService)
    {
        _taskService = taskService; _userService = userService;
        SaveCommand = new RelayCommand(async (_) => await SaveAsync(), CanSave);
        if (IsInDesignModeStatic()) { Title = "Design Task"; AssignableUsers.Add(new User(Guid.NewGuid(), "u", "e", UserRole.Staff, "DU")); SelectedAssignee = AssignableUsers.FirstOrDefault(); }
        else { _ = LoadAssignableUsersAsync(); }
    }
    private static bool IsInDesignModeStatic() { return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime; }
    private async Task LoadAssignableUsersAsync() 
    {
        if (_userService == null) return; 
        try
        {
            var paginatedResult = await _userService.GetUsersAsync(skip: 0, limit: 1000, includeInactive: false); 
            AssignableUsers.Clear();
            if (paginatedResult?.PaginatedData != null) 
            { foreach (var user in paginatedResult.PaginatedData.OrderBy(u => u.FullName)) AssignableUsers.Add(user); }
            if (_isEditMode && _editingTaskOriginal.AssigneeId.HasValue) { SelectedAssignee = AssignableUsers.FirstOrDefault(u => u.Id == _editingTaskOriginal.AssigneeId.Value); }
        } catch (Exception ex) { Debug.WriteLine($"Error loading assignable users in AddEditTaskViewModel: {ex.Message}"); }
    }
    public void InitializeForAdd() { _isEditMode = false; _editingTaskOriginal = new TaskItem(Guid.NewGuid(), "", "", TaskStatus.ToDo, true) { CreatedDate = DateTime.UtcNow }; PopulateFieldsFromTask(_editingTaskOriginal); OnPropertyChanged(nameof(WindowTitle)); ClearAllErrors(); (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    public void InitializeForEdit(TaskItem taskToEdit) { _isEditMode = true; _editingTaskOriginal = new TaskItem(taskToEdit.Id, taskToEdit.Title, taskToEdit.Description, taskToEdit.Status, taskToEdit.IsActive) { AssigneeId = taskToEdit.AssigneeId, AssigneeName = taskToEdit.AssigneeName, AssigneeUsername = taskToEdit.AssigneeUsername, CreatedDate = taskToEdit.CreatedDate, DueDate = taskToEdit.DueDate }; PopulateFieldsFromTask(_editingTaskOriginal); OnPropertyChanged(nameof(WindowTitle)); ClearAllErrors(); (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    private void PopulateFieldsFromTask(TaskItem task) { Title = task.Title; Description = task.Description; SelectedStatus = task.Status; DueDate = task.DueDate; IsActive = task.IsActive; SelectedAssignee = task.AssigneeId.HasValue ? AssignableUsers.FirstOrDefault(u => u.Id == task.AssigneeId.Value) : null; ErrorMessage = null; }
    protected override void ValidateProperty(string? propertyName) { base.ValidateProperty(propertyName); ClearErrors(propertyName); if (propertyName == nameof(Title) && string.IsNullOrWhiteSpace(Title)) AddError(nameof(Title), "Title is required."); (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    protected override void ValidateAllProperties() { ValidateProperty(nameof(Title)); }
    private bool CanSave(object? parameter) { if (IsSaving) return false; return !HasErrors; }
    private async Task SaveAsync()
    {
        ValidateAllProperties(); if (HasErrors) { /* Show Toast? */ return; }
        if (_taskService == null) { ErrorMessage = "Task service unavailable."; return; }
        IsSaving = true; ErrorMessage = null;
        TaskItem taskToSave = new TaskItem { Id = _isEditMode ? _editingTaskOriginal.Id : Guid.NewGuid(), Title = this.Title, Description = this.Description, Status = this.SelectedStatus, IsActive = this.IsActive, AssigneeId = SelectedAssignee?.Id, DueDate = DueDate, CreatedDate = _isEditMode ? _editingTaskOriginal.CreatedDate : DateTime.UtcNow };
        bool success = false;
        try
        {
            if (_isEditMode) { success = await _taskService.UpdateTaskAsync(taskToSave); }
            else { TaskItem? addedTask = await _taskService.AddTaskAsync(taskToSave); success = addedTask != null; }
        } catch (Exception ex) { ErrorMessage = "Error saving task."; Debug.WriteLine($"SaveTaskAsync Error: {ex}"); }
        finally { IsSaving = false; }
        CloseActionWithResult?.Invoke(success);
        if (!success && string.IsNullOrEmpty(ErrorMessage)) ErrorMessage = _isEditMode ? "Failed to update task." : "Failed to add task.";
    }
    private void ClearAllErrors() { ClearErrors(nameof(Title)); }
}
