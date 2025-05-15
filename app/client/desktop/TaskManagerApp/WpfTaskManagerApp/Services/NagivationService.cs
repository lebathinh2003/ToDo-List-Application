namespace WpfTaskManagerApp.Services;
using WpfTaskManagerApp.ViewModels.Common;
public class NavigationService : INavigationService
{
    private readonly System.Func<System.Type, ViewModelBase> _viewModelFactory;
    private ViewModelBase? _currentView;

    public ViewModelBase? CurrentView
    {
        get => _currentView;
        private set
        {
            _currentView = value;
            CurrentViewChanged?.Invoke();
        }
    }
    public event System.Action? CurrentViewChanged;
    public NavigationService(System.Func<System.Type, ViewModelBase> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }
    public void NavigateTo<T>() where T : ViewModelBase
    {
        CurrentView = _viewModelFactory.Invoke(typeof(T));
    }
}