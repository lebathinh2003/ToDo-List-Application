using System.Windows.Input;
namespace WpfTaskManagerApp.ViewModels.Common;

// Basic ICommand implementation.
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute; // Action to run.
    private readonly Predicate<object?>? _canExecute; // Predicate to check executability.

    // Notifies when CanExecute changes.
    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    // Constructor.
    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    // Checks if command can execute.
    public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);

    // Executes the command.
    public void Execute(object? parameter) => _execute(parameter);

    // Manually raises CanExecuteChanged.
    public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
}

// Generic ICommand implementation.
public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute; // Typed action to run.
    private readonly Predicate<T?>? _canExecute; // Typed predicate.

    // Notifies when CanExecute changes.
    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    // Constructor.
    public RelayCommand(Action<T?> execute, Predicate<T?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    // Checks if command can execute with typed parameter.
    public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(ConvertParameter(parameter));

    // Executes the command with typed parameter.
    public void Execute(object? parameter) => _execute(ConvertParameter(parameter));

    // Manually raises CanExecuteChanged.
    public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();

    // Converts object parameter to T?.
    private T? ConvertParameter(object? parameter)
    {
        if (parameter == null && typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null)
        {
            return default; // For non-nullable value types, use default.
        }
        return (T?)parameter;
    }
}