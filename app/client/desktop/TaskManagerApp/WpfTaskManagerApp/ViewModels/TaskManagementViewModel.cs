using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.Services;
using WpfTaskManagerApp.ViewModels.Common;
using WpfTaskManagerApp.Views;
using TaskStatus = WpfTaskManagerApp.Core.TaskStatus;

namespace WpfTaskManagerApp.ViewModels;
public class TaskManagementViewModel : ViewModelBase, IDisposable
{
    private readonly ITaskService? _taskService;
    private readonly IUserService? _userService;
    private readonly IAuthenticationService? _authenticationService;
    private readonly IServiceProvider? _serviceProvider;
    private readonly ISignalRService? _signalRService;
    private readonly ToastNotificationViewModel? _toastViewModel;
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (SetProperty(ref _isLoading, value))
            {
                (AddTaskCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (EditTaskCommand as RelayCommand<TaskItem>)?.RaiseCanExecuteChanged();
                (RestoreTaskCommand as RelayCommand<TaskItem>)?.RaiseCanExecuteChanged();
                (DeleteTaskCommand as RelayCommand<TaskItem>)?.RaiseCanExecuteChanged();
                (SearchCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (RefreshCommand as RelayCommand)?.RaiseCanExecuteChanged();
                UpdatePaginationCommandsCanExecute();
            }
        }
    }

    private ObservableCollection<TaskItem> _tasks = new ObservableCollection<TaskItem>();
    public ObservableCollection<TaskItem> Tasks
    {
        get => _tasks;
        set => SetProperty(ref _tasks, value);
    }

    private TaskItem? _selectedTask;
    public TaskItem? SelectedTask
    {
        get => _selectedTask;
        set
        {
            if (SetProperty(ref _selectedTask, value))
            {
                (EditTaskCommand as RelayCommand<TaskItem>)?.RaiseCanExecuteChanged();
                (DeleteTaskCommand as RelayCommand<TaskItem>)?.RaiseCanExecuteChanged();
                (RestoreTaskCommand as RelayCommand<TaskItem>)?.RaiseCanExecuteChanged();
            }
        }
    }

    private string _searchTerm = string.Empty;
    public string SearchTerm
    {
        get => _searchTerm;
        set => SetProperty(ref _searchTerm, value);
    }

    private TaskStatusItem? _selectedFilterStatus;
    public TaskStatusItem? SelectedFilterStatus
    {
        get => _selectedFilterStatus;
        set
        {
            if (_selectedFilterStatus != value)
            {
                _selectedFilterStatus = value;
                OnPropertyChanged(nameof(SelectedFilterStatus));
                CurrentPage = 1;
                _ = LoadTasksAsync();
            }
        }
    }
    private List<TaskStatusItem>? _allTaskStatusItems;
    public IEnumerable<TaskStatusItem> AllTaskStatusesWithOptionalNone
    {
        get
        {
            if (_allTaskStatusItems == null)
            {
                _allTaskStatusItems = new List<TaskStatusItem>();
                var addStatus = new TaskStatusItem(null, "All");
                _allTaskStatusItems.Add(addStatus);
                _selectedFilterStatus = addStatus;
                foreach (TaskStatus status in Enum.GetValues(typeof(TaskStatus)))
                {
                    var displayName = status
                    switch
                    {
                        TaskStatus.ToDo => "To Do",
                        TaskStatus.InProgress => "In Progress",
                        TaskStatus.Done => "Done",
                        TaskStatus.Cancelled => "Cancelled",
                        _ => status.ToString(),
                    };
                    _allTaskStatusItems.Add(new TaskStatusItem(status, displayName));
                }
            }
            return _allTaskStatusItems;
        }
    }

    private string? _sortBy;
    public string? SortBy
    {
        get => _sortBy;
        set
        {
            if (SetProperty(ref _sortBy, value))
            {
                CurrentPage = 1;
                _ = LoadTasksAsync();
            }
        }
    }
    private string _sortOrder = "asc";
    public string SortOrder
    {
        get => _sortOrder;
        set
        {
            if (SetProperty(ref _sortOrder, value))
            {
                CurrentPage = 1;
                _ = LoadTasksAsync();
            }
        }
    }
    public ObservableCollection<string> SortableTaskProperties
    {
        get;
    }
    public ObservableCollection<string> SortOrders
    {
        get;
    }

    public bool CanAdminManageTasks => _authenticationService?.CurrentUser?.Role == UserRole.Admin;

