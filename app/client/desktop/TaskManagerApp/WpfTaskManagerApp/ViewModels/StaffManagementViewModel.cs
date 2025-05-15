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
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (SetProperty(ref _isLoading, value))
            {
                (AddStaffCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (EditStaffCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteStaffCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (SearchCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (RefreshCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    private ObservableCollection<User> _staffList = new(); 
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
                (EditStaffCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteStaffCommand as RelayCommand)?.RaiseCanExecuteChanged();
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

    public ICommand AddStaffCommand { get; }
    public ICommand EditStaffCommand { get; }
    public ICommand DeleteStaffCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand RefreshCommand { get; }

    public StaffManagementViewModel()
    {
        _userService = null;
        _serviceProvider = null;
        StaffList = new ObservableCollection<User>
            {
                new User(Guid.NewGuid(), "design.admin", "admin@design.com", UserRole.Admin, "Design Admin"),
                new User(Guid.NewGuid(), "design.staff", "staff@design.com", UserRole.Staff, "Design Staff")
            };
        TotalItems = StaffList.Count;
        IsLoading = false;

        AddStaffCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        EditStaffCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        DeleteStaffCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        SearchCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        NextPageCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        PreviousPageCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
        RefreshCommand = new RelayCommand(async _ => await Task.CompletedTask, _ => false);
    }


    public StaffManagementViewModel(IUserService userService, IServiceProvider serviceProvider)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        AddStaffCommand = new RelayCommand(async param => await OpenAddStaffDialog(param), param => !IsLoading);
        EditStaffCommand = new RelayCommand(async param => await OpenEditStaffDialog(param), param => SelectedStaff != null && !IsLoading);
        DeleteStaffCommand = new RelayCommand(async _ => await DeleteStaffAsync(), _ => SelectedStaff != null && SelectedStaff.IsActive && !IsLoading);

        SearchCommand = new RelayCommand(async _ => {
            _currentPage = 1;
            OnPropertyChanged(nameof(CurrentPage));
            await LoadStaffAsync();
        }, _ => !IsLoading);

        NextPageCommand = new RelayCommand(async _ => {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await LoadStaffAsync();
            }
        }, _ => CurrentPage < TotalPages && !IsLoading);

        PreviousPageCommand = new RelayCommand(async _ => {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadStaffAsync();
            }
        }, _ => CurrentPage > 1 && !IsLoading);

        RefreshCommand = new RelayCommand(async _ => await LoadStaffAsync(), _ => !IsLoading);

        _ = LoadStaffAsync();
    }

    private async void OnSearchTermChanged()
    {
        if (_userService == null) return;
        try
        {
            _currentPage = 1;
            OnPropertyChanged(nameof(CurrentPage));
            await LoadStaffAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in OnSearchTermChanged (Staff): {ex}");
        }
    }

    private async void OnPageSizeChanged()
    {
        if (_userService == null) return;
        try
        {
            await LoadStaffAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in OnPageSizeChanged (Staff): {ex}");
        }
    }

    public async Task LoadStaffAsync()
    {
        if (_userService == null) return;
        if (IsLoading) return;
        IsLoading = true;
        try
        {
            if (PageSize <= 0) _pageSize = 10;

            var allUsers = await _userService.GetUsersAsync(SearchTerm, includeInactive: true);

            TotalItems = allUsers.Count();

            var pagedUsers = allUsers
                                .OrderBy(u => u.FullName)
                                .Skip((CurrentPage - 1) * PageSize)
                                .Take(PageSize);

            StaffList = new ObservableCollection<User>(pagedUsers);
            SelectedStaff = null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load staff: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task OpenAddStaffDialog(object? parameter)
    {
        if (_serviceProvider == null || _userService == null) return;

        var addEditUserViewModel = _serviceProvider.GetRequiredService<AddEditUserViewModel>();
        addEditUserViewModel.InitializeForAdd();

        addEditUserViewModel.CloseActionWithResult = async (success) =>
        {
            // Debug.WriteLine($"AddEditUserViewModel CloseActionWithResult called with: {success} from StaffManagementViewModel");
            if (success)
            {
                await LoadStaffAsync();
            }
        };

        // Debug.WriteLine("Simulating showing Add Staff Dialog...");
        await Task.Delay(1);
        // Debug.WriteLine("Add Staff Dialog simulation finished.");
    }

    private async Task OpenEditStaffDialog(object? parameter)
    {
        if (SelectedStaff == null || _serviceProvider == null || _userService == null) return;

        var addEditUserViewModel = _serviceProvider.GetRequiredService<AddEditUserViewModel>();
        addEditUserViewModel.InitializeForEdit(SelectedStaff); // SelectedStaff.Id giờ là Guid

        addEditUserViewModel.CloseActionWithResult = async (success) =>
        {
            // Debug.WriteLine($"AddEditUserViewModel CloseActionWithResult called with: {success} from StaffManagementViewModel");
            if (success)
            {
                await LoadStaffAsync();
            }
        };

        // Debug.WriteLine($"Simulating showing Edit Staff Dialog for {SelectedStaff.Username}...");
        await Task.Delay(1);
        // Debug.WriteLine("Edit Staff Dialog simulation finished.");
    }

    private async Task DeleteStaffAsync()
    {
        if (SelectedStaff == null || _userService == null) return; // SelectedStaff.Id giờ là Guid

        try
        {
            bool success = await _userService.DeleteUserAsync(SelectedStaff.Id);
            if (success)
            {
                await LoadStaffAsync();
            }
            else
            {
                // Debug.WriteLine($"Soft delete failed for staff {SelectedStaff.Username}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting staff: {ex.Message}");
        }
    }
}