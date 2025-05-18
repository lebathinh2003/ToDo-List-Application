using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.Services;
using WpfTaskManagerApp.ViewModels.Common;
using WpfTaskManagerApp.Views;
namespace WpfTaskManagerApp.ViewModels;
public class StaffManagementViewModel : ViewModelBase
{
    private readonly IUserService? _userService;
    private readonly IServiceProvider? _serviceProvider;
    private readonly ToastNotificationViewModel? _toastViewModel;
    private bool _isLoading;

    public bool IsLoading { get => _isLoading; set { if (SetProperty(ref _isLoading, value)) { (AddStaffCommand as RelayCommand)?.RaiseCanExecuteChanged(); (EditStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged(); (DeleteStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged(); (RestoreStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged(); (SearchCommand as RelayCommand)?.RaiseCanExecuteChanged(); (RefreshCommand as RelayCommand)?.RaiseCanExecuteChanged(); UpdatePaginationCommandsCanExecute(); } } }
    private ObservableCollection<User> _staffList = new ObservableCollection<User>();
    public ObservableCollection<User> StaffList { get => _staffList; set => SetProperty(ref _staffList, value); }
    private User? _selectedStaff;
    public User? SelectedStaff { get => _selectedStaff; set { if (SetProperty(ref _selectedStaff, value)) { (EditStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged(); (DeleteStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged(); (RestoreStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged(); } } }
    private string _searchTerm = string.Empty;
    public string SearchTerm { get => _searchTerm; set => SetProperty(ref _searchTerm, value); }
    private string? _sortBy;
    public string? SortBy { get => _sortBy; set { if (SetProperty(ref _sortBy, value)) { CurrentPage = 1; _ = LoadStaffAsync(); } } }
    private string _sortOrder = "asc";
    public string SortOrder { get => _sortOrder; set { if (SetProperty(ref _sortOrder, value)) { CurrentPage = 1; _ = LoadStaffAsync(); } } }
    public ObservableCollection<string> SortableProperties { get; }
    public ObservableCollection<string> SortOrders { get; }
    private int _currentPage = 1;
    public int CurrentPage { get => _currentPage; set { if (value < 1) value = 1; if (SetProperty(ref _currentPage, value)) { OnPropertyChanged(nameof(CurrentPageDisplay)); UpdatePaginationCommandsCanExecute(); } } }
    public string CurrentPageDisplay => $"{CurrentPage}";
    private int _limit = 10;
    public int Limit { get => _limit; set { if (value < 1) value = 1; if (SetProperty(ref _limit, value)) { CurrentPage = 1; _ = LoadStaffAsync(); } } }
    private int _totalItems;
    public int TotalItems { get => _totalItems; private set { if (SetProperty(ref _totalItems, value)) { OnPropertyChanged(nameof(TotalPages)); OnPropertyChanged(nameof(TotalPagesDisplay)); UpdatePaginationCommandsCanExecute(); } } }
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

    public StaffManagementViewModel()
    {
        _userService = null!; _serviceProvider = null!; _toastViewModel = new ToastNotificationViewModel();
        SortableProperties = new ObservableCollection<string> { "FullName", "Username", "Email", "Role" };
        SortOrders = new ObservableCollection<string> { "asc", "desc" }; SortBy = "FullName";
        StaffList = new ObservableCollection<User> { new User(Guid.NewGuid(), "d.admin", "a@d.c", UserRole.Admin, "D Admin"), new User(Guid.NewGuid(), "d.staff", "s@d.c", UserRole.Staff, "D Staff", isActive: false) };
        TotalItems = 2; CurrentPage = 1; Limit = 10; IsLoading = false;
        AddStaffCommand = new RelayCommand(async _ => { }, _ => false); EditStaffCommand = new RelayCommand<User>(async _ => { }, _ => false); DeleteStaffCommand = new RelayCommand<User>(async _ => { }, _ => false); RestoreStaffCommand = new RelayCommand<User>(async _ => { }, _ => false);
        SearchCommand = new RelayCommand(async _ => { }, _ => false); RefreshCommand = new RelayCommand(async _ => { }, _ => false);
        FirstPageCommand = new RelayCommand(async _ => { }, _ => false); PreviousPageCommand = new RelayCommand(async _ => { }, _ => false); NextPageCommand = new RelayCommand(async _ => { }, _ => false); LastPageCommand = new RelayCommand(async _ => { }, _ => false);
        UpdatePaginationCommandsCanExecute();
    }
    public StaffManagementViewModel(IUserService userService, IServiceProvider serviceProvider, ToastNotificationViewModel toastViewModel)
    {
        _userService = userService; _serviceProvider = serviceProvider; _toastViewModel = toastViewModel;
        SortableProperties = new ObservableCollection<string> { "FullName", "Username", "Email", "Role" };
        SortOrders = new ObservableCollection<string> { "asc", "desc" }; _sortBy = "FullName"; _sortOrder = "asc";
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
    private void UpdatePaginationCommandsCanExecute() { OnPropertyChanged(nameof(CanGoToPreviousPage)); OnPropertyChanged(nameof(CanGoToNextPage)); (FirstPageCommand as RelayCommand)?.RaiseCanExecuteChanged(); (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged(); (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged(); (LastPageCommand as RelayCommand)?.RaiseCanExecuteChanged(); }
    public async Task LoadStaffAsync()
    {
        if (_userService == null) return; if (IsLoading) return; IsLoading = true;
        try
        {
            if (Limit <= 0) _limit = 10; int apiSkipParameter = CurrentPage - 1;
            var paginatedResult = await _userService.GetUsersAsync(apiSkipParameter, Limit, SortBy, SortOrder, SearchTerm, includeInactive: true);
            if (paginatedResult?.PaginatedData != null)
            {
                StaffList = new ObservableCollection<User>(paginatedResult.PaginatedData);
                if (paginatedResult.Metadata != null) { TotalItems = paginatedResult.Metadata.TotalRow; if (CurrentPage > TotalPages && TotalPages > 0) CurrentPage = TotalPages; else if (TotalPages == 0 && TotalItems == 0) CurrentPage = 1; }
                else { TotalItems = StaffList.Count; }
            }
            else { StaffList.Clear(); TotalItems = 0; CurrentPage = 1; _toastViewModel?.Show("No staff found.", ToastType.Warning); }
        }
        catch (Exception ex) { _toastViewModel?.Show($"Error loading staff: {ex.Message}", ToastType.Error); StaffList.Clear(); TotalItems = 0; }
        finally { IsLoading = false; }
    }
    private async Task OpenAddEditStaffDialog(User? userToEdit)
    {
        if (_serviceProvider == null || _toastViewModel == null) { _toastViewModel?.Show("Services unavailable.", ToastType.Error); return; }
        var addEditUserVM = _serviceProvider.GetRequiredService<AddEditUserViewModel>();
        if (userToEdit == null) addEditUserVM.InitializeForAdd(); else addEditUserVM.InitializeForEdit(userToEdit);
        var dialogView = new AddEditUserDialog { DataContext = addEditUserVM };
        var dialogWindow = new Window { Title = addEditUserVM.WindowTitle, Content = dialogView, Width = 500, Height = 680, WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = Application.Current.MainWindow, ResizeMode = ResizeMode.NoResize, ShowInTaskbar = false, WindowStyle = WindowStyle.ToolWindow };
        addEditUserVM.CloseActionWithResult = (success) => { dialogWindow.DialogResult = success; dialogWindow.Close(); if (success) _ = LoadStaffAsync(); };
        dialogWindow.ShowDialog();
    }
    private async Task DeleteStaffAsync(User? staffToDelete)
    {
        if (staffToDelete == null || _userService == null || _toastViewModel == null) return;
        IsLoading = true; bool success = false;
        try
        {
            success = await _userService.DeleteUserAsync(staffToDelete.Id);
            if (success) {
                _toastViewModel.Show($"Staff '{staffToDelete.FullName}' marked inactive.", ToastType.Success);
                staffToDelete.IsActive = false;
                (DeleteStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged(); 
                (RestoreStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged(); 
                await LoadStaffAsync();
            }
            else { _toastViewModel.Show($"Failed to mark staff '{staffToDelete.FullName}' inactive.", ToastType.Error); }
        }
        catch (Exception ex) { _toastViewModel.Show($"Error deleting staff: {ex.Message}", ToastType.Error); }
        finally { IsLoading = false; }
    }
    private async Task RestoreStaffAsync(User? staffToRestore)
    {
        if (staffToRestore == null || _userService == null || _toastViewModel == null) return;
        IsLoading = true; bool success = false;
        try
        {
            success = await _userService.RestoreUserAsync(staffToRestore.Id);
            if (success) { _toastViewModel.Show($"Staff '{staffToRestore.FullName}' restored.", ToastType.Success); staffToRestore.IsActive = true; (DeleteStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged(); (RestoreStaffCommand as RelayCommand<User>)?.RaiseCanExecuteChanged(); await LoadStaffAsync(); }
            else { _toastViewModel.Show($"Failed to restore staff '{staffToRestore.FullName}'.", ToastType.Error); }
        }
        catch (Exception ex) { _toastViewModel.Show($"Error restoring staff: {ex.Message}", ToastType.Error); }
        finally { IsLoading = false; }
    }
}
