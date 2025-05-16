using System.Diagnostics;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using WpfTaskManagerApp.Models;
using System.Net.Http.Json;
using WpfTaskManagerApp.Configs;
using System.Net.Http.Headers;
namespace WpfTaskManagerApp.Services;
public class ApiAuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private User? _currentUser;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IUserService _userService;
    private readonly ITokenProvider _tokenProvider;

    public User? CurrentUser => _currentUser;

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

    public async Task<User?> LoginAsync(string username, string password)
    {
        var loginRequest = new LoginRequestModel { Username = username, Password = password };
        try
        {
            HttpResponseMessage identityResponse = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.AuthEndPoint}/login", loginRequest, _jsonSerializerOptions);
            // Debug.WriteLine($"ApiAuthenticationService.LoginAsync: Identity API response status: {identityResponse.StatusCode}");

            if (identityResponse.IsSuccessStatusCode)
            {
                LoginResponseModel? loginResponse = await identityResponse.Content.ReadFromJsonAsync<LoginResponseModel>(_jsonSerializerOptions);
                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    // Debug.WriteLine($"ApiAuthenticationService.LoginAsync: Identity login successful. Token received. UserId: {loginResponse.UserId}, Username: {loginResponse.Username}, Role: {loginResponse.Role}");
                    _tokenProvider.SetToken(loginResponse.Token);

                    // Debug.WriteLine($"ApiAuthenticationService.LoginAsync: Attempting to fetch full user details for UserId: {loginResponse.UserId}");
                    User? userProfileDetails = await _userService.GetUserByIdAsync(loginResponse.UserId);

                    if (userProfileDetails != null)
                    {
                        // Debug.WriteLine($"ApiAuthenticationService.LoginAsync: User API returned - ID: {userProfileDetails.Id}, Username: '{userProfileDetails.Username}', FullName: '{userProfileDetails.FullName}', Email: '{userProfileDetails.Email}', Address: '{userProfileDetails.Address}', Role (from User API): {userProfileDetails.Role}, IsActive: {userProfileDetails.IsActive}");
                        _currentUser = userProfileDetails;
                        _currentUser.Role = loginResponse.Role;
                        _currentUser.Id = loginResponse.UserId;
                        _currentUser.Username = loginResponse.Username;
                        _currentUser.Email = loginResponse.Email;

                        // Debug.WriteLine($"ApiAuthenticationService.LoginAsync: Final _currentUser object - ID: {_currentUser.Id}, Username: '{_currentUser.Username}', FullName: '{_currentUser.FullName}', Email: '{_currentUser.Email}', Address: '{_currentUser.Address}', Role: '{_currentUser.Role}', IsActive: {_currentUser.IsActive}");
                        return _currentUser;
                    }
                    else
                    {
                        // Debug.WriteLine($"ApiAuthenticationService.LoginAsync: Failed to fetch full user details from User API for ID: {loginResponse.UserId} after identity login. UserProfileDetails is null.");
                        _tokenProvider.ClearToken();
                        return null;
                    }
                }
                // Debug.WriteLine($"Login API (Identity) response error: Invalid token or response structure. Status: {identityResponse.StatusCode}");
            }
            else
            {
                // string errorContent = await identityResponse.Content.ReadAsStringAsync();
                // Debug.WriteLine($"Login failed (Identity API): {identityResponse.StatusCode} - {errorContent}");
            }
        }
        catch (HttpRequestException ex) { Debug.WriteLine($"Login request error: {ex.Message}"); }
        catch (JsonException ex) { Debug.WriteLine($"Login JSON parsing error: {ex.Message}"); }
        catch (Exception ex) { Debug.WriteLine($"An unexpected error occurred during login: {ex.Message}"); }

        _currentUser = null;
        _tokenProvider.ClearToken();
        return null;
    }

    public Task LogoutAsync()
    {
        _currentUser = null;
        _tokenProvider.ClearToken();
        // Debug.WriteLine("User logged out.");
        return Task.CompletedTask;
    }

    public async Task<bool> IsUserAuthenticatedAsync()
    {
        string? token = _tokenProvider.GetToken();
        if (!string.IsNullOrEmpty(token) && _currentUser == null)
        {
            // Debug.WriteLine("ApiAuthenticationService.IsUserAuthenticatedAsync: Token exists, but CurrentUser is null. Re-validation logic might be needed here.");
        }
        return !string.IsNullOrEmpty(_tokenProvider.GetToken()) && _currentUser != null;
    }

    // ***** TRIỂN KHAI ChangePasswordAsync TRẢ VỀ bool *****
    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordModel changePasswordModel)
    {
        var token = _tokenProvider.GetToken();
        if (string.IsNullOrEmpty(token))
        {
            Debug.WriteLine("ChangePasswordAsync: No token available.");
            return false;
        }
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            Debug.WriteLine($"ChangePasswordAsync API error: {changePasswordModel.CurrentPassword} - {changePasswordModel.NewPassword}");

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.AuthEndPoint}/change-password", changePasswordModel, _jsonSerializerOptions);

            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"ChangePasswordAsync API error: {response.StatusCode} - {errorContent}");
                // Bạn có thể log lỗi chi tiết ở đây, nhưng không trả về cho ViewModel
            }
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ChangePasswordAsync exception: {ex.Message}");
        }
        return false;
    }
}