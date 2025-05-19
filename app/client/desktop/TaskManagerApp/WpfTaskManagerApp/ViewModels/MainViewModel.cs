using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.Services;
using WpfTaskManagerApp.ViewModels.Common;
using WpfTaskManagerApp.Views;

namespace WpfTaskManagerApp.ViewModels;

/// <summary>
/// The main ViewModel for the application, orchestrating navigation,
/// authentication, SignalR communication, and overall application state.
/// </summary>
public class MainViewModel : ViewModelBase, IDisposable
{
    private readonly IAuthenticationService _authenticationService;
    private readonly INavigationService _navigationService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISignalRService _signalRService;

    /// <summary>
    /// Gets the ViewModel responsible for displaying toast notifications.
    /// </summary>
    public ToastNotificationViewModel ToastViewModel { get; }

    private ViewModelBase? _currentViewModel;
    /// <summary>
    /// Gets or sets the currently active ViewModel displayed in the main content area.
    /// Updates active view states when changed.
    /// </summary>
    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            if (SetProperty(ref _currentViewModel, value))
            {
                UpdateActiveViewStates(); // Reflect change in UI indicators
            }
        }
    }

    private User? _currentUser;
    /// <summary>
    /// Gets or sets the currently authenticated user.
    /// Updates login status, role-based properties, and navigation state when changed.
    /// </summary>
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
                UpdateNavigationAndUIState(); // Adjust UI and services based on auth state
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether a user is currently logged in.
    /// </summary>
    public bool IsLoggedIn => CurrentUser != null;

    /// <summary>
    /// Gets a value indicating whether the current user has an Admin role.
    /// </summary>
    public bool IsAdmin => CurrentUser?.Role == UserRole.Admin;

    /// <summary>
    /// Gets a value indicating whether the current user has a Staff role.
    /// </summary>
    public bool IsStaff => CurrentUser?.Role == UserRole.Staff;

    private bool _isProfileViewActive;
    /// <summary>
    /// Gets or sets a value indicating whether the Profile view is currently active.
    /// Used for UI elements like highlighting the active navigation item.
    /// </summary>
    public bool IsProfileViewActive
    {
        get => _isProfileViewActive;
        set => SetProperty(ref _isProfileViewActive, value);
    }

    private bool _isStaffManagementViewActive;
    /// <summary>
    /// Gets or sets a value indicating whether the Staff Management view is currently active.
    /// Used for UI elements like highlighting the active navigation item.
    /// </summary>
    public bool IsStaffManagementViewActive
    {
        get => _isStaffManagementViewActive;
        set => SetProperty(ref _isStaffManagementViewActive, value);
    }

    private bool _isTaskManagementViewActive;
    /// <summary>
    /// Gets or sets a value indicating whether the Task Management view is currently active.
    /// Used for UI elements like highlighting the active navigation item.
    /// </summary>
    public bool IsTaskManagementViewActive
    {
        get => _isTaskManagementViewActive;
        set => SetProperty(ref _isTaskManagementViewActive, value);
    }

    /// <summary>
    /// Command to navigate to the user's profile view.
    /// Can only execute if a user is logged in.
    /// </summary>
    public ICommand NavigateToProfileCommand { get; }

    /// <summary>
    /// Command to navigate to the staff management view.
    /// Can only execute if a user is logged in and is an Admin.
    /// </summary>
    public ICommand NavigateToStaffManagementCommand { get; }

    /// <summary>
    /// Command to navigate to the task management view.
    /// Can only execute if a user is logged in.
    /// </summary>
    public ICommand NavigateToTaskManagementCommand { get; }

    /// <summary>
    /// Command to log out the current user.
    /// Can only execute if a user is logged in.
    /// </summary>
    public ICommand LogoutCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    /// <param name="authenticationService">Service for user authentication.</param>
    /// <param name="navigationService">Service for view navigation.</param>
    /// <param name="serviceProvider">Service provider for resolving dependencies, like other ViewModels.</param>
    /// <param name="signalRService">Service for real-time communication via SignalR.</param>
    /// <param name="toastViewModel">ViewModel for managing toast notifications.</param>
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

        _navigationService.CurrentViewChanged += OnCurrentViewChanged;

        NavigateToProfileCommand = new RelayCommand(_ => _navigationService.NavigateTo<ProfileViewModel>(), _ => IsLoggedIn);
        NavigateToStaffManagementCommand = new RelayCommand(_ => _navigationService.NavigateTo<StaffManagementViewModel>(), _ => IsLoggedIn && IsAdmin);
        NavigateToTaskManagementCommand = new RelayCommand(_ => _navigationService.NavigateTo<TaskManagementViewModel>(), _ => IsLoggedIn);
        LogoutCommand = new RelayCommand(async _ => await Logout(), _ => IsLoggedIn);

        // Subscribe to SignalR events
        _signalRService.NewTaskAssigned += OnSignalRNewTaskAssigned;
        _signalRService.TaskStatusUpdated += OnSignalRTaskStatusUpdated;
        _signalRService.ForceLogoutReceived += OnForceLogoutReceived;
        _signalRService.ReloadReceived += OnReloadReceived;

        // Initialize authentication state and navigate accordingly
        _ = InitializeAuthenticationStateAsync();
    }

    /// <summary>
    /// Shows a toast notification.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="type">The type of toast notification (e.g., Information, Error).</param>
    /// <param name="duration">The duration in seconds for the toast to be visible.</param>
    public void ShowToast(string message, ToastType type = ToastType.Information, int duration = 4)
    {
        ToastViewModel.Show(message, type, duration);
    }

    /// <summary>
    /// Initializes the authentication state. If a user is already authenticated (e.g., from a saved token),
    /// sets the CurrentUser. Otherwise, navigates to the LoginView.
    /// </summary>
    private async Task InitializeAuthenticationStateAsync()
    {
        if (await _authenticationService.IsUserAuthenticatedAsync())
        {
            CurrentUser = _authenticationService.CurrentUser;
            // Navigation to default view for logged-in user (e.g., Profile) is handled by UpdateNavigationAndUIState
        }
        else
        {
            var loginVM = GetViewModel<LoginViewModel>();
            loginVM.LoginSuccessAction = OnLoginSuccess; // Set callback for successful login
            _navigationService.NavigateTo<LoginViewModel>();
        }
    }

    /// <summary>
    /// Callback method invoked when a user successfully logs in via the LoginViewModel.
    /// </summary>
    /// <param name="loggedInUser">The user who has successfully logged in.</param>
    private void OnLoginSuccess(User loggedInUser)
    {
        CurrentUser = loggedInUser;
    }

    /// <summary>
    /// Handles the SignalR event for a new task being assigned.
    /// Shows a tray notification and refreshes the task list if the TaskManagementView is active.
    /// </summary>
    /// <param name="newTask">The newly assigned task.</param>
    private void OnSignalRNewTaskAssigned(TaskItem newTask)
    {
        var currentUser = _authenticationService.CurrentUser;
        // Notify if the current user is an Admin or the assignee of the new task
        if (currentUser != null && (currentUser.Role == UserRole.Admin || newTask.AssigneeId == currentUser.Id))
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.ShowTrayNotification(
                    "New Task Assigned",
                    $"Task: {newTask.Title}\nAssignee: {newTask.AssigneeName ?? "N/A"}\nClick to view details.",
                    newTask
                );

                // If TaskManagementView is currently active, reload its tasks
                if (CurrentViewModel is TaskManagementViewModel tmvm)
                {
                    await tmvm.LoadTasksAsync();
                }
            });
        }
    }

    /// <summary>
    /// Handles the SignalR event for a task's status being updated.
    /// Shows a tray notification and refreshes the task list if the TaskManagementView is active.
    /// </summary>
    /// <param name="updatedTask">The task with an updated status.</param>
    private void OnSignalRTaskStatusUpdated(TaskItem updatedTask)
    {
        var currentUser = _authenticationService.CurrentUser;
        // Notify if the current user is an Admin or the assignee of the updated task
        if (currentUser != null && (currentUser.Role == UserRole.Admin || updatedTask.AssigneeId == currentUser.Id))
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.ShowTrayNotification(
                    "Task Status Updated",
                    $"Task '{updatedTask.Title}' is now {updatedTask.Status}.\nClick to view.",
                    updatedTask
                );

                // If TaskManagementView is currently active, reload its tasks
                if (CurrentViewModel is TaskManagementViewModel tmvm)
                {
                    await tmvm.LoadTasksAsync();
                }
            });
        }
    }

    /// <summary>
    /// Handles the SignalR event indicating a forced logout, typically initiated by an administrator.
    /// Logs out the current user, displays a notification, and navigates to the login screen.
    /// </summary>
    /// <param name="reason">The reason for the forced logout, if provided.</param>
    private async void OnForceLogoutReceived(string? reason)
    {
        if (IsLoggedIn)
        {
            string? loggedOutUserName = CurrentUser?.Username;

            // Perform the logout
            await _authenticationService.LogoutAsync();
            CurrentUser = null; // This will trigger UI updates via its setter

            // Display notification to the user on the UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                string message = $"User '{loggedOutUserName}' has been logged out by an administrator.";
                if (!string.IsNullOrEmpty(reason))
                {
                    message += $" Reason: {reason}";
                }
                ToastViewModel.Show(message, ToastType.Warning, 10); // Show a longer toast

                // Ensure the main window is visible to show the login screen
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null && !mainWindow.IsVisible)
                {
                    mainWindow.ShowWindowAndActivate();
                }
            });
        }
    }

    /// <summary>
    /// Handle signalR event for reload request.
    /// This method is called when the server sends a reload request.
    /// </summary>
    private async void OnReloadReceived()
    {
        // If TaskManagementView is currently active, reload its tasks
        if (CurrentViewModel is TaskManagementViewModel tmvm)
        {
            await tmvm.LoadTasksAsync();
        }

        if(CurrentViewModel is StaffManagementViewModel smvm)
        {
            await smvm.LoadStaffAsync();
        }
    }

    /// <summary>
    /// Handles the activation of a tray notification, typically by user click.
    /// Opens the edit task dialog for the associated task.
    /// </summary>
    /// <param name="task">The task associated with the activated notification.</param>
    public void HandleTrayNotificationActivation(TaskItem? task)
    {
        OpenEditTaskDialogFromNotification(task);
    }

    /// <summary>
    /// Opens the Add/Edit Task dialog to view or edit the specified task.
    /// This method is typically called when a user clicks a tray notification.
    /// Ensures the main window is visible and active before showing the dialog.
    /// </summary>
    /// <param name="task">The task to be displayed in the dialog.</param>
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

        // Ensure the main application window is visible and active
        var mainWindow = Application.Current.MainWindow as MainWindow;
        if (mainWindow != null)
        {
            if (!mainWindow.IsVisible) mainWindow.Show();
            if (mainWindow.WindowState == WindowState.Minimized) mainWindow.WindowState = WindowState.Normal;
            mainWindow.Activate();
            // Bring to front if it's not already the topmost window
            if (!mainWindow.Topmost)
            {
                mainWindow.Topmost = true;
                mainWindow.Topmost = false; // Reset Topmost to allow other windows to come on top later
            }
        }

        try
        {
            var addEditTaskViewModel = _serviceProvider.GetRequiredService<AddEditTaskViewModel>();

            // Load assignable users first, as InitializeForEdit might depend on this list
            // or its outcome might affect user selection availability.
            await addEditTaskViewModel.LoadAssignableUsersAsync();
            addEditTaskViewModel.InitializeForEdit(task);
            // Optionally, reload users if InitializeForEdit changes context that affects assignable users.
            // If LoadAssignableUsersAsync is idempotent or InitializeForEdit doesn't affect it, this second call might be redundant.
            // Kept for safety if InitializeForEdit clears or modifies user-related state.
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
                Owner = Application.Current.MainWindow, // Set owner for proper dialog behavior
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.ToolWindow
            };

            // Action to close the dialog and handle the result
            addEditTaskViewModel.CloseActionWithResult = (success) =>
            {
                dialogWindow.DialogResult = success;
                dialogWindow.Close();
                if (success)
                {
                    // If the TaskManagementView is active, refresh its task list
                    if (CurrentViewModel is TaskManagementViewModel tmvm)
                    {
                        _ = tmvm.LoadTasksAsync(); // Fire-and-forget task refresh
                    }
                }
            };

            dialogWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            // Log detailed error for developers
            // Debug.WriteLine($"Error opening edit task dialog from notification: {ex.Message}");
            ToastViewModel.Show($"Error opening task details: {ex.Message}", ToastType.Error);
        }
    }

    /// <summary>
    /// Updates navigation commands' CanExecute state and handles UI/service changes
    /// based on the user's login status. For example, connects/disconnects SignalR
    /// and navigates to the appropriate view (login or default authenticated view).
    /// </summary>
    private void UpdateNavigationAndUIState()
    {
        // Refresh CanExecute for commands that depend on login state or role
        (NavigateToProfileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NavigateToStaffManagementCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NavigateToTaskManagementCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (LogoutCommand as RelayCommand)?.RaiseCanExecuteChanged();

        if (IsLoggedIn)
        {
            _ = _signalRService.ConnectAsync(); // Connect to SignalR hub
            // If user is logged in and current view is LoginViewModel or null, navigate to a default authenticated view
            if (CurrentViewModel is LoginViewModel || CurrentViewModel == null)
            {
                _navigationService.NavigateTo<ProfileViewModel>();
            }
        }
        else
        {
            _ = _signalRService.DisconnectAsync(); // Disconnect from SignalR hub
            // If user is not logged in and not already on LoginViewModel, navigate to LoginViewModel
            if (!(CurrentViewModel is LoginViewModel))
            {
                var loginVM = GetViewModel<LoginViewModel>();
                loginVM.LoginSuccessAction = OnLoginSuccess; // Re-assign callback for successful login
                _navigationService.NavigateTo<LoginViewModel>();
            }
        }
    }

    /// <summary>
    /// Updates boolean properties that indicate which view is currently active.
    /// This is useful for UI elements like navigation menus to highlight the active item.
    /// </summary>
    private void UpdateActiveViewStates()
    {
        IsProfileViewActive = CurrentViewModel is ProfileViewModel;
        IsStaffManagementViewActive = CurrentViewModel is StaffManagementViewModel;
        IsTaskManagementViewActive = CurrentViewModel is TaskManagementViewModel;
    }

    /// <summary>
    /// Handles the <see cref="INavigationService.CurrentViewChanged"/> event.
    /// Updates the <see cref="CurrentViewModel"/> property and refreshes command states.
    /// If the new view is LoginViewModel, ensures its LoginSuccessAction is set.
    /// </summary>
    private void OnCurrentViewChanged()
    {
        CurrentViewModel = _navigationService.CurrentView;

        // If navigated to LoginView, ensure the success callback is wired up
        if (CurrentViewModel is LoginViewModel loginVMInstance && loginVMInstance.LoginSuccessAction == null)
        {
            loginVMInstance.LoginSuccessAction = OnLoginSuccess;
        }

        // Update command states as navigation might affect their CanExecute conditions
        (NavigateToProfileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NavigateToStaffManagementCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NavigateToTaskManagementCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (LogoutCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    /// <summary>
    /// Retrieves a ViewModel instance of the specified type from the service provider.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the ViewModel to retrieve.</typeparam>
    /// <returns>An instance of <typeparamref name="TViewModel"/>.</returns>
    private TViewModel GetViewModel<TViewModel>() where TViewModel : ViewModelBase
    {
        return _serviceProvider.GetRequiredService<TViewModel>();
    }

    /// <summary>
    /// Logs out the current user by calling the authentication service
    /// and clearing the <see cref="CurrentUser"/> property.
    /// </summary>
    private async Task Logout()
    {
        await _authenticationService.LogoutAsync();
        CurrentUser = null; // Triggers UI updates and navigation to login view
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// Unsubscribes from service events.
    /// </summary>
    public override void Dispose()
    {
        _navigationService.CurrentViewChanged -= OnCurrentViewChanged;

        // Unsubscribe from SignalR events to prevent memory leaks
        if (_signalRService != null)
        {
            _signalRService.NewTaskAssigned -= OnSignalRNewTaskAssigned;
            _signalRService.TaskStatusUpdated -= OnSignalRTaskStatusUpdated;
            _signalRService.ForceLogoutReceived -= OnForceLogoutReceived;
            // Consider disconnecting SignalR if not already handled by application shutdown logic
            // _ = _signalRService.DisconnectAsync(); // If appropriate here
        }

        base.Dispose();
        GC.SuppressFinalize(this); // Suppress finalization if Dispose is called
    }
}