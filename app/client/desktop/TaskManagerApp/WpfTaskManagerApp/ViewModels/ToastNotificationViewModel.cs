using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.ViewModels.Common;
using System.Diagnostics; // THÊM CHO DEBUG

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
            Debug.WriteLine($"ToastNotificationViewModel.IsVisible SETTER: Old='{_isVisible}', New='{value}'"); // DEBUG
            if (SetProperty(ref _isVisible, value))
            {
                Debug.WriteLine($"ToastNotificationViewModel.IsVisible: PropertyChanged raised for IsVisible = {value}"); // DEBUG
            }
        }
    }

    private ToastType _toastType;
    public ToastType ToastType
    {
        get => _toastType;
        set => SetProperty(ref _toastType, value);
    }

    private DispatcherTimer? _timer;
    public ICommand HideCommand { get; }

    public ToastNotificationViewModel()
    {
        IsVisible = false;
        HideCommand = new RelayCommand(_ => Hide());
        Debug.WriteLine("ToastNotificationViewModel: Constructor called."); // DEBUG
    }

    public void Show(string message, ToastType type, int durationInSeconds = 4)
    {
        Debug.WriteLine($"ToastNotificationViewModel.Show: Called with message='{message}', type='{type}', duration='{durationInSeconds}'"); // DEBUG
        Message = message;
        ToastType = type;
        IsVisible = true; // Điều này sẽ trigger setter và log

        _timer?.Stop();

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(durationInSeconds) };
        _timer.Tick += (sender, args) =>
        {
            Debug.WriteLine("ToastNotificationViewModel: Timer ticked, calling Hide()."); // DEBUG
            Hide();
            _timer?.Stop();
            _timer = null;
        };
        _timer.Start();
        Debug.WriteLine("ToastNotificationViewModel.Show: Timer started."); // DEBUG
    }

    public void Hide()
    {
        Debug.WriteLine("ToastNotificationViewModel.Hide: Called."); // DEBUG
        IsVisible = false; // Điều này sẽ trigger setter và log
    }
}
