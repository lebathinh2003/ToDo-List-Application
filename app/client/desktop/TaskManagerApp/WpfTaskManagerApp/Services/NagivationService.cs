using WpfTaskManagerApp.Interfaces;
using WpfTaskManagerApp.ViewModels.Common;
namespace WpfTaskManagerApp.Services;

// Service for handling view model navigation.
public class NavigationService : INavigationService
{
    // Factory to create view model instances.
    private readonly Func<Type, ViewModelBase> _viewModelFactory;
    private ViewModelBase? _currentView;

    // Gets the current active view model.
    public ViewModelBase? CurrentView
    {
        get => _currentView;
        private set
        {
            _currentView = value;
            CurrentViewChanged?.Invoke(); // Notify listeners.
        }
    }

    // Event fired when the current view changes.
    public event Action? CurrentViewChanged;

    // Constructor.
    public NavigationService(Func<Type, ViewModelBase> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    // Navigates to the specified view model type.
    public void NavigateTo<T>() where T : ViewModelBase
    {
        CurrentView = _viewModelFactory.Invoke(typeof(T));
    }
}