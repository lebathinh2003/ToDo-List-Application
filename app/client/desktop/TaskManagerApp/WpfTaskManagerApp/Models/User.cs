using System; // Added for Guid
using System.Collections.Generic; // Added for EqualityComparer
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfTaskManagerApp.Core;

namespace WpfTaskManagerApp.Models;

// Represents a user, implements INotifyPropertyChanged for UI updates.
public class User : INotifyPropertyChanged
{
    private Guid _id;
    // User unique identifier.
    public Guid Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private string _username = string.Empty;
    // User's login name.
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    private string _email = string.Empty;
    // User's email address.
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    // Hashed password (client-side usually doesn't need INPC).
    public string? PasswordHash { get; set; }

    private UserRole _role;
    // User's role (e.g., Admin, Staff).
    public UserRole Role
    {
        get => _role;
        set => SetProperty(ref _role, value);
    }

    private string _fullName = string.Empty;
    // User's full name.
    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    private string? _address;
    // User's address (nullable).
    public string? Address
    {
        get => _address;
        set => SetProperty(ref _address, value);
    }

    private bool _isActive = true;
    // Indicates if the user account is active.
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    // Default constructor.
    public User() { }

    // Constructor with initial values.
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

    // Event for property changes.
    public event PropertyChangedEventHandler? PropertyChanged;

    // Raises PropertyChanged event.
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Sets property and raises event if changed.
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    // String representation for display (e.g., in ComboBox).
    public override string ToString()
    {
        return FullName;
    }
}