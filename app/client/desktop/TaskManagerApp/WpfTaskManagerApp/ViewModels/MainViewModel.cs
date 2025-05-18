using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.Services;
using WpfTaskManagerApp.ViewModels.Common;
namespace WpfTaskManagerApp.ViewModels;
public class MainViewModel : ViewModelBase, IDisposable
{
    private readonly IAuthenticationService _authenticationService;
    private readonly INavigationService _navigationService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISignalRService _signalRService;
    public ToastNotificationViewModel ToastViewModel
    {
        get;
    }

    private ViewModelBase? _currentViewModel;
    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            if (SetProperty(ref _currentViewModel, value))
            {
                UpdateActiveViewStates();
            }
        }
    }
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
                UpdateNavigationAndUIState();
            }
        }
    }
    public bool IsLoggedIn => CurrentUser != null;
    public bool IsAdmin => CurrentUser?.Role == UserRole.Admin;
    public bool IsStaff => CurrentUser?.Role == UserRole.Staff;
    private bool _isProfileViewActive;
    public bool IsProfileViewActive
    {
        get => _isProfileViewActive;
        set => SetProperty(ref _isProfileViewActive, value);
    }
    private bool _isStaffManagementViewActive;
    public bool IsStaffManagementViewActive
    {
        get => _isStaffManagementViewActive;
        set => SetProperty(ref _isStaffManagementViewActive, value);
    }
    private bool _isTaskManagementViewActive;
    public bool IsTaskManagementViewActive
    {
        get => _isTaskManagementViewActive;
        set => SetProperty(ref _isTaskManagementViewActive, value);
    }

    public ICommand NavigateToProfileCommand
    {
        get;
    }
    public ICommand NavigateToStaffManagementCommand
    {
        get;
    }
    public ICommand NavigateToTaskManagementCommand
    {
        get;
    }
    public ICommand LogoutCommand
    {
        get;
    }

    public MainViewModel(IAuthenticationService authenticationService, INavigationService navigationService, IServiceProvider serviceProvider, ISignalRService signalRService, ToastNotificationViewModel toastViewModel)
    {
        _authenticationService = authenticationService;
        _navigationService = navigationService;
        _serviceProvider = serviceProvider;
        _signalRService = signalRService;
        ToastViewModel = toastViewModel;
        _navigationService.CurrentViewChanged += OnCurrentViewChanged;
        NavigateToProfileCommand = new RelayCommand(_ => _navigationService.NavigateTo<ProfileViewModel>(), _ => IsLoggedIn);
        NavigateToStaffManagementCommand = new RelayCommand(_ => _navigationService.NavigateTo<StaffManagementViewModel>(), _ => IsLoggedIn && IsAdmin);
        NavigateToTaskManagementCommand = new RelayCommand(_ => {
            _navigationService.NavigateTo<TaskManagementViewModel>();
            Debug.WriteLine("MainViewModel: NavigateToTaskManagementCommand Executed.");
        }, _ => IsLoggedIn);
        LogoutCommand = new RelayCommand(async _ => await Logout(), _ => IsLoggedIn);
        // ***** ĐĂNG KÝ SỰ KIỆN SIGNALR *****
        _signalRService.NewTaskAssigned += OnSignalRNewTaskAssigned;
        _signalRService.TaskStatusUpdated += OnSignalRTaskStatusUpdated;
        // ***** KẾT THÚC ĐĂNG KÝ *****
        _ = InitializeAuthenticationStateAsync();
    }
    // Phương thức ShowToast cũ vẫn có thể dùng cho các toast không cần click action
    public void ShowToast(string message, ToastType type = ToastType.Information, int duration = 4)
    {
        ToastViewModel.Show(message, type, duration);
    }

    private async Task InitializeAuthenticationStateAsync()
    {
        if (await _authenticationService.IsUserAuthenticatedAsync())
        {
            CurrentUser = _authenticationService.CurrentUser;
        }
        else
        {
            var loginVM = GetViewModel<LoginViewModel>();
            loginVM.LoginSuccessAction = OnLoginSuccess;
            _navigationService.NavigateTo<LoginViewModel>();
        }
    }

    private void OnLoginSuccess(User loggedInUser)
    {
        CurrentUser = loggedInUser;
    }

    private void OnSignalRNewTaskAssigned(TaskItem newTask)
    {
        Debug.WriteLine($"MainViewModel: Received SignalR NewTaskAssigned: {newTask.Title}");
        var currentUser = _authenticationService.CurrentUser;
        // Chỉ hiển thị toast nếu task được gán cho user hiện tại (nếu là staff) hoặc nếu là admin (admin có thể muốn biết)
        // Hoặc có thể server chỉ gửi thông báo này cho đúng người dùng.
        if (currentUser != null && (currentUser.Role == UserRole.Admin || newTask.AssigneeId == currentUser.Id))
        {
            Application.Current.Dispatcher.Invoke(() => // Đảm bảo chạy trên UI thread
            {
                ToastViewModel.Show(
                    $"New Task Assigned: {newTask.Title}",
                    ToastType.Information,
                    10, // Thời gian hiển thị lâu hơn cho thông báo quan trọng
                    newTask,
                    HandleToastNotificationClick // Action khi click
                );
            });
        }
    }


    private void OnSignalRTaskStatusUpdated(TaskItem updatedTask)
    {
        Debug.WriteLine($"MainViewModel: Received SignalR TaskStatusUpdated: {updatedTask.Title} to {updatedTask.Status}");
        // Có thể hiển thị toast hoặc cập nhật UI trực tiếp nếu View hiện tại là TaskManagementView
        // Ví dụ, nếu CurrentViewModel là TaskManagementViewModel, gọi một phương thức trên đó để cập nhật task
        if (CurrentViewModel is TaskManagementViewModel tmvm)
        {
            Application.Current.Dispatcher.Invoke(() => tmvm.UpdateTaskInList(updatedTask));
        }
        // Hoặc hiển thị một toast đơn giản
        // ShowToast($"Task '{updatedTask.Title}' status updated to {updatedTask.Status}.", ToastType.Information);
    }

    // ***** XỬ LÝ KHI CLICK VÀO TOAST THÔNG BÁO TASK MỚI *****
    private void HandleToastNotificationClick(TaskItem? task)
    {
        if (task == null || _serviceProvider == null) return;

        Debug.WriteLine($"MainViewModel: Toast for task '{task.Title}' clicked. Navigating to edit.");

        // Lấy AddEditTaskViewModel, khởi tạo và điều hướng
        var addEditTaskViewModel = _serviceProvider.GetRequiredService<AddEditTaskViewModel>();

        // LoadAssignableUsersAsync phải được gọi trước InitializeForEdit nếu nó cần danh sách user
        // Tuy nhiên, AddEditTaskViewModel đã có logic này rồi.
        // await addEditTaskViewModel.LoadAssignableUsersAsync(); // ViewModel sẽ tự gọi khi cần

        addEditTaskViewModel.InitializeForEdit(task);

        // Thay vì dùng NavigationService (vì nó tạo instance mới), set trực tiếp CurrentViewModel
        // Hoặc nếu muốn dùng NavigationService, cần cơ chế truyền tham số hoặc ViewModel đã khởi tạo
        CurrentViewModel = addEditTaskViewModel;
        // Hoặc _navigationService.NavigateTo<AddEditTaskViewModel>(); và AddEditTaskViewModel tự lấy task cần edit
        // từ một shared service/state. Cách set CurrentViewModel trực tiếp đơn giản hơn cho trường hợp này.

        // Nếu MainWindow đang bị ẩn, hiện nó lên
        if (Application.Current.MainWindow != null && !Application.Current.MainWindow.IsVisible)
        {
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }
        Application.Current.MainWindow?.Activate();
    }

    private void UpdateNavigationAndUIState()
    {
        (NavigateToProfileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NavigateToStaffManagementCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NavigateToTaskManagementCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (LogoutCommand as RelayCommand)?.RaiseCanExecuteChanged();
        if (IsLoggedIn)
        {
            _ = _signalRService.ConnectAsync();
            if (CurrentViewModel is LoginViewModel || CurrentViewModel == null) _navigationService.NavigateTo<ProfileViewModel>();
        }
        else
        {
            _ = _signalRService.DisconnectAsync();
            if (!(CurrentViewModel is LoginViewModel))
            {
                var loginVM = GetViewModel<LoginViewModel>();
                loginVM.LoginSuccessAction = OnLoginSuccess;
                _navigationService.NavigateTo<LoginViewModel>();
            }
        }
    }
    private void UpdateActiveViewStates()
    {
        IsProfileViewActive = CurrentViewModel is ProfileViewModel;
        IsStaffManagementViewActive = CurrentViewModel is StaffManagementViewModel;
        IsTaskManagementViewActive = CurrentViewModel is TaskManagementViewModel;
        Debug.WriteLine($"MainViewModel.UpdateActiveViewStates: ProfileActive={IsProfileViewActive}, StaffActive={IsStaffManagementViewActive}, TaskActive={IsTaskManagementViewActive}. CurrentViewModel is {CurrentViewModel?.GetType().Name}");
    }
    private void OnCurrentViewChanged()
    {
        Debug.WriteLine($"MainViewModel.OnCurrentViewChanged: NavigationService.CurrentView is now '{_navigationService.CurrentView?.GetType().FullName ?? null}");
        CurrentViewModel = _navigationService.CurrentView;
        if (CurrentViewModel is LoginViewModel loginVMInstance && loginVMInstance.LoginSuccessAction == null)
        {
            loginVMInstance.LoginSuccessAction = OnLoginSuccess;
        }
        (NavigateToProfileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NavigateToStaffManagementCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NavigateToTaskManagementCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (LogoutCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
    private TViewModel GetViewModel<TViewModel>() where TViewModel : ViewModelBase
    {
        return _serviceProvider.GetRequiredService<TViewModel>();
    }
    private async Task Logout()
    {
        await _authenticationService.LogoutAsync();
        CurrentUser = null;
    }
    public override void Dispose()
    {
        _navigationService.CurrentViewChanged -= OnCurrentViewChanged;
        // ***** HỦY ĐĂNG KÝ SỰ KIỆN SIGNALR *****
        if (_signalRService != null)
        {
            _signalRService.NewTaskAssigned -= OnSignalRNewTaskAssigned;
            _signalRService.TaskStatusUpdated -= OnSignalRTaskStatusUpdated;
        }
        // ***** KẾT THÚC HỦY ĐĂNG KÝ *****
        base.Dispose();
    }
}