    private int _currentPage = 1;
    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (value < 1) value = 1;
            if (SetProperty(ref _currentPage, value))
            {
                OnPropertyChanged(nameof(CurrentPageDisplay));
                UpdatePaginationCommandsCanExecute();
            }
        }
    }
    public string CurrentPageDisplay => $"{CurrentPage}";
    private int _limit = 10;
    public int Limit
    {
        get => _limit;
        set
        {
            if (value < 1) value = 1;
            if (SetProperty(ref _limit, value))
            {
                CurrentPage = 1;
                _ = LoadTasksAsync();
            }
        }
    }
    private int _totalItems;
    public int TotalItems
    {
        get => _totalItems;
        private set
        {
            if (SetProperty(ref _totalItems, value))
            {
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(TotalPagesDisplay));
                UpdatePaginationCommandsCanExecute();
            }
        }
    }
    public int TotalPages => (TotalItems == 0 || Limit <= 0) ? 1 : (int)Math.Ceiling((double)TotalItems / Limit);
    public string TotalPagesDisplay => $"{TotalPages}";
    public bool CanGoToPreviousPage => CurrentPage > 1 && !IsLoading;
    public bool CanGoToNextPage => CurrentPage < TotalPages && !IsLoading;

    public ICommand AddTaskCommand
    {
        get;
    }
    public ICommand EditTaskCommand
    {
        get;
    }
    public ICommand DeleteTaskCommand
    {
        get;
    }
    public ICommand RestoreTaskCommand
    {
        get;
    }
    public ICommand SearchCommand
    {
        get;
    }
    public ICommand RefreshCommand
    {
        get;
    }
    public ICommand FirstPageCommand
    {
        get;
    }
    public ICommand PreviousPageCommand
    {
        get;
    }
    public ICommand NextPageCommand
    {
        get;
    }
    public ICommand LastPageCommand
    {
        get;
    }

    public TaskManagementViewModel() // Constructor cho Design-Time
    {
        _taskService = null;
        _userService = null;
        _authenticationService = null;
        _serviceProvider = null;
        _signalRService = null;
        _toastViewModel = new ToastNotificationViewModel();
        SortableTaskProperties = new ObservableCollection<string> {
            "Title",
            "DueDate",
            "AssigneeName",
            "Status",
            "CreatedAt"
        };
        SortOrders = new ObservableCollection<string> {
            "asc",
            "desc"
        };
        SortBy = "CreatedAt";
        Tasks = new ObservableCollection<TaskItem> {
            new TaskItem(Guid.NewGuid(), "Design UI for Login", "Complete the UI design for the login page", TaskStatus.InProgress) {
                AssigneeName = "Designer A", DueDate = DateTime.Now.AddDays(2)
            },
            new TaskItem(Guid.NewGuid(), "Develop API for Tasks", "Implement CRUD operations for tasks", TaskStatus.ToDo) {
                AssigneeName = "Developer B", DueDate = DateTime.Now.AddDays(5), IsActive = true
            },
            new TaskItem(Guid.NewGuid(), "Test Payment Gateway", "Perform thorough testing of the payment gateway", TaskStatus.Done) {
                AssigneeName = "QA C", DueDate = DateTime.Now.AddDays(-1), IsActive = false
            }
        };
        TotalItems = Tasks.Count;
        CurrentPage = 1;
        Limit = 10;
        AddTaskCommand = new RelayCommand(_ => { }, _ => false);
        EditTaskCommand = new RelayCommand<TaskItem>(_ => { }, _ => false);
        DeleteTaskCommand = new RelayCommand<TaskItem>(_ => { }, _ => false);
        RestoreTaskCommand = new RelayCommand<TaskItem>(_ => { }, _ => false);
        SearchCommand = new RelayCommand(async _ => {
            CurrentPage = 1;
            await LoadTasksAsync();
        }, _ => !IsLoading); // Cho phép search ở design-time
        RefreshCommand = new RelayCommand(async _ => await LoadTasksAsync(), _ => !IsLoading); // Cho phép refresh ở design-time
        FirstPageCommand = new RelayCommand(_ => { }, _ => false);
        PreviousPageCommand = new RelayCommand(_ => { }, _ => false);
        NextPageCommand = new RelayCommand(_ => { }, _ => false);
        LastPageCommand = new RelayCommand(_ => { }, _ => false);
        OnPropertyChanged(nameof(CanAdminManageTasks));
        UpdatePaginationCommandsCanExecute();
    }

    public TaskManagementViewModel(ITaskService taskService, IUserService userService,
        IAuthenticationService authenticationService, IServiceProvider serviceProvider,
        ISignalRService signalRService, ToastNotificationViewModel toastViewModel)
    {
        _taskService = taskService ??
            throw new ArgumentNullException(nameof(taskService));
        _userService = userService;
        _authenticationService = authenticationService ??
            throw new ArgumentNullException(nameof(authenticationService));
        _serviceProvider = serviceProvider ??
            throw new ArgumentNullException(nameof(serviceProvider));
        _signalRService = signalRService ??
            throw new ArgumentNullException(nameof(signalRService));
        _toastViewModel = toastViewModel ??
            throw new ArgumentNullException(nameof(toastViewModel));

        SortableTaskProperties = new ObservableCollection<string> {
            "Title",
            "DueDate",
            "AssigneeName",
            "Status",
            "CreatedAt"
        };
        SortOrders = new ObservableCollection<string> {
            "asc",
            "desc"
        };
        _sortBy = "CreatedAt";
        _sortOrder = "desc";

        AddTaskCommand = new RelayCommand(async _ => await OpenAddEditTaskDialog(null), _ => CanAdminManageTasks && !IsLoading);
        EditTaskCommand = new RelayCommand<TaskItem>(async (task) => await OpenAddEditTaskDialog(task),
            (task) =>
            {
                if (task == null || IsLoading) return false;
                var currentUser = _authenticationService?.CurrentUser;
                if (currentUser == null) return false;
                // Admin có thể sửa mọi task
                if (currentUser.Role == UserRole.Admin) return true;
                // Staff chỉ có thể sửa task được gán cho họ
                if (currentUser.Role == UserRole.Staff && task.AssigneeId == currentUser.Id) return true;
                return false;
            });
        DeleteTaskCommand = new RelayCommand<TaskItem>(async (task) => await DeleteTaskAsync(task),
            (task) => task != null && task.IsActive && CanAdminManageTasks && !IsLoading);
        RestoreTaskCommand = new RelayCommand<TaskItem>(async (task) => await RestoreTaskAsync(task), (task) => task != null && !task.IsActive && !IsLoading);

        SearchCommand = new RelayCommand(async _ => {
            CurrentPage = 1;
            await LoadTasksAsync();
        }, _ => !IsLoading);
        RefreshCommand = new RelayCommand(async _ => await LoadTasksAsync(), _ => !IsLoading);
        FirstPageCommand = new RelayCommand(async _ => {
            if (CurrentPage != 1)
            {
                CurrentPage = 1;
                await LoadTasksAsync();
            }
        }, _ => CanGoToPreviousPage);
        PreviousPageCommand = new RelayCommand(async _ => {
            if (CanGoToPreviousPage)
            {
                CurrentPage--;
                await LoadTasksAsync();
            }
        }, _ => CanGoToPreviousPage);
        NextPageCommand = new RelayCommand(async _ => {
            if (CanGoToNextPage)
            {
                CurrentPage++;
                await LoadTasksAsync();
            }
        }, _ => CanGoToNextPage);
        LastPageCommand = new RelayCommand(async _ => {
            if (CurrentPage != TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
                await LoadTasksAsync();
            }
        }, _ => CanGoToNextPage && CurrentPage != TotalPages);

        _signalRService.NewTaskAssigned += OnSignalRNewTaskAssigned;
        _signalRService.TaskStatusUpdated += OnSignalRTaskStatusUpdated;

        _ = LoadTasksAsync();
        OnPropertyChanged(nameof(CanAdminManageTasks));
    }

    private void UpdatePaginationCommandsCanExecute()
    {
        OnPropertyChanged(nameof(CanGoToPreviousPage));
        OnPropertyChanged(nameof(CanGoToNextPage));
        (FirstPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (LastPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    public async Task LoadTasksAsync()
    {
        if (_taskService == null || _authenticationService == null)
        {
            _toastViewModel?.Show("Task or authentication service is not available.", ToastType.Error);
            return;
        }
        if (IsLoading) return;
        IsLoading = true;

        try
        {
            if (Limit <= 0) _limit = 10;
            int apiSkipParameter = CurrentPage - 1;
            PaginatedResult<TaskItem>? paginatedResult = null;
            var currentUser = _authenticationService.CurrentUser;

            if (currentUser == null)
            {
                Tasks.Clear();
                TotalItems = 0;
                CurrentPage = 1;
                _toastViewModel?.Show("No user logged in. Cannot load tasks.", ToastType.Warning);
                return;
            }

            if (currentUser.Role == UserRole.Admin)
            {
                paginatedResult = await _taskService.GetAllTasksAsync(apiSkipParameter, Limit, SortBy, SortOrder, SearchTerm, SelectedFilterStatus, includeInactive: true);
            }
            else // Staff
            {
                paginatedResult = await _taskService.GetTasksByAssigneeAsync(currentUser.Id, apiSkipParameter, Limit, SortBy, SortOrder, SearchTerm, SelectedFilterStatus);
            }

            if (paginatedResult?.PaginatedData != null)
            {
                Tasks = new ObservableCollection<TaskItem>(paginatedResult.PaginatedData);
                if (paginatedResult.Metadata != null)
                {
                    TotalItems = paginatedResult.Metadata.TotalRow;
                    if (CurrentPage > TotalPages && TotalPages > 0) CurrentPage = TotalPages;
                    else if (TotalPages == 0 && TotalItems == 0) CurrentPage = 1;
                }
                else
                {
                    TotalItems = Tasks.Count;
                }
            }
            else
            {
                Tasks.Clear();
                TotalItems = 0;
                CurrentPage = 1;
                _toastViewModel?.Show("No tasks found or error loading tasks.", ToastType.Information);
            }
        }
        catch (Exception ex)
        {
            _toastViewModel?.Show($"Error loading tasks: {ex.Message}", ToastType.Error);
            Tasks.Clear();
            TotalItems = 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task OpenAddEditTaskDialog(TaskItem? taskToEdit)
    {
        if (_serviceProvider == null || _toastViewModel == null)
        {
            _toastViewModel?.Show("Cannot open dialog: core services missing.", ToastType.Error);
            return;
        }
        var addEditTaskViewModel = _serviceProvider.GetRequiredService<AddEditTaskViewModel>();

        // Gọi initialize trước, để biết đang ở chế độ thêm hay sửa
        if (taskToEdit == null)
        {
            addEditTaskViewModel.InitializeForAdd();
        }
        else
        {
            addEditTaskViewModel.InitializeForEdit(taskToEdit);
        }

        // Sau đó mới load danh sách người dùng có thể giao
        await addEditTaskViewModel.LoadAssignableUsersAsync();

        var dialogView = new AddEditTaskDialog
        {
            DataContext = addEditTaskViewModel
        };
        var dialogWindow = new Window
        {
            Title = addEditTaskViewModel.WindowTitle,
            Content = dialogView,
            Width = 520,
            Height = 750,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Application.Current.MainWindow,
            ResizeMode = ResizeMode.NoResize,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.ToolWindow
        };
        addEditTaskViewModel.CloseActionWithResult = (success) => {
            dialogWindow.DialogResult = success;
            dialogWindow.Close();
            if (success)
            {
                _ = LoadTasksAsync();
            }
        };
        dialogWindow.ShowDialog();
    }

    private async Task DeleteTaskAsync(TaskItem? taskToDelete)
    {
        if (taskToDelete == null || _taskService == null || _toastViewModel == null || !CanAdminManageTasks) return;

        // TODO: Implement confirmation dialog
        IsLoading = true;
        bool success = false;
        try
        {
            success = await _taskService.DeleteTaskAsync(taskToDelete.Id);
            if (success)
            {
                _toastViewModel.Show($"Task '{taskToDelete.Title}' marked as inactive.", ToastType.Success);
                // Cập nhật lạc quan
                taskToDelete.IsActive = false;
                (DeleteTaskCommand as RelayCommand<TaskItem>)?.RaiseCanExecuteChanged();
                (RestoreTaskCommand as RelayCommand<TaskItem>)?.RaiseCanExecuteChanged();
                // Có thể không cần LoadStaffAsync() ngay nếu UI đã cập nhật,
                // nhưng để đảm bảo đồng bộ thì nên gọi, hoặc gọi khi refresh.
                // await LoadStaffAsync(); 
            }
            else { _toastViewModel.Show($"Failed to mark task '{taskToDelete.Title}' as inactive.", ToastType.Error); }
        }
        catch (Exception ex) {
            _toastViewModel.Show($"Error deleting task: {ex.Message}", ToastType.Error);
        }
        finally { IsLoading = false; }
    }

    private async Task RestoreTaskAsync(TaskItem? taskToRestore)
    {
        if (taskToRestore == null || _taskService == null || _toastViewModel == null || !CanAdminManageTasks) return;
        IsLoading = true;
        bool success = false;
        try
        {
            success = await _taskService.RestoreTaskAsync(taskToRestore.Id);
            if (success)
            {
                _toastViewModel.Show($"Task '{taskToRestore.Title}' restored successfully.", ToastType.Success);
                taskToRestore.IsActive = true;
                (DeleteTaskCommand as RelayCommand<TaskItem>)?.RaiseCanExecuteChanged();
                (RestoreTaskCommand as RelayCommand<TaskItem>)?.RaiseCanExecuteChanged();
                // await LoadStaffAsync(); // Tương tự như Delete
            }
            else { _toastViewModel.Show($"Failed to restore task '{taskToRestore.Title}'.", ToastType.Error); }
        }
        catch (Exception ex) { 
            _toastViewModel.Show($"Error restoring task: {ex.Message}", ToastType.Error);
        }
        finally { IsLoading = false; }
    }

    private void OnSignalRNewTaskAssigned(TaskItem newTask)
    {
        var currentUser = _authenticationService?.CurrentUser;
        if (currentUser == null) return;
        bool shouldAddTask = currentUser.Role == UserRole.Admin || (currentUser.Role == UserRole.Staff && newTask.AssigneeId == currentUser.Id);
        if (shouldAddTask)
        {
            var existingTask = Tasks.FirstOrDefault(t => t.Id == newTask.Id);
            if (existingTask == null)
            {
                Application.Current.Dispatcher.Invoke(() => {
                    Tasks.Insert(0, newTask);
                    TotalItems++;
                });
                _toastViewModel?.Show($"New task assigned to you: {newTask.Title}", ToastType.Information);
            }
        }
    }
    private void OnSignalRTaskStatusUpdated(TaskItem updatedTask)
    {
        var taskInList = Tasks.FirstOrDefault(t => t.Id == updatedTask.Id);
        if (taskInList != null)
        {
            Application.Current.Dispatcher.Invoke(() => {
                taskInList.Title = updatedTask.Title;
                taskInList.Description = updatedTask.Description;
                taskInList.Status = updatedTask.Status;
                taskInList.AssigneeId = updatedTask.AssigneeId;
                taskInList.AssigneeName = updatedTask.AssigneeName;
                taskInList.AssigneeUsername = updatedTask.AssigneeUsername;
                taskInList.DueDate = updatedTask.DueDate;
                taskInList.IsActive = updatedTask.IsActive;
            });
            _toastViewModel?.Show($"Task '{updatedTask.Title}' updated to {updatedTask.Status}.", ToastType.Information);
        }
        else // Nếu task không có trong danh sách hiện tại (ví dụ: admin xem tất cả, staff chỉ xem của mình)
        {
            var currentUser = _authenticationService?.CurrentUser;
            if (currentUser != null && (currentUser.Role == UserRole.Admin || (currentUser.Role == UserRole.Staff && updatedTask.AssigneeId == currentUser.Id)))
            {
                _toastViewModel?.Show($"Task '{updatedTask.Title}' (possibly assigned to you) was updated to {updatedTask.Status}.", ToastType.Information);
            }
        }
    }

    public void UpdateTaskInList(TaskItem updatedTask)
    {
        var taskInList = Tasks.FirstOrDefault(t => t.Id == updatedTask.Id);
        if (taskInList != null)
        {
            // Cập nhật các thuộc tính của taskInList từ updatedTask
            // Điều này sẽ tự động cập nhật UI nếu TaskItem implement INotifyPropertyChanged
            taskInList.Title = updatedTask.Title;
            taskInList.Description = updatedTask.Description;
            taskInList.Status = updatedTask.Status;
            taskInList.AssigneeId = updatedTask.AssigneeId;
            taskInList.AssigneeName = updatedTask.AssigneeName;
            taskInList.DueDate = updatedTask.DueDate;
            taskInList.IsActive = updatedTask.IsActive;
        }
    }

    public override void Dispose()
    {
        if (_signalRService != null)
        {
            _signalRService.NewTaskAssigned -= OnSignalRNewTaskAssigned;
            _signalRService.TaskStatusUpdated -= OnSignalRTaskStatusUpdated;
        }
        base.Dispose();
    }
}