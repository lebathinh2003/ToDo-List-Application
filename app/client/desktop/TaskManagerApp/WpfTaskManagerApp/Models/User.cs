using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using WpfTaskManagerApp.Core;

namespace WpfTaskManagerApp.Models;
public class User : INotifyPropertyChanged // KẾ THỪA INotifyPropertyChanged
{
    private Guid _id;

    public Guid Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private string _username = string.Empty;
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string? PasswordHash { get; set; } // Không cần PropertyChanged cho cái này ở client

    private UserRole _role;
    public UserRole Role
    {
        get => _role;
        set => SetProperty(ref _role, value);
    }

    private string _fullName = string.Empty;
    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    private string? _address;
    public string? Address
    {
        get => _address;
        set => SetProperty(ref _address, value);
    }

    private bool _isActive = true;
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    // Các thuộc tính không hiển thị, không cần INotifyPropertyChanged nếu không bind trực tiếp
    // public DateTime CreatedAt { get; set; } 
    // public DateTime UpdatedAt { get; set; }

    public User() { }

    public User(Guid id, string username, string email, UserRole role, string fullName, string? address = null, bool isActive = true)
    {
        Id = id;
        Username = username;
        Email = email;
        Role = role;
        FullName = fullName;
        Address = address;
        IsActive = isActive;
    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    //User for replace DisplayMemberPath when using ItemTemplate
    public override string ToString()
    {
        return FullName;
    }
}
