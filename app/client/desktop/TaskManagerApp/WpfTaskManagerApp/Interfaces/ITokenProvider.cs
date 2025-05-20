namespace WpfTaskManagerApp.Interfaces;

// Provides access to the authentication token.
public interface ITokenProvider
{
    // Gets the current token.
    string? GetToken();

    // Sets a new token.
    void SetToken(string? token);

    // Clears the current token.
    void ClearToken();
}