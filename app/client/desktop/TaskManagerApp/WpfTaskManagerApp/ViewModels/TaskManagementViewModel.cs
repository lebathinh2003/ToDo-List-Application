using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.Services;
using WpfTaskManagerApp.ViewModels.Common;
using TaskStatus = WpfTaskManagerApp.Core.TaskStatus;

namespace WpfTaskManagerApp.ViewModels;
public class TaskManagementViewModel : ViewModelBase, IDisposable
{
    private readonly ITaskService? _taskService;
    private readonly IUserService? _userService;
    private readonly IAuthenticationService? _authenticationService;
    private readonly IServiceProvider? _serviceProvider;
    private readonly ISignalRService? _signalRService;
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (SetProperty(ref _isLoading, value))
            {
                (AddTaskCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (EditTaskCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteTaskCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (SearchCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (UpdateStatusCommand as RelayCommand<TaskStatus>)?.RaiseCanExecuteChanged();
                (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (RefreshCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }


    private ObservableCollection<TaskItem> _tasks = new(); 
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
                (EditTaskCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteTaskCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (UpdateStatusCommand as RelayCommand<TaskStatus>)?.RaiseCanExecuteChanged();
            }
        }
    }

    private string _searchTerm = string.Empty;
    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (SetProperty(ref _searchTerm, value))
            {
                OnSearchTermChanged();
            }
        }
    }

    private TaskStatus? _selectedFilterStatus;
    public TaskStatus? SelectedFilterStatus
    {
        get => _selectedFilterStatus;
        set
        {
            if (SetProperty(ref _selectedFilterStatus, value))
            {
                OnFilterStatusChanged();
            }
        }
    }
    public IEnumerable<TaskStatus?> AllTaskStatusesWithOptionalNone
    {
        get
        {
            yield return null;
            foreach (TaskStatus status in Enum.GetValues(typeof(TaskStatus)))
            {
                yield return status;
            }
        }
    }

    public bool CanAdminManageTasks => _authenticationService?.CurrentUser?.Role == UserRole.Admin;
    public bool CanStaffUpdateStatusDirectly => _authenticationService?.CurrentUser?.Role == UserRole.Staff;

    private int _currentPage = 1;
    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (SetProperty(ref _currentPage, value))
            {
                (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }
    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (SetProperty(ref _pageSize, value))
            {
                _currentPage = 1;
                OnPropertyChanged(nameof(CurrentPage));
                (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
                OnPageSizeChanged();
            }
        }
    }
    private int _totalItems;
    public int TotalItems
    {
        get => _totalItems;
        set
        {
            if (SetProperty(ref _totalItems, value))
            {
                OnPropertyChanged(nameof(TotalPages));
                (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }
    public int TotalPages => (TotalItems == 0 || PageSize <= 0) ? 0 : (int)Math.Ceiling((double)TotalItems / PageSize);

    public ICommand AddTaskCommand { get; }
    public ICommand EditTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand UpdateStatusCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand RefreshCommand { get; }

    public TaskManagementViewModel()
    {
        _taskService = null;
        _userService = null;
        _authenticationService = null;
        _serviceProvider = null;
        _signalRService = null;

        Tasks = new ObservableCollection<TaskItem>
            {
                new TaskItem(Guid.NewGuid(), "Design Task 1", "Description for design task 1", TaskStatus.ToDo) { AssigneeName = "Designer" },
                new TaskItem(Guid.NewGuid(), "Design Task 2", "Description for design task 2", TaskStatus.InProgress) { AssigneeName = "Designer" }
            };
        TotalItems = Tasks.Count;
        IsLoading = false;

        AddTaskCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        EditTaskCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        DeleteTaskCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        SearchCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        UpdateStatusCommand = new RelayCommand<TaskStatus>(async _ => await Task.CompletedTask, _ => false);
        NextPageCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        PreviousPageCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        RefreshCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
    }


    public TaskManagementViewModel(ITaskService taskService,
                                 IUserService userService,
                                 IAuthenticationService authenticationService,
                                 IServiceProvider serviceProvider,
                                 ISignalRService signalRService)
    {
        _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _signalRService = signalRService ?? throw new ArgumentNullException(nameof(signalRService));

        AddTaskCommand = new RelayCommand(async param => await OpenAddTaskDialog(param), param => CanAdminManageTasks && !IsLoading);
        EditTaskCommand = new RelayCommand(async param => await OpenEditTaskDialog(param),
            param => SelectedTask != null && SelectedTask.IsActive && CanAdminManageTasks && !IsLoading);
        DeleteTaskCommand = new RelayCommand(async _ => await DeleteTaskAsync(),
            _ => SelectedTask != null && SelectedTask.IsActive && CanAdminManageTasks && !IsLoading);

        UpdateStatusCommand = new RelayCommand<TaskStatus>(
            async (newStatus) => await UpdateTaskStatusAsync(newStatus),
            (newStatus) => SelectedTask != null && SelectedTask.IsActive && !IsLoading &&
                           (CanAdminManageTasks ||
                            (CanStaffUpdateStatusDirectly && SelectedTask.AssigneeId == _authenticationService.CurrentUser?.Id))
        );

        SearchCommand = new RelayCommand(async _ => {
            _currentPage = 1;
            OnPropertyChanged(nameof(CurrentPage));
            await LoadTasksAsync();
        }, _ => !IsLoading);

        NextPageCommand = new RelayCommand(async _ => {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await LoadTasksAsync();
            }
        }, _ => CurrentPage < TotalPages && !IsLoading);

        PreviousPageCommand = new RelayCommand(async _ => {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadTasksAsync();
            }
        }, _ => CurrentPage > 1 && !IsLoading);

        RefreshCommand = new RelayCommand(async _ => await LoadTasksAsync(), _ => !IsLoading);

        _signalRService.NewTaskAssigned += OnSignalRNewTaskAssigned;
        _signalRService.TaskStatusUpdated += OnSignalRTaskStatusUpdated;

        _ = LoadTasksAsync();
    }

    private void OnSignalRNewTaskAssigned(TaskItem newTask)
    {
        var currentUser = _authenticationService?.CurrentUser;
        if (currentUser == null) return;

        bool shouldAddTask = false;
        if (currentUser.Role == UserRole.Admin)
        {
            shouldAddTask = true;
        }
        else if (currentUser.Role == UserRole.Staff && newTask.AssigneeId == currentUser.Id)
        {
            shouldAddTask = true;
        }

        if (shouldAddTask)
        {
            var existingTask = Tasks.FirstOrDefault(t => t.Id == newTask.Id);
            if (existingTask == null)
            {
                Application.Current.Dispatcher.Invoke(() => {
                    Tasks.Insert(0, newTask);
                    TotalItems++;
                    OnPropertyChanged(nameof(TotalPages));
                    (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
                });
                ShowToastNotification($"New task assigned: {newTask.Title}");
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
            ShowToastNotification($"Task updated: {updatedTask.Title} is now {updatedTask.Status}");
        }
        else
        {
            var currentUser = _authenticationService?.CurrentUser;
            if (currentUser != null && currentUser.Role == UserRole.Staff && updatedTask.AssigneeId == currentUser.Id)
            {
                ShowToastNotification($"Task assigned to you was updated: {updatedTask.Title} is now {updatedTask.Status}");
            }
            else if (currentUser != null && currentUser.Role == UserRole.Admin)
            {
                ShowToastNotification($"Task updated: {updatedTask.Title} is now {updatedTask.Status}");
            }
        }
    }

    private void ShowToastNotification(string message)
    {
        // Debug.WriteLine($"TOAST: {message}");
    }


    private async void OnSearchTermChanged()
    {
        if (_taskService == null) return;
        try
        {
            _currentPage = 1;
            OnPropertyChanged(nameof(CurrentPage));
            await LoadTasksAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in OnSearchTermChanged (TaskManagement): {ex.Message}");
        }
    }
    private async void OnFilterStatusChanged()
    {
        if (_taskService == null) return;
        try
        {
            _currentPage = 1;
            OnPropertyChanged(nameof(CurrentPage));
            await LoadTasksAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in OnFilterStatusChanged (TaskManagement): {ex.Message}");
        }
    }
    private async void OnPageSizeChanged()
    {
        if (_taskService == null) return;
        try
        {
            await LoadTasksAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in OnPageSizeChanged (TaskManagement): {ex.Message}");
        }
    }

    public async Task LoadTasksAsync()
    {
        if (_taskService == null || _authenticationService == null) return;
        if (IsLoading) return;
        IsLoading = true;
        try
        {
            if (PageSize <= 0) _pageSize = 10;

            IEnumerable<TaskItem> tasksFromService;
            var currentUser = _authenticationService.CurrentUser; // currentUser.Id giờ là Guid
            if (currentUser == null)
            {
                Tasks = new ObservableCollection<TaskItem>();
                TotalItems = 0;
                return;
            }

            if (currentUser.Role == UserRole.Admin)
            {
                tasksFromService = await _taskService.GetAllTasksAsync(SearchTerm, SelectedFilterStatus, includeInactive: true);
            }
            else
            {
                tasksFromService = await _taskService.GetTasksByAssigneeAsync(currentUser.Id, SearchTerm, SelectedFilterStatus, includeInactive: false);
            }

            TotalItems = tasksFromService.Count();
            var pagedTasks = tasksFromService
                                .OrderByDescending(t => t.CreatedDate)
                                .Skip((CurrentPage - 1) * PageSize)
                                .Take(PageSize);

            Application.Current.Dispatcher.Invoke(() => {
                Tasks.Clear();
                foreach (var task in pagedTasks)
                {
                    Tasks.Add(task);
                }
                SelectedTask = null;
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load tasks: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task OpenAddTaskDialog(object? parameter)
    {
        if (_serviceProvider == null) return;
        var addEditTaskViewModel = _serviceProvider.GetRequiredService<AddEditTaskViewModel>();
        addEditTaskViewModel.InitializeForAdd();

        addEditTaskViewModel.CloseActionWithResult = async (success) =>
        {
            // Debug.WriteLine($"AddEditTaskViewModel CloseActionWithResult called with: {success} from TaskManagementViewModel");
            if (success)
            {
                await LoadTasksAsync();
            }
        };

        // Debug.WriteLine("Simulating showing Add Task Dialog...");
        await Task.Delay(1);
        // Debug.WriteLine("Add Task Dialog simulation finished.");
    }

    private async Task OpenEditTaskDialog(object? parameter)
    {
        if (SelectedTask == null || _serviceProvider == null) return; // SelectedTask.Id giờ là Guid
        var addEditTaskViewModel = _serviceProvider.GetRequiredService<AddEditTaskViewModel>();
        addEditTaskViewModel.InitializeForEdit(SelectedTask);

        addEditTaskViewModel.CloseActionWithResult = async (success) =>
        {
            // Debug.WriteLine($"AddEditTaskViewModel CloseActionWithResult called with: {success} from TaskManagementViewModel");
            if (success)
            {
                await LoadTasksAsync();
            }
        };

        // Debug.WriteLine($"Simulating showing Edit Task Dialog for '{SelectedTask.Title}'...");
        await Task.Delay(1);
        // Debug.WriteLine("Edit Task Dialog simulation finished.");
    }


    private async Task DeleteTaskAsync()
    {
        if (SelectedTask == null || !CanAdminManageTasks || _taskService == null) return; // SelectedTask.Id giờ là Guid
        try
        {
            bool success = await _taskService.DeleteTaskAsync(SelectedTask.Id);
            if (success)
            {
                await LoadTasksAsync();
            }
            else
            {
                // Debug.WriteLine($"Soft delete failed for task '{SelectedTask.Title}'");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting task: {ex.Message}");
        }
    }

    private async Task UpdateTaskStatusAsync(TaskStatus newStatus)
    {
        if (SelectedTask == null || _taskService == null || _authenticationService == null) return; // SelectedTask.Id giờ là Guid

        var currentUser = _authenticationService.CurrentUser; // currentUser.Id giờ là Guid
        if (currentUser == null) return;

        bool canUpdate = currentUser.Role == UserRole.Admin ||
                         (currentUser.Role == UserRole.Staff && SelectedTask.AssigneeId == currentUser.Id);

        if (!canUpdate)
        {
            // Debug.WriteLine("User does not have permission to update this task's status.");
            return;
        }

        try
        {
            bool success = await _taskService.UpdateTaskStatusAsync(SelectedTask.Id, newStatus); // SelectedTask.Id giờ là Guid
            if (success)
            {
                Application.Current.Dispatcher.Invoke(() => {
                    var taskToUpdate = Tasks.FirstOrDefault(t => t.Id == SelectedTask.Id);
                    if (taskToUpdate != null)
                    {
                        taskToUpdate.Status = newStatus;
                    }
                });
            }
            else
            {
                // Debug.WriteLine($"Failed to update status for task '{SelectedTask.Title}'");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating task status: {ex.Message}");
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