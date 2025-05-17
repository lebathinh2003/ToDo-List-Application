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
    private readonly ToastNotificationViewModel? _toastViewModel;
    private TaskItem _editingTaskOriginal = new TaskItem(Guid.NewGuid(), "", "");
    private bool _isEditMode;
    private bool _isSaving;
    public bool IsSaving { get => _isSaving; set { if (SetProperty(ref _isSaving, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }
    public string WindowTitle => _isEditMode ? "Edit Task" : "Add New Task";
    private string _title = string.Empty;
    public string Title { get => _title; set { if (SetProperty(ref _title, value)) ValidateProperty(nameof(Title)); } }
    private string _description = string.Empty;
    public string Description { get => _description; set => SetProperty(ref _description, value); }
    private TaskStatus _selectedStatus;
    public TaskStatus SelectedStatus { get => _selectedStatus; set => SetProperty(ref _selectedStatus, value); }
    public IEnumerable<TaskStatus> AllTaskStatuses => Enum.GetValues(typeof(TaskStatus)).Cast<TaskStatus>();
    private User? _selectedAssignee;
    public User? SelectedAssignee { get => _selectedAssignee; set { if (SetProperty(ref _selectedAssignee, value)) (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); } }
    public ObservableCollection<User> AssignableUsers { get; } = new();
    private DateTime? _dueDate;
    public DateTime? DueDate { get => _dueDate; set => SetProperty(ref _dueDate, value); }
    private bool _isActive = true;
    public bool IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public Action<bool>? CloseActionWithResult { get; set; }

    public AddEditTaskViewModel() : this(null, null, null) { }
    public AddEditTaskViewModel(ITaskService? taskService, IUserService? userService, ToastNotificationViewModel? toastViewModel)
    {
        _taskService = taskService; _userService = userService; _toastViewModel = toastViewModel;
        SaveCommand = new RelayCommand(async (_) => await SaveAsync(), CanSave);
        CancelCommand = new RelayCommand(_ => CloseActionWithResult?.Invoke(false));
        if (IsInDesignModeStatic()) { Title = "Design Task"; AssignableUsers.Add(new User(Guid.NewGuid(), "u", "e", UserRole.Staff, "DU")); SelectedAssignee = AssignableUsers.FirstOrDefault(); SelectedStatus = TaskStatus.ToDo; DueDate = DateTime.Now.AddDays(7); }
        else { _ = LoadAssignableUsersAsync(); }
    }
    private static bool IsInDesignModeStatic() { return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime; }
    public async Task LoadAssignableUsersAsync()
    {
        if (_userService == null) return;
        try
        {
            var paginatedResult = await _userService.GetUsersAsync(skip: 0, limit: 1000, includeInactive: false);
            AssignableUsers.Clear();
            if (paginatedResult?.PaginatedData != null)
            { foreach (var user in paginatedResult.PaginatedData.OrderBy(u => u.FullName)) AssignableUsers.Add(user); }
            if (_isEditMode && _editingTaskOriginal.AssigneeId.HasValue) { SelectedAssignee = AssignableUsers.FirstOrDefault(u => u.Id == _editingTaskOriginal.AssigneeId.Value); }
            ValidateProperty(nameof(SelectedAssignee));
        }
        catch (Exception ex) { Debug.WriteLine($"Error loading assignable users: {ex.Message}"); _toastViewModel?.Show("Error loading users.", ToastType.Error); }
    }
    public void InitializeForAdd() { _isEditMode = false; _editingTaskOriginal = new TaskItem(Guid.NewGuid(), "", "", TaskStatus.ToDo, true) { CreatedDate = DateTime.UtcNow }; PopulateFieldsFromTask(_editingTaskOriginal); OnPropertyChanged(nameof(WindowTitle)); ClearAllErrors(); (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    public void InitializeForEdit(TaskItem taskToEdit) { _isEditMode = true; _editingTaskOriginal = new TaskItem(taskToEdit.Id, taskToEdit.Title, taskToEdit.Description, taskToEdit.Status, taskToEdit.IsActive) { AssigneeId = taskToEdit.AssigneeId, AssigneeName = taskToEdit.AssigneeName, AssigneeUsername = taskToEdit.AssigneeUsername, CreatedDate = taskToEdit.CreatedDate, DueDate = taskToEdit.DueDate }; PopulateFieldsFromTask(_editingTaskOriginal); OnPropertyChanged(nameof(WindowTitle)); ClearAllErrors(); (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    private void PopulateFieldsFromTask(TaskItem task) { Title = task.Title; Description = task.Description; SelectedStatus = task.Status; DueDate = task.DueDate; IsActive = task.IsActive; }
    protected override void ValidateProperty(string? propertyName) { base.ValidateProperty(propertyName); ClearErrors(propertyName); switch (propertyName) { case nameof(Title): if (string.IsNullOrWhiteSpace(Title)) AddError(nameof(Title), "Title is required."); break; case nameof(SelectedAssignee): if (SelectedAssignee == null) AddError(nameof(SelectedAssignee), "Assignee is required."); break; } (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    protected override void ValidateAllProperties() { ValidateProperty(nameof(Title)); ValidateProperty(nameof(SelectedAssignee)); }
    private bool CanSave(object? parameter) { if (IsSaving) return false; ValidateAllProperties(); return !HasErrors; }
    private async Task SaveAsync()
    {
        ValidateAllProperties(); if (HasErrors) { _toastViewModel?.Show("Please correct validation errors.", ToastType.Warning); return; }
        if (_taskService == null || _toastViewModel == null) { _toastViewModel?.Show("Task service unavailable.", ToastType.Error); return; }
        IsSaving = true;
        TaskItem taskToSave = new TaskItem { Id = _isEditMode ? _editingTaskOriginal.Id : Guid.NewGuid(), Title = this.Title, Description = this.Description, Status = this.SelectedStatus, IsActive = this.IsActive, AssigneeId = SelectedAssignee!.Id, DueDate = DueDate, CreatedDate = _isEditMode ? _editingTaskOriginal.CreatedDate : DateTime.UtcNow };
        bool success = false; string successMsg = "", failureMsg = "";
        try
        {
            if (_isEditMode) { success = await _taskService.UpdateTaskAsync(taskToSave); successMsg = $"Task '{taskToSave.Title}' updated."; failureMsg = $"Failed to update task '{taskToSave.Title}'."; }
            else { TaskItem? addedTask = await _taskService.AddTaskAsync(taskToSave); success = addedTask != null; successMsg = $"Task '{taskToSave.Title}' added."; failureMsg = $"Failed to add task '{taskToSave.Title}'."; }
        }
        catch (Exception ex) { failureMsg = "Error saving task."; Debug.WriteLine($"SaveTaskAsync Error: {ex}"); success = false; }
        finally { IsSaving = false; }
        if (success) { _toastViewModel.Show(successMsg, ToastType.Success); CloseActionWithResult?.Invoke(true); }
        else { _toastViewModel.Show(failureMsg, ToastType.Error); }
    }
    private void ClearAllErrors() { ClearErrors(nameof(Title)); ClearErrors(nameof(SelectedAssignee)); }
}