using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfTaskManagerApp.ViewModels.Common;
public abstract class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
{
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
        ValidateProperty(propertyName);
        return true;
    }
    public virtual void Dispose() { }

    private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    public bool HasErrors => _errorsByPropertyName.Any();
    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        { return _errorsByPropertyName.Values.SelectMany(errors => errors).ToList(); }
        return _errorsByPropertyName.ContainsKey(propertyName!) ? _errorsByPropertyName[propertyName!] : Enumerable.Empty<string>();
    }
    protected void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        OnPropertyChanged(nameof(HasErrors));
    }
    protected void AddError(string propertyName, string error)
    {
        if (!_errorsByPropertyName.ContainsKey(propertyName))
            _errorsByPropertyName[propertyName] = new List<string>();
        if (!_errorsByPropertyName[propertyName].Contains(error))
        { _errorsByPropertyName[propertyName].Add(error); OnErrorsChanged(propertyName); }
    }
    protected void ClearErrors(string? propertyName = null)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            var propertiesWithErrors = _errorsByPropertyName.Keys.ToList();
            _errorsByPropertyName.Clear();
            foreach (var propName in propertiesWithErrors) { OnErrorsChanged(propName); }
            OnErrorsChanged(null!);
        }
        else if (_errorsByPropertyName.ContainsKey(propertyName))
        { _errorsByPropertyName.Remove(propertyName); OnErrorsChanged(propertyName); }
    }
    protected virtual void ValidateProperty(string? propertyName) { }
    protected virtual void ValidateAllProperties() { }
}