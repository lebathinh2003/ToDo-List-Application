using System.Windows; 
using System.Windows.Input; 
using Microsoft.Extensions.DependencyInjection; 
using WpfTaskManagerApp.Core; 
using WpfTaskManagerApp.Interfaces;
using WpfTaskManagerApp.Models; 
using WpfTaskManagerApp.ViewModels.Common; 
using WpfTaskManagerApp.Views; 

namespace WpfTaskManagerApp.ViewModels;

// Main application ViewModel, handles overall state and navigation.
public class MainViewModel : ViewModelBase, IDisposable
{
    // Injected services.
    private readonly IAuthenticationService _authenticationService;
    private readonly INavigationService _navigationService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISignalRService _signalRService;

    // ViewModel for toast notifications.
    public ToastNotificationViewModel ToastViewModel { get; }

    // Currently displayed ViewModel in the main content area.
    private ViewModelBase? _currentViewModel;
    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            if (SetProperty(ref _currentViewModel, value))
            {
                UpdateActiveViewStates(); // Update UI indicators.
            }
        }
    }

    // Currently authenticated user.
    private User? _currentUser;
    public User? CurrentUser
    {
        get => _currentUser;
        set
        {
            if (SetProperty(ref _currentUser, value))
            {
                OnPropertyChanged(nameof(IsLoggedIn));
                OnPropertyChanged(nameof(IsAdmin));
                OnPropertyChanged(nameof(IsStaff));
                UpdateNavigationAndUIState(); // Adjust UI based on auth state.
            }
        }
    }

    // True if a user is logged in.
    public bool IsLoggedIn => CurrentUser != null;
    // True if the current user is an Admin.
    public bool IsAdmin => CurrentUser?.Role == UserRole.Admin;
    // True if the current user is Staff.
    public bool IsStaff => CurrentUser?.Role == UserRole.Staff;

    // UI state flags for active views.
    private bool _isProfileViewActive;
    public bool IsProfileViewActive { get => _isProfileViewActive; set => SetProperty(ref _isProfileViewActive, value); }
    private bool _isStaffManagementViewActive;
    public bool IsStaffManagementViewActive { get => _isStaffManagementViewActive; set => SetProperty(ref _isStaffManagementViewActive, value); }
    private bool _isTaskManagementViewActive;
    public bool IsTaskManagementViewActive { get => _isTaskManagementViewActive; set => SetProperty(ref _isTaskManagementViewActive, value); }

    // Navigation commands.
    public ICommand NavigateToProfileCommand { get; }
    public ICommand NavigateToStaffManagementCommand { get; }
    public ICommand NavigateToTaskManagementCommand { get; }
    // Logout command.
    public ICommand LogoutCommand { get; }

    // Constructor.
    public MainViewModel(
        IAuthenticationService authenticationService,
        INavigationService navigationService,
        IServiceProvider serviceProvider,
        ISignalRService signalRService,
        ToastNotificationViewModel toastViewModel)
    {
        _authenticationService = authenticationService;
        _navigationService = navigationService;
        _serviceProvider = serviceProvider;
        _signalRService = signalRService;
        ToastViewModel = toastViewModel;

        _navigationService.CurrentViewChanged += OnCurrentViewChanged; // Subscribe to view changes.

        // Initialize commands.
        NavigateToProfileCommand = new RelayCommand(_ => _navigationService.NavigateTo<ProfileViewModel>(), _ => IsLoggedIn);
        NavigateToStaffManagementCommand = new RelayCommand(_ => _navigationService.NavigateTo<StaffManagementViewModel>(), _ => IsLoggedIn && IsAdmin);
        NavigateToTaskManagementCommand = new RelayCommand(_ => _navigationService.NavigateTo<TaskManagementViewModel>(), _ => IsLoggedIn);
        LogoutCommand = new RelayCommand(async _ => await Logout(), _ => IsLoggedIn);

        // Subscribe to SignalR events.
        _signalRService.NewTaskAssigned += OnSignalRNewTaskAssigned;
        _signalRService.TaskStatusUpdated += OnSignalRTaskStatusUpdated;
        _signalRService.ForceLogoutReceived += OnForceLogoutReceived;
        _signalRService.ReloadReceived += OnReloadReceived;

        _ = InitializeAuthenticationStateAsync(); // Check initial auth status.
    }

    // Shows a toast notification.
    public void ShowToast(string message, ToastType type = ToastType.Information, int duration = 4)
    {
        ToastViewModel.Show(message, type, duration);
    }

    // Initializes authentication state on startup.
    private async Task InitializeAuthenticationStateAsync()
    {
        if (await _authenticationService.IsUserAuthenticatedAsync())
        {
            CurrentUser = _authenticationService.CurrentUser; // Set user if already authenticated.
        }
        else
        {
            var loginVM = GetViewModel<LoginViewModel>();
            loginVM.LoginSuccessAction = OnLoginSuccess; // Setup callback for login.
            _navigationService.NavigateTo<LoginViewModel>(); // Navigate to login screen.
        }
    }

    // Callback for successful login.
    private void OnLoginSuccess(User loggedInUser)
    {
        CurrentUser = loggedInUser; // Set current user.
    }

    // Handles SignalR event for new task assignment.
    private void OnSignalRNewTaskAssigned(TaskItem newTask)
    {
        var currentUser = _authenticationService.CurrentUser;
        if (currentUser != null && (currentUser.Role == UserRole.Admin || newTask.AssigneeId == currentUser.Id))
        {
            Application.Current.Dispatcher.Invoke(async () => // Ensure UI updates on UI thread.
            {
                (Application.Current.MainWindow as MainWindow)?.ShowTrayNotification(
                    "New Task Assigned",
                    $"Task: {newTask.Title}\nAssignee: {newTask.AssigneeName ?? "N/A"}\nClick to view details.",
                    newTask
                );
                if (CurrentViewModel is TaskManagementViewModel tmvm) await tmvm.LoadTasksAsync(); // Refresh task list if active.
            });
        }
    }

    // Handles SignalR event for task status update.
    private void OnSignalRTaskStatusUpdated(TaskItem updatedTask)
    {
        var currentUser = _authenticationService.CurrentUser;
        if (currentUser != null && (currentUser.Role == UserRole.Admin || updatedTask.AssigneeId == currentUser.Id))
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                (Application.Current.MainWindow as MainWindow)?.ShowTrayNotification(
                    "Task Status Updated",
                    $"Task '{updatedTask.Title}' is now {updatedTask.Status}.\nClick to view.",
                    updatedTask
                );
                if (CurrentViewModel is TaskManagementViewModel tmvm) await tmvm.LoadTasksAsync(); // Refresh task list if active.
            });
        }
    }

    // Handles SignalR event for forced logout.
    private async void OnForceLogoutReceived(string? reason)
    {
        if (IsLoggedIn)
        {
            string? loggedOutUserName = CurrentUser?.Username;
            await _authenticationService.LogoutAsync(); // Perform logout.
            CurrentUser = null; // Clear current user, triggers UI update.

            Application.Current.Dispatcher.Invoke(() => // UI operations on UI thread.
            {
                string message = $"User '{loggedOutUserName}' has been logged out by an administrator.";
                if (!string.IsNullOrEmpty(reason)) message += $" Reason: {reason}";
                ToastViewModel.Show(message, ToastType.Warning, 10); // Show long toast.

                if (Application.Current.MainWindow is MainWindow mainWindow && !mainWindow.IsVisible)
                {
                    mainWindow.ShowWindowAndActivate(); // Ensure login screen is visible.
                }
            });
        }
    }

    // Handles SignalR event for data reload request.
    private async void OnReloadReceived()
    {
        if (CurrentViewModel is TaskManagementViewModel tmvm) await tmvm.LoadTasksAsync(); // Reload tasks.
        if (CurrentViewModel is StaffManagementViewModel smvm) await smvm.LoadStaffAsync(); // Reload staff.
    }

    // Handles activation of tray notification (user click).
    public void HandleTrayNotificationActivation(TaskItem? task)
    {
        OpenEditTaskDialogFromNotification(task);
    }

    // Opens the Add/Edit Task dialog from a tray notification.
    private async void OpenEditTaskDialogFromNotification(TaskItem? task)
    {
        if (task == null)
        {
            ToastViewModel?.Show("Cannot open task details: Task information is missing.", ToastType.Error);
            return;
        }
        if (_serviceProvider == null)
        {
            ToastViewModel?.Show("Cannot open task details: Service provider is not available.", ToastType.Error);
            return;
        }

        if (Application.Current.MainWindow is MainWindow mainWindow) // Ensure main window is active.
        {
            if (!mainWindow.IsVisible) mainWindow.Show();
            if (mainWindow.WindowState == WindowState.Minimized) mainWindow.WindowState = WindowState.Normal;
            mainWindow.Activate();
            if (!mainWindow.Topmost) { mainWindow.Topmost = true; mainWindow.Topmost = false; }
        }

        try
        {
            var addEditTaskViewModel = _serviceProvider.GetRequiredService<AddEditTaskViewModel>();
            await addEditTaskViewModel.LoadAssignableUsersAsync(); // Load users.
            addEditTaskViewModel.InitializeForEdit(task); // Init for edit.
            await addEditTaskViewModel.LoadAssignableUsersAsync(); // Reload users if context changed.

            var dialogView = new AddEditTaskDialog { DataContext = addEditTaskViewModel };
            var dialogWindow = new Window // Create dialog window.
            {
                Title = addEditTaskViewModel.WindowTitle,
                Content = dialogView,
                Width = 520,
                Height = 800,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Application.Current.MainWindow,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.ToolWindow
            };

            addEditTaskViewModel.CloseActionWithResult = (success) => // Handle dialog close.
            {
                dialogWindow.DialogResult = success; dialogWindow.Close();
                if (success && CurrentViewModel is TaskManagementViewModel tmvm)
                {
                    _ = tmvm.LoadTasksAsync(); // Refresh tasks if successful.
                }
            };
            dialogWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            ToastViewModel.Show($"Error opening task details: {ex.Message}", ToastType.Error);
        }
    }

    // Updates navigation UI and connects/disconnects SignalR based on login state.
    private void UpdateNavigationAndUIState()
    {
        (NavigateToProfileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NavigateToStaffManagementCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NavigateToTaskManagementCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (LogoutCommand as RelayCommand)?.RaiseCanExecuteChanged();

        if (IsLoggedIn)
        {
            _ = _signalRService.ConnectAsync(); // Connect SignalR.
            if (CurrentViewModel is LoginViewModel || CurrentViewModel == null) // If on login or no view, navigate to profile.
            {
                _navigationService.NavigateTo<ProfileViewModel>();
            }
        }
        else
        {
            _ = _signalRService.DisconnectAsync(); // Disconnect SignalR.
            if (!(CurrentViewModel is LoginViewModel)) // If not on login, navigate to login.
            {
                var loginVM = GetViewModel<LoginViewModel>();
                loginVM.LoginSuccessAction = OnLoginSuccess;
                _navigationService.NavigateTo<LoginViewModel>();
            }
        }
    }

    // Updates UI state for active navigation items.
    private void UpdateActiveViewStates()
    {
        IsProfileViewActive = CurrentViewModel is ProfileViewModel;
        IsStaffManagementViewActive = CurrentViewModel is StaffManagementViewModel;
        IsTaskManagementViewActive = CurrentViewModel is TaskManagementViewModel;
    }

    // Handles changes to the current view from navigation service.
    private void OnCurrentViewChanged()
    {
        CurrentViewModel = _navigationService.CurrentView;
        if (CurrentViewModel is LoginViewModel loginVMInstance && loginVMInstance.LoginSuccessAction == null)
        {
            loginVMInstance.LoginSuccessAction = OnLoginSuccess; // Ensure login callback is set.
        }
        (NavigateToProfileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NavigateToStaffManagementCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NavigateToTaskManagementCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (LogoutCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    // Retrieves a ViewModel instance from service provider.
    private TViewModel GetViewModel<TViewModel>() where TViewModel : ViewModelBase
    {
        return _serviceProvider.GetRequiredService<TViewModel>();
    }

    // Logs out the current user.
    private async Task Logout()
    {
        await _authenticationService.LogoutAsync();
        CurrentUser = null; // Clears user, triggers UI updates.
    }

    // Disposes resources, unsubscribes from events.
    public override void Dispose()
    {
        _navigationService.CurrentViewChanged -= OnCurrentViewChanged;
        if (_signalRService != null) // Unsubscribe from SignalR events.
        {
            _signalRService.NewTaskAssigned -= OnSignalRNewTaskAssigned;
            _signalRService.TaskStatusUpdated -= OnSignalRTaskStatusUpdated;
            _signalRService.ForceLogoutReceived -= OnForceLogoutReceived;
            _signalRService.ReloadReceived -= OnReloadReceived;
        }
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}