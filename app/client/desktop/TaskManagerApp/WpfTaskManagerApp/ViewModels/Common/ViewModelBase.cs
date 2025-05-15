using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfTaskManagerApp.ViewModels.Common;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Hàm hỗ trợ để set giá trị cho property và gọi OnPropertyChanged
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    // Thường thì các ViewModel sẽ cần giải phóng tài nguyên hoặc hủy đăng ký sự kiện
    public virtual void Dispose() { }
}
