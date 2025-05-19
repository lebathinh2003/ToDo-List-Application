using System.Windows.Input;
using System.Windows.Threading;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.ViewModels.Common;
using System.Diagnostics;
using WpfTaskManagerApp.Models; // THÊM CHO DEBUG

namespace WpfTaskManagerApp.ViewModels;
public class ToastNotificationViewModel : ViewModelBase
{
    private string _message = string.Empty;
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }
    private bool _isVisible;
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (SetProperty(ref _isVisible, value)) Debug.WriteLine($"ToastNotificationViewModel.IsVisible: PropertyChanged raised for IsVisible = {value}");
        }
    }
    private ToastType _toastType;
    public ToastType ToastType
    {
        get => _toastType;
        set => SetProperty(ref _toastType, value);
    }

    private TaskItem? _associatedTask;
    public TaskItem? AssociatedTask
    {
        get => _associatedTask;
        private set => SetProperty(ref _associatedTask, value);
    }
    private Action<TaskItem?>? _clickAction;

    private DispatcherTimer? _timer;
    public ICommand HideCommand
    {
        get;
    }
    public ICommand NotificationClickedCommand
    {
        get;
    }

    public ToastNotificationViewModel()
    {
        IsVisible = false;
        HideCommand = new RelayCommand(_ => Hide());
        NotificationClickedCommand = new RelayCommand(_ => ExecuteClickAction(), _ => AssociatedTask != null && _clickAction != null);
    }

    // Phương thức Show này vẫn có thể được dùng cho các thông báo toast trong ứng dụng không phải từ SignalR
    public void Show(string message, ToastType type, int durationInSeconds = 5, TaskItem? task = null, Action<TaskItem?>? clickAction = null)
    {
        Message = message;
        ToastType = type;
        AssociatedTask = task;
        _clickAction = clickAction;
        IsVisible = true;
        (NotificationClickedCommand as RelayCommand)?.RaiseCanExecuteChanged();

        _timer?.Stop();
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(durationInSeconds)
        };
        _timer.Tick += (s, e) => {
            Hide();
            _timer?.Stop();
            _timer = null;
        };
        _timer.Start();
    }
    private void ExecuteClickAction()
    {
        if (AssociatedTask != null && _clickAction != null)
        {
            _clickAction.Invoke(AssociatedTask);
            Hide();
        }
    }
    public void Hide()
    {
        IsVisible = false;
        AssociatedTask = null;
        _clickAction = null;
        (NotificationClickedCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}