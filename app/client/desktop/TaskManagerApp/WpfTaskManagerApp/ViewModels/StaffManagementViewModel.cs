using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input; 
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Interfaces;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.ViewModels.Common;
using WpfTaskManagerApp.Views;

namespace WpfTaskManagerApp.ViewModels;

// ViewModel for staff management.
public class StaffManagementViewModel : ViewModelBase
{
    // Services.
    private readonly IUserService? _userService;
    private readonly IServiceProvider? _serviceProvider;
    private readonly ToastNotificationViewModel? _toastViewModel;
    private readonly IAuthenticationService? _authenticationService; // Used for auth context if needed.

    // Loading state.
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (SetProperty(ref _isLoading, value))
            {
                (AddStaffCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (EditStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged();
                (DeleteStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged();
                (RestoreStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged();
                (AssignTaskCommand as RelayCommand<User>)?.RaiseCanExecuteChanged();
                (SearchCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (RefreshCommand as RelayCommand)?.RaiseCanExecuteChanged();
                UpdatePaginationCommandsCanExecute();
            }
        }
    }

    // Staff list.
    private ObservableCollection<User> _staffList = new ObservableCollection<User>();
    public ObservableCollection<User> StaffList
    {
        get => _staffList;
        set => SetProperty(ref _staffList, value);
    }

    // Selected staff member.
    private User? _selectedStaff;
    public User? SelectedStaff
    {
        get => _selectedStaff;
        set
        {
            if (SetProperty(ref _selectedStaff, value))
            {
                (EditStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged();
                (DeleteStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged();
                (RestoreStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged();
                (AssignTaskCommand as RelayCommand<User>)?.RaiseCanExecuteChanged();
            }
        }
    }

    // Search term.
    private string _searchTerm = string.Empty;
    public string SearchTerm
    {
        get => _searchTerm;
        set => SetProperty(ref _searchTerm, value);
    }

    // Sort property.
    private string? _sortBy;
    public string? SortBy
    {
        get => _sortBy;
        set
        {
            if (SetProperty(ref _sortBy, value))
            {
                CurrentPage = 1;
                _ = LoadStaffAsync();
            }
        }
    }

    // Sort order.
    private string _sortOrder = "asc";
    public string SortOrder
    {
        get => _sortOrder;
        set
        {
            if (SetProperty(ref _sortOrder, value))
            {
                CurrentPage = 1;
                _ = LoadStaffAsync();
            }
        }
    }

    // Available properties for sorting.
    public ObservableCollection<string> SortableProperties { get; }
    // Available sort orders.
    public ObservableCollection<string> SortOrders { get; }

    // Current page number.
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
    // Display string for current page.
    public string CurrentPageDisplay => $"{CurrentPage}";

    // Items per page.
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
                _ = LoadStaffAsync();
            }
        }
    }

    // Total number of items.
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
    // Total number of pages.
    public int TotalPages => (TotalItems == 0 || Limit <= 0) ? 1 : (int)Math.Ceiling((double)TotalItems / Limit);
    // Display string for total pages.
    public string TotalPagesDisplay => $"{TotalPages}";

    // Can navigate to previous page.
    public bool CanGoToPreviousPage => CurrentPage > 1 && !IsLoading;
    // Can navigate to next page.
    public bool CanGoToNextPage => CurrentPage < TotalPages && !IsLoading;

    // Commands.
    public ICommand AddStaffCommand { get; }
    public ICommand EditStaffCommand { get; }
    public ICommand DeleteStaffCommand { get; }
    public ICommand RestoreStaffCommand { get; }
    public ICommand AssignTaskCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand FirstPageCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand LastPageCommand { get; }

    // Design-time constructor.
    public StaffManagementViewModel()
    {
        _userService = null!;
        _serviceProvider = null!;
        _toastViewModel = new ToastNotificationViewModel();
        SortableProperties = new ObservableCollection<string> { "FullName", "Username", "Email", "Role", "CreatedAt", };
        SortOrders = new ObservableCollection<string> { "asc", "desc" };
        SortBy = "CreatedAt";
        StaffList = new ObservableCollection<User> {
            new User(Guid.NewGuid(), "d.admin", "a@d.c", UserRole.Admin, "D Admin"),
            new User(Guid.NewGuid(), "d.staff", "s@d.c", UserRole.Staff, "D Staff", isActive: false)
        };
        TotalItems = 2; CurrentPage = 1; Limit = 10; IsLoading = false;
        AddStaffCommand = new RelayCommand(_ => { }, _ => false);
        EditStaffCommand = new RelayCommand<User>(_ => { }, _ => false);
        DeleteStaffCommand = new RelayCommand<User>(_ => { }, _ => false);
        RestoreStaffCommand = new RelayCommand<User>(_ => { }, _ => false);
        AssignTaskCommand = new RelayCommand<User>(_ => { }, _ => false);
        SearchCommand = new RelayCommand(_ => { }, _ => false);
        RefreshCommand = new RelayCommand(_ => { }, _ => false);
        FirstPageCommand = new RelayCommand(_ => { }, _ => false);
        PreviousPageCommand = new RelayCommand(_ => { }, _ => false);
        NextPageCommand = new RelayCommand(_ => { }, _ => false);
        LastPageCommand = new RelayCommand(_ => { }, _ => false);
        UpdatePaginationCommandsCanExecute();
    }

    // Runtime constructor.
    public StaffManagementViewModel(IUserService userService, IServiceProvider serviceProvider, IAuthenticationService authenticationService, ToastNotificationViewModel toastViewModel)
    {
        _userService = userService;
        _serviceProvider = serviceProvider;
        _authenticationService = authenticationService;
        _toastViewModel = toastViewModel;
        SortableProperties = new ObservableCollection<string> { "FullName", "Username", "Email", "Role", "CreatedAt", };
        SortOrders = new ObservableCollection<string> { "asc", "desc" };
        _sortBy = "CreatedAt"; _sortOrder = "asc";
        AddStaffCommand = new RelayCommand(async _ => await OpenAddEditStaffDialog(null), _ => !IsLoading);
        EditStaffCommand = new RelayCommand<User>(async (user) => await OpenAddEditStaffDialog(user), (user) => user != null && !IsLoading);
        DeleteStaffCommand = new RelayCommand<User>(async (user) => await DeleteStaffAsync(user), (user) => user != null && user.IsActive && !IsLoading);
        RestoreStaffCommand = new RelayCommand<User>(async (user) => await RestoreStaffAsync(user), (user) => user != null && !user.IsActive && !IsLoading);
        AssignTaskCommand = new RelayCommand<User>(async (user) => await OpenAssignTaskDialog(user), (user) => user != null && user.IsActive && !IsLoading);
        SearchCommand = new RelayCommand(async _ => { CurrentPage = 1; await LoadStaffAsync(); }, _ => !IsLoading);
        RefreshCommand = new RelayCommand(async _ => await LoadStaffAsync(), _ => !IsLoading);
        FirstPageCommand = new RelayCommand(async _ => { if (CurrentPage != 1) { CurrentPage = 1; await LoadStaffAsync(); } }, _ => CanGoToPreviousPage);
        PreviousPageCommand = new RelayCommand(async _ => { if (CanGoToPreviousPage) { CurrentPage--; await LoadStaffAsync(); } }, _ => CanGoToPreviousPage);
        NextPageCommand = new RelayCommand(async _ => { if (CanGoToNextPage) { CurrentPage++; await LoadStaffAsync(); } }, _ => CanGoToNextPage);
        LastPageCommand = new RelayCommand(async _ => { if (CurrentPage != TotalPages && TotalPages > 0) { CurrentPage = TotalPages; await LoadStaffAsync(); } }, _ => CanGoToNextPage && CurrentPage != TotalPages);
        _ = LoadStaffAsync();
    }

    // Updates CanExecute for pagination commands.
    private void UpdatePaginationCommandsCanExecute()
    {
        OnPropertyChanged(nameof(CanGoToPreviousPage));
        OnPropertyChanged(nameof(CanGoToNextPage));
        (FirstPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (LastPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    // Loads staff list from service.
    public async Task LoadStaffAsync()
    {
        if (_userService == null) return;
        if (IsLoading) return;
        IsLoading = true;
        try
        {
            if (Limit <= 0) _limit = 10;
            int apiSkipParameter = CurrentPage - 1; // API parameter for skip.
            var paginatedResult = await _userService.GetUsersAsync(apiSkipParameter, Limit, SortBy, SortOrder, SearchTerm, includeInactive: true);
            if (paginatedResult?.PaginatedData != null)
            {
                StaffList = new ObservableCollection<User>(paginatedResult.PaginatedData);
                if (paginatedResult.Metadata != null)
                {
                    TotalItems = paginatedResult.Metadata.TotalRow;
                    if (CurrentPage > TotalPages && TotalPages > 0) CurrentPage = TotalPages;
                    else if (TotalPages == 0 && TotalItems == 0) CurrentPage = 1;
                }
                else { TotalItems = StaffList.Count; }
            }
            else
            {
                StaffList.Clear(); TotalItems = 0; CurrentPage = 1;
                _toastViewModel?.Show("No staff found.", ToastType.Warning);
            }
        }
        catch (Exception ex)
        {
            _toastViewModel?.Show($"Error loading staff: {ex.Message}", ToastType.Error);
            StaffList.Clear(); TotalItems = 0;
        }
        finally { IsLoading = false; }
    }

    // Opens dialog to assign task to staff.
    private async Task OpenAssignTaskDialog(User? staffToAssign)
    {
        if (staffToAssign == null || _serviceProvider == null || _toastViewModel == null)
        {
            _toastViewModel?.Show("Cannot assign task: Invalid staff or missing services.", ToastType.Error);
            return;
        }
        var addEditTaskViewModel = _serviceProvider.GetRequiredService<AddEditTaskViewModel>();
        await addEditTaskViewModel.LoadAssignableUsersAsync();
        addEditTaskViewModel.InitializeForAdd();
        addEditTaskViewModel.SelectedAssignee = staffToAssign;

        var dialogView = new AddEditTaskDialog { DataContext = addEditTaskViewModel };
        var dialogWindow = new Window
        {
            Title = "Assign New Task to " + staffToAssign.FullName,
            Content = dialogView,
            Width = 520,
            Height = 750, // Adjusted height
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Application.Current.MainWindow,
            ResizeMode = ResizeMode.NoResize,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.ToolWindow
        };
        addEditTaskViewModel.CloseActionWithResult = (success) =>
        {
            dialogWindow.DialogResult = success; dialogWindow.Close();
            if (success) { _toastViewModel.Show($"Task assigned to {staffToAssign.FullName} successfully.", ToastType.Success); }
        };
        dialogWindow.ShowDialog();
    }

    // Opens dialog to add or edit staff.
    private async Task OpenAddEditStaffDialog(User? userToEdit)
    {
        await Task.Delay(1); // UI responsiveness.
        if (_serviceProvider == null || _toastViewModel == null)
        {
            _toastViewModel?.Show("Services unavailable.", ToastType.Error);
            return;
        }
        var addEditUserVM = _serviceProvider.GetRequiredService<AddEditUserViewModel>();
        if (userToEdit == null) addEditUserVM.InitializeForAdd();
        else addEditUserVM.InitializeForEdit(userToEdit);

        var dialogView = new AddEditUserDialog { DataContext = addEditUserVM };
        var dialogWindow = new Window
        {
            Title = addEditUserVM.WindowTitle,
            Content = dialogView,
            Width = 500,
            Height = 680,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Application.Current.MainWindow,
            ResizeMode = ResizeMode.NoResize,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.ToolWindow
        };
        addEditUserVM.CloseActionWithResult = (success) => {
            dialogWindow.DialogResult = success; dialogWindow.Close();
            if (success) _ = LoadStaffAsync(); // Refresh list.
        };
        dialogWindow.ShowDialog();
    }

    // Marks staff as inactive.
    private async Task DeleteStaffAsync(User? staffToDelete)
    {
        if (staffToDelete == null || _userService == null || _toastViewModel == null) return;
        IsLoading = true; bool success = false;
        try
        {
            success = await _userService.DeleteUserAsync(staffToDelete.Id);
            if (success)
            {
                _toastViewModel.Show($"Staff '{staffToDelete.FullName}' marked inactive.", ToastType.Success);
                staffToDelete.IsActive = false; // Optimistic update.
                (DeleteStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged();
                (RestoreStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged();
                await LoadStaffAsync(); // Refresh list.
            }
            else { _toastViewModel.Show($"Failed to mark staff '{staffToDelete.FullName}' inactive.", ToastType.Error); }
        }
        catch (Exception ex) { _toastViewModel.Show($"Error deleting staff: {ex.Message}", ToastType.Error); }
        finally { IsLoading = false; }
    }

    // Restores inactive staff.
    private async Task RestoreStaffAsync(User? staffToRestore)
    {
        if (staffToRestore == null || _userService == null || _toastViewModel == null) return;
        IsLoading = true; bool success = false;
        try
        {
            success = await _userService.RestoreUserAsync(staffToRestore.Id);
            if (success)
            {
                _toastViewModel.Show($"Staff '{staffToRestore.FullName}' restored.", ToastType.Success);
                staffToRestore.IsActive = true; // Optimistic update.
                (DeleteStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged();
                (RestoreStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged();
                await LoadStaffAsync(); // Refresh list.
            }
            else { _toastViewModel.Show($"Failed to restore staff '{staffToRestore.FullName}'.", ToastType.Error); }
        }
        catch (Exception ex) { _toastViewModel.Show($"Error restoring staff: {ex.Message}", ToastType.Error); }
        finally { IsLoading = false; }
    }
}