using System;
using System.Collections.Generic; // Added for EqualityComparer
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TaskStatus = WpfTaskManagerApp.Core.TaskStatus; // Alias for TaskStatus

namespace WpfTaskManagerApp.Models;

// Represents a task item, implements INotifyPropertyChanged for UI updates.
public class TaskItem : INotifyPropertyChanged
{
    private Guid _id;
    // Task unique identifier.
    public Guid Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private string _code = string.Empty;
    // Task title.
    public string Code
    {
        get => _code;
        set => SetProperty(ref _code, value);
    }

    private string _title = string.Empty;
    // Task title.
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private string _description = string.Empty;
    // Task description.
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private Guid? _assigneeId;
    // ID of the assigned user (nullable).
    public Guid? AssigneeId
    {
        get => _assigneeId;
        set => SetProperty(ref _assigneeId, value);
    }

    private string? _assigneeName;
    // Name of the assigned user (nullable).
    public string? AssigneeName
    {
        get => _assigneeName;
        set => SetProperty(ref _assigneeName, value);
    }

    private string? _assigneeUsername;
    // Username of the assigned user (nullable).
    public string? AssigneeUsername
    {
        get => _assigneeUsername;
        set => SetProperty(ref _assigneeUsername, value);
    }

    private TaskStatus _status;
    // Current status of the task.
    public TaskStatus Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    private bool _isActive = true;
    // Indicates if the task is active.
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    private DateTime _createdDate = DateTime.UtcNow;
    // Date the task was created.
    public DateTime CreatedDate
    {
        get => _createdDate;
        set => SetProperty(ref _createdDate, value);
    }

    private DateTime? _dueDate;
    // Due date for the task (nullable).
    public DateTime? DueDate
    {
        get => _dueDate;
        set => SetProperty(ref _dueDate, value);
    }

    // Default constructor.
    public TaskItem() { }

    // Constructor with initial values.
    public TaskItem(Guid id, string code, string title, string description, TaskStatus status = TaskStatus.ToDo, bool isActive = true)
    {
        Id = id;
        Code = code;
        Title = title;
        Description = description;
        Status = status;
        IsActive = isActive;
        CreatedDate = DateTime.UtcNow;
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
}