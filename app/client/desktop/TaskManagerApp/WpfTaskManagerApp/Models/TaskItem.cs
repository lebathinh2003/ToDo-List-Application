using System.ComponentModel;
using System.Runtime.CompilerServices;
using TaskStatus = WpfTaskManagerApp.Core.TaskStatus;
namespace WpfTaskManagerApp.Models;
public class TaskItem : INotifyPropertyChanged
{
    private Guid _id;
    public Guid Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private Guid? _assigneeId;
    public Guid? AssigneeId
    {
        get => _assigneeId;
        set => SetProperty(ref _assigneeId, value);
    }

    private string? _assigneeName;
    public string? AssigneeName
    {
        get => _assigneeName;
        set => SetProperty(ref _assigneeName, value);
    }

    private string? _assigneeUsername;
    public string? AssigneeUsername
    {
        get => _assigneeUsername;
        set => SetProperty(ref _assigneeUsername, value);
    }

    private TaskStatus _status;
    public TaskStatus Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    private bool _isActive = true;
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    private DateTime _createdDate = DateTime.UtcNow;
    public DateTime CreatedDate
    {
        get => _createdDate;
        set => SetProperty(ref _createdDate, value);
    }

    private DateTime? _dueDate;
    public DateTime? DueDate
    {
        get => _dueDate;
        set => SetProperty(ref _dueDate, value);
    }

    public TaskItem() { }

    public TaskItem(Guid id, string title, string description, TaskStatus status = TaskStatus.ToDo, bool isActive = true)
    {
        Id = id;
        Title = title;
        Description = description;
        Status = status;
        IsActive = isActive;
        CreatedDate = DateTime.UtcNow;
    }

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
}