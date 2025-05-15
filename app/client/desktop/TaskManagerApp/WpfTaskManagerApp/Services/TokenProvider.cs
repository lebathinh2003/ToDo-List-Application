namespace WpfTaskManagerApp.Services;
public class TokenProvider : ITokenProvider
{
    private string? _currentToken;

    public string? GetToken()
    {
        return _currentToken;
    }

    public void SetToken(string? token)
    {
        _currentToken = token;
    }

    public void ClearToken()
    {
        _currentToken = null;
    }
}
