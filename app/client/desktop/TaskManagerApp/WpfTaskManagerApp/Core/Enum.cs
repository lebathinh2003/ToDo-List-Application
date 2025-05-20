namespace WpfTaskManagerApp.Core;

// User roles.
public enum UserRole
{
    Admin,
    Staff
}

// Task statuses.
public enum TaskStatus
{
    ToDo,
    InProgress,
    Done,
    Cancelled
}

// Toast notification types.
public enum ToastType
{
    Information,
    Success,
    Warning,
    Error
}

// Represents a task status for UI display.
public class TaskStatusItem
{
    // The actual status value (nullable).
    public TaskStatus? Status { get; }
    // Text to display for the status.
    public string DisplayName { get; }

    // Constructor.
    public TaskStatusItem(TaskStatus? status, string displayName)
    {
        Status = status;
        DisplayName = displayName;
    }

    // Returns the display name for string representation.
    public override string ToString() => DisplayName;
}