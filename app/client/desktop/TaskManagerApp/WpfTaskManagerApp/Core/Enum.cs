namespace WpfTaskManagerApp.Core;
public enum UserRole
{
    Admin,
    Staff
}

public enum TaskStatus
{
    ToDo,
    InProgress,
    Done,
    Cancelled
}
public enum ToastType
{
    Information,
    Success,
    Warning,
    Error
}

public class TaskStatusItem
{
    public TaskStatus? Status { get; }
    public string DisplayName { get; }

    public TaskStatusItem(TaskStatus? status, string displayName)
    {
        Status = status;
        DisplayName = displayName;
    }

    public override string ToString() => DisplayName;
}


