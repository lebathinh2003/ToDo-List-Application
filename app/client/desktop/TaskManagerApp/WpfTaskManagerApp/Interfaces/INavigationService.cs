using WpfTaskManagerApp.ViewModels.Common;
namespace WpfTaskManagerApp.Interfaces;

// Handles view navigation.
public interface INavigationService
{
    // Current active view model.
    ViewModelBase? CurrentView { get; }

    // Navigates to a specified view model type.
    void NavigateTo<T>() where T : ViewModelBase;

    // Event fired when current view changes.
    event System.Action? CurrentViewChanged;
}