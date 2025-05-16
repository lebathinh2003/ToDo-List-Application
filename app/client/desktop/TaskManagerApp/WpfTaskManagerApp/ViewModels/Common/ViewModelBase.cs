using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfTaskManagerApp.ViewModels.Common;
public abstract class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo // THÊM INotifyDataErrorInfo
{
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
        ValidateProperty(propertyName); // Gọi validate sau khi property thay đổi
        return true;
    }
    public virtual void Dispose() { }


    // INotifyDataErrorInfo implementation
    private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public bool HasErrors => _errorsByPropertyName.Any();

    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName)) // Trả về tất cả lỗi nếu propertyName là null hoặc rỗng
        {
            return _errorsByPropertyName.Values.SelectMany(errors => errors).ToList();
        }
        return _errorsByPropertyName.ContainsKey(propertyName!) ? _errorsByPropertyName[propertyName!] : Enumerable.Empty<string>();
    }

    protected void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        // Cũng thông báo thay đổi cho HasErrors để các binding liên quan được cập nhật
        OnPropertyChanged(nameof(HasErrors));
    }

    protected void AddError(string propertyName, string error)
    {
        if (!_errorsByPropertyName.ContainsKey(propertyName))
            _errorsByPropertyName[propertyName] = new List<string>();

        if (!_errorsByPropertyName[propertyName].Contains(error))
        {
            _errorsByPropertyName[propertyName].Add(error);
            OnErrorsChanged(propertyName);
        }
    }

    protected void ClearErrors(string? propertyName = null)
    {
        if (string.IsNullOrEmpty(propertyName)) // Xóa tất cả lỗi
        {
            _errorsByPropertyName.Clear();
            // Thông báo thay đổi cho tất cả các property đã từng có lỗi (hoặc dùng null/empty string)
            // Một cách đơn giản là raise cho các property đã biết hoặc cho null.
            // Tuy nhiên, để UI cập nhật đúng, cần raise cho từng property đã xóa lỗi.
            var propertiesWithErrors = _errorsByPropertyName.Keys.ToList(); // Lấy danh sách key trước khi xóa
            _errorsByPropertyName.Clear();
            foreach (var propName in propertiesWithErrors)
            {
                OnErrorsChanged(propName);
            }
            OnErrorsChanged(null); // Thông báo chung là lỗi đã thay đổi
        }
        else if (_errorsByPropertyName.ContainsKey(propertyName)) // Xóa lỗi cho một property cụ thể
        {
            _errorsByPropertyName.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }
    }

    // Phương thức ảo để các ViewModel con có thể override và thực hiện validation cụ thể
    protected virtual void ValidateProperty(string? propertyName)
    {
        // Các ViewModel con sẽ override phương thức này để gọi logic validation cho propertyName cụ thể
    }

    // Validate tất cả các property (hữu ích khi submit form)
    protected virtual void ValidateAllProperties()
    {
        // Các ViewModel con sẽ override để validate tất cả các property của chúng
    }
}