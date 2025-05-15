using System.Windows.Input;

namespace WpfTaskManagerApp.ViewModels.Common;

public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Predicate<object?>? _canExecute;

    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || _canExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        _execute(parameter);
    }

    // ***** THÊM PHƯƠNG THỨC NÀY *****
    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}

// RelayCommand với kiểu generic cho parameter
public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Predicate<T?>? _canExecute;

    public RelayCommand(Action<T?> execute, Predicate<T?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object? parameter)
    {
        if (parameter == null && typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null) // Check if T is a non-nullable value type
        {
            // For non-nullable value types, if parameter is null, pass default(T)
            return _canExecute == null || _canExecute(default(T));
        }
        if (parameter != null && !(parameter is T))
        {
            // If T is not nullable and parameter is not assignable to T, it cannot execute.
            // This handles cases like parameter being an int when T is string.
            // However, if T is nullable (e.g., int?), and parameter is null, it's fine.
            // If T is object, any parameter is fine.
            if (Nullable.GetUnderlyingType(typeof(T)) == null && !typeof(T).IsAssignableFrom(parameter.GetType()))
                return false;
        }

        // Attempt to cast/convert. If T is a value type and parameter is null, this will fail for non-nullable T.
        // For nullable T (like int?), (T?)null is valid.
        // For reference types, (T?)null is valid.
        T? typedParameter;
        try
        {
            typedParameter = (T?)parameter;
        }
        catch (InvalidCastException)
        {
            // This might happen if parameter is, for example, a string "123" and T is int.
            // Depending on desired behavior, you might try Convert.ChangeType or just return false.
            // For simplicity here, if direct cast fails and it's not null for a value type, assume false.
            // This part can be made more robust if complex type conversions are expected.
            if (parameter != null) return false; // If parameter is not null and cast failed.
            typedParameter = default(T); // If parameter is null and T is a reference type or nullable value type.
        }


        return _canExecute == null || _canExecute(typedParameter);
    }

    public void Execute(object? parameter)
    {
        if (parameter == null && typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null)
        {
            _execute(default(T));
        }
        else
        {
            // Similar logic to CanExecute for casting
            T? typedParameter;
            try
            {
                typedParameter = (T?)parameter;
            }
            catch (InvalidCastException)
            {
                // Handle or rethrow if parameter cannot be cast to T
                // For this example, we'll assume if CanExecute passed, Execute should work with a similar cast
                // or that the parameter type was already validated.
                // If parameter is null and T is a non-nullable value type, this would be an issue,
                // but CanExecute should have ideally prevented this.
                if (parameter != null && !typeof(T).IsAssignableFrom(parameter.GetType()))
                {
                    // This situation should ideally be caught by CanExecute
                    // Or you might attempt a Convert.ChangeType if appropriate
                    throw new ArgumentException($"Parameter is of type {parameter.GetType()} but type {typeof(T)} was expected.", nameof(parameter));
                }
                typedParameter = default(T);
            }
            _execute(typedParameter);
        }
    }

    // ***** THÊM PHƯƠNG THỨC NÀY *****
    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}
