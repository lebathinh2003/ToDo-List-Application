using System.Diagnostics;
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
            Debug.WriteLine($"MainViewModel.CurrentUser SETTER: Trying to set. Current _currentUser IsNull? {_currentUser == null}. New value IsNull? {value == null}.");
            if (value != null)
            {
                Debug.WriteLine($"MainViewModel.CurrentUser SETTER: New value User ID: {value.Id}, Username: {value.Username}");
            }

            if (SetProperty(ref _currentUser, value))
            {
                Debug.WriteLine($"MainViewModel.CurrentUser SETTER: SetProperty successful. _currentUser IsNull? {_currentUser == null}. IsLoggedIn immediately after set: {IsLoggedIn}");
                OnPropertyChanged(nameof(IsLoggedIn));
                OnPropertyChanged(nameof(IsAdmin));
                OnPropertyChanged(nameof(IsStaff));
                UpdateNavigationAndUIState();
            }
            else
            {
                Debug.WriteLine($"MainViewModel.CurrentUser SETTER: SetProperty returned false (value was same as old or null). _currentUser IsNull? {_currentUser == null}. Value passed IsNull? {value == null}. IsLoggedIn: {IsLoggedIn}");
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

    public ICommand NavigateToProfileCommand { get; }
    public ICommand NavigateToStaffManagementCommand { get; }
    public ICommand NavigateToTaskManagementCommand { get; }
    public ICommand LogoutCommand { get; }

    public MainViewModel(IAuthenticationService authenticationService,
                         INavigationService navigationService,
                         IServiceProvider serviceProvider,
                         ISignalRService signalRService)
    {
        _authenticationService = authenticationService;
        _navigationService = navigationService;
        _serviceProvider = serviceProvider;
        _signalRService = signalRService;

        _navigationService.CurrentViewChanged += OnCurrentViewChanged;

        NavigateToProfileCommand = new RelayCommand(_ => _navigationService.NavigateTo<ProfileViewModel>(), _ => IsLoggedIn);
        NavigateToStaffManagementCommand = new RelayCommand(_ => _navigationService.NavigateTo<StaffManagementViewModel>(), _ => IsLoggedIn && IsAdmin);
        NavigateToTaskManagementCommand = new RelayCommand(_ => _navigationService.NavigateTo<TaskManagementViewModel>(), _ => IsLoggedIn);
        LogoutCommand = new RelayCommand(async _ => await Logout(), _ => IsLoggedIn);

        _ = InitializeAuthenticationStateAsync();
    }

    private async Task InitializeAuthenticationStateAsync()
    {
        Debug.WriteLine("MainViewModel.InitializeAuthenticationStateAsync: Checking authentication state...");
        if (await _authenticationService.IsUserAuthenticatedAsync())
        {
            Debug.WriteLine("MainViewModel.InitializeAuthenticationStateAsync: User IS authenticated. Setting CurrentUser.");
            CurrentUser = _authenticationService.CurrentUser;
        }
        else
        {
            Debug.WriteLine("MainViewModel.InitializeAuthenticationStateAsync: User IS NOT authenticated. Navigating to LoginView.");
            // Chỉ điều hướng, việc gán LoginSuccessAction sẽ được thực hiện trong OnCurrentViewChanged
            _navigationService.NavigateTo<LoginViewModel>();
        }
    }

    private void OnLoginSuccess(User loggedInUser)
    {
        Debug.WriteLine($"MainViewModel.OnLoginSuccess: ENTERED. Received loggedInUser. IsNull? {loggedInUser == null}");
        if (loggedInUser != null)
        {
            Debug.WriteLine($"MainViewModel.OnLoginSuccess: User ID: {loggedInUser.Id}, Username: {loggedInUser.Username}, Role: {loggedInUser.Role}");
        }
        CurrentUser = loggedInUser;
        Debug.WriteLine($"MainViewModel.OnLoginSuccess: CurrentUser is set. IsLoggedIn now: {IsLoggedIn}. UpdateNavigationAndUIState will be called by CurrentUser setter.");
    }

    private void UpdateNavigationAndUIState()
    {
        Debug.WriteLine($"MainViewModel.UpdateNavigationAndUIState: Called. IsLoggedIn: {IsLoggedIn}, CurrentViewModel is {CurrentViewModel?.GetType().Name ?? "null"}");

        (NavigateToProfileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NavigateToStaffManagementCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NavigateToTaskManagementCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (LogoutCommand as RelayCommand)?.RaiseCanExecuteChanged();

        if (IsLoggedIn)
        {
            Debug.WriteLine("MainViewModel.UpdateNavigationAndUIState: User is logged in. Connecting SignalR.");
            _ = _signalRService.ConnectAsync();
            if (CurrentViewModel is LoginViewModel || CurrentViewModel == null)
            {
                Debug.WriteLine("MainViewModel.UpdateNavigationAndUIState: Current view is Login or null, navigating to Profile.");
                _navigationService.NavigateTo<ProfileViewModel>();
            }
        }
        else
        {
            Debug.WriteLine("MainViewModel.UpdateNavigationAndUIState: User is NOT logged in. Disconnecting SignalR.");
            _ = _signalRService.DisconnectAsync();
            if (!(CurrentViewModel is LoginViewModel))
            {
                Debug.WriteLine("MainViewModel.UpdateNavigationAndUIState: Current view is not Login. Navigating to LoginView.");
                // Chỉ điều hướng, việc gán LoginSuccessAction sẽ được thực hiện trong OnCurrentViewChanged
                _navigationService.NavigateTo<LoginViewModel>();
            }
        }
    }

    private void UpdateActiveViewStates()
    {
        IsProfileViewActive = CurrentViewModel is ProfileViewModel;
        IsStaffManagementViewActive = CurrentViewModel is StaffManagementViewModel;
        IsTaskManagementViewActive = CurrentViewModel is TaskManagementViewModel;
        Debug.WriteLine($"MainViewModel.UpdateActiveViewStates: ProfileActive={IsProfileViewActive}, StaffActive={IsStaffManagementViewActive}, TaskActive={IsTaskManagementViewActive}");
    }

    private void OnCurrentViewChanged()
    {
        Debug.WriteLine($"MainViewModel.OnCurrentViewChanged: NavigationService.CurrentView is now '{_navigationService.CurrentView?.GetType().FullName ?? "null"}'");
        CurrentViewModel = _navigationService.CurrentView;

        // ***** GÁN LOGINSUCCESSACTION CHO INSTANCE LOGINVIEWMODEL HIỆN TẠI *****
        if (CurrentViewModel is LoginViewModel loginVMInstance)
        {
            Debug.WriteLine("MainViewModel.OnCurrentViewChanged: CurrentViewModel is LoginViewModel. Assigning LoginSuccessAction.");
            loginVMInstance.LoginSuccessAction = OnLoginSuccess;
        }
        // ***** KẾT THÚC GÁN *****

        (NavigateToProfileCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NavigateToStaffManagementCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NavigateToTaskManagementCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (LogoutCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private TViewModel GetViewModel<TViewModel>() where TViewModel : ViewModelBase
    {
        return (TViewModel)_serviceProvider.GetRequiredService(typeof(TViewModel))!;
    }

    private async Task Logout()
    {
        Debug.WriteLine("MainViewModel.Logout: Logging out...");
        await _authenticationService.LogoutAsync();
        CurrentUser = null;
    }

    public override void Dispose()
    {
        _navigationService.CurrentViewChanged -= OnCurrentViewChanged;
        base.Dispose();
    }
}