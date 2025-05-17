using WpfTaskManagerApp.ViewModels.Common;
// Navigation Service
public interface INavigationService
{
    ViewModelBase? CurrentView { get; }
    void NavigateTo<T>() where T : ViewModelBase;
    event System.Action? CurrentViewChanged;
}
