using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.Services;
using WpfTaskManagerApp.ViewModels.Common;
namespace WpfTaskManagerApp.ViewModels;
public class StaffManagementViewModel : ViewModelBase
{
    private readonly IUserService? _userService;
    private readonly IServiceProvider? _serviceProvider;
    private readonly ToastNotificationViewModel? _toastViewModel;
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
                (SearchCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (RefreshCommand as RelayCommand)?.RaiseCanExecuteChanged();
                UpdatePaginationCommandsCanExecute();
            }
        }
    }

    private ObservableCollection<User> _staffList = new ObservableCollection<User>();
    public ObservableCollection<User> StaffList
    {
        get => _staffList;
        set => SetProperty(ref _staffList, value);
    }

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
            }
        }
    }

    private string _searchTerm = string.Empty;
    public string SearchTerm
    {
        get => _searchTerm;
        set => SetProperty(ref _searchTerm, value);
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
                _ = LoadStaffAsync();
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
                _ = LoadStaffAsync();
            }
        }
    }
    public ObservableCollection<string> SortableProperties { get; }
    public ObservableCollection<string> SortOrders { get; }

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
                _ = LoadStaffAsync();
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

    public ICommand AddStaffCommand { get; }
    public ICommand EditStaffCommand { get; }
    public ICommand DeleteStaffCommand { get; }
    public ICommand RestoreStaffCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand FirstPageCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand LastPageCommand { get; }

    public StaffManagementViewModel() // Constructor cho Design-Time
    {
        _userService = null!;
        _serviceProvider = null!;
        _toastViewModel = new ToastNotificationViewModel();

        SortableProperties = new ObservableCollection<string> { "FullName", "Username", "Email", "Role" };
        SortOrders = new ObservableCollection<string> { "asc", "desc" };
        SortBy = "FullName";

        StaffList = new ObservableCollection<User>
            {
                new User(Guid.NewGuid(), "design.admin", "admin@design.com", UserRole.Admin, "Design Admin", "123 Design St", true),
                new User(Guid.NewGuid(), "design.staff1", "staff1@design.com", UserRole.Staff, "Design Staff One", "456 Design Ave", true),
                new User(Guid.NewGuid(), "design.staff2.inactive", "staff2@design.com", UserRole.Staff, "Design Staff Two", "789 Design Rd", false)
            };
        TotalItems = 3; CurrentPage = 1; Limit = 2; IsLoading = false;

        AddStaffCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        EditStaffCommand = new RelayCommand<User>(async _ => await Task.CompletedTask, _ => false);
        DeleteStaffCommand = new RelayCommand<User>(async _ => await Task.CompletedTask, _ => false);
        RestoreStaffCommand = new RelayCommand<User>(async _ => await Task.CompletedTask, _ => false);
        SearchCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        RefreshCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        FirstPageCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        PreviousPageCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        NextPageCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        LastPageCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        UpdatePaginationCommandsCanExecute();
    }

    public StaffManagementViewModel(IUserService userService, IServiceProvider serviceProvider, ToastNotificationViewModel toastViewModel)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _toastViewModel = toastViewModel ?? throw new ArgumentNullException(nameof(toastViewModel));

        SortableProperties = new ObservableCollection<string> { "FullName", "Username", "Email", "Role" };
        SortOrders = new ObservableCollection<string> { "asc", "desc" };
        _sortBy = "FullName";
        _sortOrder = "asc";

        AddStaffCommand = new RelayCommand(async _ => await OpenAddEditStaffDialog(null), _ => !IsLoading);
        EditStaffCommand = new RelayCommand<User>(async (user) => await OpenAddEditStaffDialog(user), (user) => user != null && !IsLoading);
        DeleteStaffCommand = new RelayCommand<User>(async (user) => await DeleteStaffAsync(user), (user) => user != null && user.IsActive && !IsLoading);
        RestoreStaffCommand = new RelayCommand<User>(async (user) => await RestoreStaffAsync(user), (user) => user != null && !user.IsActive && !IsLoading);

        SearchCommand = new RelayCommand(async _ => { CurrentPage = 1; await LoadStaffAsync(); }, _ => !IsLoading);
        RefreshCommand = new RelayCommand(async _ => await LoadStaffAsync(), _ => !IsLoading);

        FirstPageCommand = new RelayCommand(async _ => { if (CurrentPage != 1) { CurrentPage = 1; await LoadStaffAsync(); } }, _ => CanGoToPreviousPage);
        PreviousPageCommand = new RelayCommand(async _ => { if (CanGoToPreviousPage) { CurrentPage--; await LoadStaffAsync(); } }, _ => CanGoToPreviousPage);
        NextPageCommand = new RelayCommand(async _ => { if (CanGoToNextPage) { CurrentPage++; await LoadStaffAsync(); } }, _ => CanGoToNextPage);
        LastPageCommand = new RelayCommand(async _ => { if (CurrentPage != TotalPages && TotalPages > 0) { CurrentPage = TotalPages; await LoadStaffAsync(); } }, _ => CanGoToNextPage && CurrentPage != TotalPages);

        _ = LoadStaffAsync();
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

    public async Task LoadStaffAsync()
    {
        if (_userService == null) { Debug.WriteLine("StaffManagementViewModel: UserService is null. Cannot load staff."); return; }
        if (IsLoading) return;
        IsLoading = true;
        SelectedStaff = null;
        Debug.WriteLine($"StaffManagementViewModel.LoadStaffAsync: Loading staff. Page: {CurrentPage}, Limit: {Limit}, SortBy: {SortBy}, SortOrder: {SortOrder}, Keyword: '{SearchTerm}'");
        try
        {
            if (Limit <= 0) _limit = 10;
            int skip = (CurrentPage - 1);

            var paginatedResult = await _userService.GetUsersAsync(skip, Limit, SortBy, SortOrder, SearchTerm, includeInactive: true);

            if (paginatedResult?.PaginatedData != null)
            {
                StaffList = new ObservableCollection<User>(paginatedResult.PaginatedData);
                if (paginatedResult.Metadata != null)
                {
                    TotalItems = paginatedResult.Metadata.TotalRow;
                    Debug.WriteLine($"StaffManagementViewModel.LoadStaffAsync: Loaded {StaffList.Count} users. TotalItems from API: {TotalItems}. Calculated TotalPages: {TotalPages}");
                    Debug.WriteLine($"StaffManagementViewModel.LoadStaffAsync: skip {skip}, limit {Limit}, sortBy {SortBy}, sortOrder {SortOrder}, keyword {SearchTerm}"); 
                    if (CurrentPage > TotalPages && TotalPages > 0) { CurrentPage = TotalPages; }
                    else if (TotalPages == 0 && TotalItems == 0) { CurrentPage = 1; }
                }
                else { TotalItems = StaffList.Count; }

                // ***** LOG ISACTIVE STATUS OF LOADED USERS *****
                // foreach (var user in StaffList)
                // {
                //     Debug.WriteLine($"StaffManagementViewModel.LoadStaffAsync: User: {user.Username}, IsActive: {user.IsActive}");
                // }
                // ***** END LOG *****
            }
            else
            {
                StaffList.Clear(); TotalItems = 0; CurrentPage = 1;
                _toastViewModel?.Show("Could not load staff data or no staff found.", ToastType.Warning);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load staff: {ex.Message}");
            _toastViewModel?.Show($"Error loading staff: {ex.Message}", ToastType.Error);
            StaffList.Clear(); TotalItems = 0;
        }
        finally { IsLoading = false; }
    }

    private async Task OpenAddEditStaffDialog(User? userToEdit) { /* ... Giữ nguyên ... */ }

    private async Task DeleteStaffAsync(User? staffToDelete)
    {
        if (staffToDelete == null || _userService == null || _toastViewModel == null) return;
        Debug.WriteLine($"StaffManagementViewModel.DeleteStaffAsync: Attempting to delete user '{staffToDelete.FullName}' (ID: {staffToDelete.Id}, IsActive: {staffToDelete.IsActive})");

        IsLoading = true;
        try
        {
            bool success = await _userService.DeleteUserAsync(staffToDelete.Id);
            if (success)
            {
                _toastViewModel.Show($"Staff '{staffToDelete.FullName}' marked as inactive.", ToastType.Success);
                Debug.WriteLine($"StaffManagementViewModel.DeleteStaffAsync: API call successful for user '{staffToDelete.FullName}'. Reloading staff list...");
                await LoadStaffAsync(); // Tải lại danh sách
            }
            else
            {
                _toastViewModel.Show($"Failed to mark staff '{staffToDelete.FullName}' as inactive.", ToastType.Error);
                Debug.WriteLine($"StaffManagementViewModel.DeleteStaffAsync: API call FAILED for user '{staffToDelete.FullName}'.");
            }
        }
        catch (Exception ex) { _toastViewModel.Show($"Error deleting staff: {ex.Message}", ToastType.Error); Debug.WriteLine($"StaffManagementViewModel.DeleteStaffAsync: Exception for user '{staffToDelete.FullName}': {ex.Message}"); }
        finally { IsLoading = false; }
    }

    private async Task RestoreStaffAsync(User? staffToRestore)
    {
        if (staffToRestore == null || _userService == null || _toastViewModel == null) return;
        Debug.WriteLine($"StaffManagementViewModel.RestoreStaffAsync: Attempting to restore user '{staffToRestore.FullName}' (ID: {staffToRestore.Id}, IsActive: {staffToRestore.IsActive})");

        IsLoading = true;
        try
        {
            bool success = await _userService.RestoreUserAsync(staffToRestore.Id);
            if (success)
            {
                _toastViewModel.Show($"Staff '{staffToRestore.FullName}' restored successfully.", ToastType.Success);
                Debug.WriteLine($"StaffManagementViewModel.RestoreStaffAsync: API call successful for user '{staffToRestore.FullName}'. Reloading staff list...");
                await LoadStaffAsync(); // Tải lại danh sách
            }
            else
            {
                _toastViewModel.Show($"Failed to restore staff '{staffToRestore.FullName}'.", ToastType.Error);
                Debug.WriteLine($"StaffManagementViewModel.RestoreStaffAsync: API call FAILED for user '{staffToRestore.FullName}'.");
            }
        }
        catch (Exception ex) { _toastViewModel.Show($"Error restoring staff: {ex.Message}", ToastType.Error); Debug.WriteLine($"StaffManagementViewModel.RestoreStaffAsync: Exception for user '{staffToRestore.FullName}': {ex.Message}"); }
        finally { IsLoading = false; }
    }
}