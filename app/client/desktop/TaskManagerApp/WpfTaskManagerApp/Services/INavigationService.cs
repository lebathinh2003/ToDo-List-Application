using WpfTaskManagerApp.ViewModels.Common;
public interface INavigationService
{
    ViewModelBase? CurrentView { get; }
    void NavigateTo<T>() where T : ViewModelBase;
    event System.Action? CurrentViewChanged;
}
