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
    public string Message { get => _message; set => SetProperty(ref _message, value); }
    private bool _isVisible;
    public bool IsVisible { get => _isVisible; set { if (SetProperty(ref _isVisible, value)) Debug.WriteLine($"ToastNotificationViewModel.IsVisible: PropertyChanged raised for IsVisible = {value}"); } }
    private ToastType _toastType;
    public ToastType ToastType { get => _toastType; set => SetProperty(ref _toastType, value); }
    private DispatcherTimer? _timer;
    public ICommand HideCommand { get; }

    public ToastNotificationViewModel()
    {
        IsVisible = false;
        HideCommand = new RelayCommand(_ => Hide());
    }
    public void Show(string message, ToastType type, int durationInSeconds = 4)
    {
        Message = message; ToastType = type; IsVisible = true;
        _timer?.Stop();
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(durationInSeconds) };
        _timer.Tick += (s, e) => { Hide(); _timer?.Stop(); _timer = null; };
        _timer.Start();
    }
    public void Hide() { IsVisible = false; }
}

