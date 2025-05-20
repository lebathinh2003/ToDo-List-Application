using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using WpfTaskManagerApp.Models;
using System.Net.Http.Json;
using WpfTaskManagerApp.Configs;
using System.Net.Http.Headers;
using WpfTaskManagerApp.Interfaces;
namespace WpfTaskManagerApp.Services;

// API service for authentication.
public class ApiAuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private User? _currentUser;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IUserService _userService; // To fetch full user details.
    private readonly ITokenProvider _tokenProvider; // Manages auth token.

    // Currently authenticated user.
    public User? CurrentUser => _currentUser;

    // Constructor.
    public ApiAuthenticationService(HttpClient httpClient, IUserService userService, ITokenProvider tokenProvider)
    {
        _httpClient = httpClient;
        _userService = userService;
        _tokenProvider = tokenProvider;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }

    // Logs in a user via API.
    public async Task<User?> LoginAsync(string username, string password)
    {
        var loginRequest = new LoginRequestModel { Username = username, Password = password };
        try
        {
            // First, call identity/login endpoint.
            HttpResponseMessage identityResponse = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.AuthEndPoint}/login", loginRequest, _jsonSerializerOptions);

            if (identityResponse.IsSuccessStatusCode)
            {
                LoginResponseModel? loginResponse = await identityResponse.Content.ReadFromJsonAsync<LoginResponseModel>(_jsonSerializerOptions);
                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    _tokenProvider.SetToken(loginResponse.Token);

                    // Then, fetch full user details from user service.
                    User? userProfileDetails = await _userService.GetUserByIdAsync(loginResponse.UserId);

                    if (userProfileDetails != null)
                    {
                        // Merge info from login response and user profile.
                        _currentUser = userProfileDetails; // Base details from user service.
                        _currentUser.Role = loginResponse.Role; // Role from login response.
                        _currentUser.Id = loginResponse.UserId; // Ensure ID from login response.
                        _currentUser.Username = loginResponse.Username; // Username from login response.
                        _currentUser.Email = loginResponse.Email; // Email from login response.
                        return _currentUser;
                    }
                    else
                    {
                        _tokenProvider.ClearToken(); // Failed to get profile.
                    }
                }
            }
        }
        catch (Exception)
        {
            // Log or handle login errors.
        }

        _currentUser = null;
        _tokenProvider.ClearToken();
        return null;
    }

    // Logs out the current user.
    public Task LogoutAsync()
    {
        _currentUser = null;
        _tokenProvider.ClearToken();
        // API logout call could be added here if supported.
        return Task.CompletedTask;
    }

    // Checks if the user is currently authenticated.
    public async Task<bool> IsUserAuthenticatedAsync()
    {
        await Task.Delay(1); // Simulate async.
        string? token = _tokenProvider.GetToken();
        // Simple check: token exists and user object is populated.
        return !string.IsNullOrEmpty(token) && _currentUser != null;
    }

    // Changes the password for the current user.
    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordModel changePasswordModel)
    {
        var token = _tokenProvider.GetToken();
        if (string.IsNullOrEmpty(token)) return false; // No token.

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            // Endpoint for password change.
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.AuthEndPoint}/change-password", changePasswordModel, _jsonSerializerOptions);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            // Log or handle error.
        }
        return false;
    }
}