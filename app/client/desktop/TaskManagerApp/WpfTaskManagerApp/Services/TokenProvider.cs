using WpfTaskManagerApp.Interfaces;

namespace WpfTaskManagerApp.Services;

// Manages the authentication token.
public class TokenProvider : ITokenProvider
{
    private string? _currentToken;

    // Retrieves the current token.
    public string? GetToken()
    {
        return _currentToken;
    }

    // Stores the given token.
    public void SetToken(string? token)
    {
        _currentToken = token;
    }

    // Removes the current token.
    public void ClearToken()
    {
        _currentToken = null;
    }
}