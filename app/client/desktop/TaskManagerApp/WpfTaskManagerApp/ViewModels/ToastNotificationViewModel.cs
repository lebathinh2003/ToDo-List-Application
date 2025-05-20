using System.Windows.Input;
using System.Windows.Threading; // For DispatcherTimer
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.ViewModels.Common;
using WpfTaskManagerApp.Models; // For TaskItem

namespace WpfTaskManagerApp.ViewModels;

// ViewModel for toast notifications.
public class ToastNotificationViewModel : ViewModelBase
{
    private string _message = string.Empty;
    // Notification message.
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    private bool _isVisible;
    // Controls visibility of the toast.
    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    private ToastType _toastType;
    // Type of toast (e.g., Success, Error).
    public ToastType ToastType
    {
        get => _toastType;
        set => SetProperty(ref _toastType, value);
    }

    private TaskItem? _associatedTask;
    // Optional task associated with the notification.
    public TaskItem? AssociatedTask
    {
        get => _associatedTask;
        private set => SetProperty(ref _associatedTask, value);
    }

    // Action to perform when notification is clicked.
    private Action<TaskItem?>? _clickAction;
    private DispatcherTimer? _timer; // Timer for auto-hiding.

    // Command to hide the toast.
    public ICommand HideCommand { get; }
    // Command for when notification is clicked.
    public ICommand NotificationClickedCommand { get; }

    // Constructor.
    public ToastNotificationViewModel()
    {
        IsVisible = false;
        HideCommand = new RelayCommand(_ => Hide());
        NotificationClickedCommand = new RelayCommand(_ => ExecuteClickAction(), _ => AssociatedTask != null && _clickAction != null);
    }

    // Shows the toast notification.
    public void Show(string message, ToastType type, int durationInSeconds = 5, TaskItem? task = null, Action<TaskItem?>? clickAction = null)
    {
        Message = message;
        ToastType = type;
        AssociatedTask = task;
        _clickAction = clickAction;
        IsVisible = true;
        (NotificationClickedCommand as RelayCommand)?.RaiseCanExecuteChanged();

        _timer?.Stop(); // Stop any existing timer.
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(durationInSeconds)
        };
        _timer.Tick += (s, e) => // Lambda for timer tick.
        {
            Hide();
            _timer?.Stop();
            _timer = null; // Release timer.
        };
        _timer.Start();
    }

    // Executes the click action for the notification.
    private void ExecuteClickAction()
    {
        if (AssociatedTask != null && _clickAction != null)
        {
            _clickAction.Invoke(AssociatedTask);
            Hide(); // Hide after click.
        }
    }

    // Hides the toast notification.
    public void Hide()
    {
        IsVisible = false;
        AssociatedTask = null; // Clear associated data.
        _clickAction = null;
        (NotificationClickedCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}