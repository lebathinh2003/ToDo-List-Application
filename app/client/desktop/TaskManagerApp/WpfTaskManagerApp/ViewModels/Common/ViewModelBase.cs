using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace WpfTaskManagerApp.ViewModels.Common;

// Base class for ViewModels.
public abstract class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
{
    // Event for property changes.
    public event PropertyChangedEventHandler? PropertyChanged;

    // Raises PropertyChanged event.
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Sets property, raises event, and validates if changed.
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        ValidateProperty(propertyName); // Trigger validation for the specific property.
        return true;
    }

    // Optional method for cleanup.
    public virtual void Dispose() { }

    // --- INotifyDataErrorInfo Implementation ---
    private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();

    // Event for error changes.
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    // True if there are any validation errors.
    public bool HasErrors => _errorsByPropertyName.Any();

    // Gets errors for a property or all errors.
    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            // All errors.
            return _errorsByPropertyName.Values.SelectMany(errors => errors).ToList();
        }
        // Errors for specific property.
        return _errorsByPropertyName.TryGetValue(propertyName!, out var errors) ? errors : Enumerable.Empty<string>();
    }

    // Raises ErrorsChanged event.
    protected void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        OnPropertyChanged(nameof(HasErrors)); // Notify that HasErrors might have changed.
    }

    // Adds an error for a property.
    protected void AddError(string propertyName, string error)
    {
        if (!_errorsByPropertyName.ContainsKey(propertyName))
        {
            _errorsByPropertyName[propertyName] = new List<string>();
        }
        if (!_errorsByPropertyName[propertyName].Contains(error))
        {
            _errorsByPropertyName[propertyName].Add(error);
            OnErrorsChanged(propertyName);
        }
    }

    // Clears errors for a property or all errors.
    protected void ClearErrors(string? propertyName = null)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            // Clear all errors.
            var propertiesWithErrors = _errorsByPropertyName.Keys.ToList();
            _errorsByPropertyName.Clear();
            foreach (var propName in propertiesWithErrors)
            {
                OnErrorsChanged(propName);
            }
            OnErrorsChanged(string.Empty); // Notify change for all properties (null or empty string).
        }
        else if (_errorsByPropertyName.ContainsKey(propertyName))
        {
            // Clear errors for a specific property.
            _errorsByPropertyName.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }
    }

    // Placeholder for property-specific validation logic.
    protected virtual void ValidateProperty(string? propertyName) { }

    // Placeholder for validating all properties.
    protected virtual void ValidateAllProperties() { }
}