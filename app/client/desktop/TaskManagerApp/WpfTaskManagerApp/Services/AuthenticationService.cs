using System.Diagnostics;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using WpfTaskManagerApp.Models;
using System.Net.Http.Json;
using WpfTaskManagerApp.Configs;
namespace WpfTaskManagerApp.Services;
public class ApiAuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private User? _currentUser;
    // _currentToken không còn được quản lý trực tiếp ở đây nữa
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly IUserService _userService;
    private readonly ITokenProvider _tokenProvider; // ***** THÊM ITokenProvider *****

    public User? CurrentUser => _currentUser;
    // public string? CurrentToken => _tokenProvider.GetToken(); // Lấy token từ TokenProvider nếu cần public

    // ***** CẬP NHẬT CONSTRUCTOR *****
    public ApiAuthenticationService(HttpClient httpClient, IUserService userService, ITokenProvider tokenProvider)
    {
        _httpClient = httpClient;
        _userService = userService;
        _tokenProvider = tokenProvider; // ***** LƯU TRỮ ITokenProvider *****
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

            if (identityResponse.IsSuccessStatusCode)
            {
                LoginResponseModel? loginResponse = await identityResponse.Content.ReadFromJsonAsync<LoginResponseModel>(_jsonSerializerOptions);
                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    // ***** SET TOKEN VÀO TOKENPROVIDER *****
                    _tokenProvider.SetToken(loginResponse.Token);

                    User? userProfileDetails = await _userService.GetUserByIdAsync(loginResponse.UserId);

                    if (userProfileDetails != null)
                    {
                        Debug.WriteLine($"ApiAuthenticationService.LoginAsync: User API returned - ID: {userProfileDetails.Id}, Username: '{userProfileDetails.Username}', FullName: '{userProfileDetails.FullName}', Email: '{userProfileDetails.Email}', Address: '{userProfileDetails.Address}', Role (from User API): {userProfileDetails.Role}, IsActive: {userProfileDetails.IsActive}");
                        _currentUser = userProfileDetails;
                        _currentUser.Role = loginResponse.Role;
                        _currentUser.Id = loginResponse.UserId;
                        _currentUser.Username = loginResponse.Username;
                        _currentUser.Email = loginResponse.Email;

                        Debug.WriteLine($"ApiAuthenticationService.LoginAsync: Successfully fetched and combined user details. ID: {_currentUser.Id}, Username: {_currentUser.Username}, FullName: '{_currentUser.FullName}', Role: '{_currentUser.Role}'");
                        return _currentUser;
                    }
                    else
                    {
                        Debug.WriteLine($"ApiAuthenticationService.LoginAsync: Failed to fetch full user details from User API for ID: {loginResponse.UserId} after identity login.");
                        _tokenProvider.ClearToken(); // Xóa token nếu không lấy được user
                        return null;
                    }
                }
                Debug.WriteLine($"Login API (Identity) response error: Invalid token or response structure. Status: {identityResponse.StatusCode}");
            }
            else
            {
                string errorContent = await identityResponse.Content.ReadAsStringAsync();
                Debug.WriteLine($"Login failed (Identity API): {identityResponse.StatusCode} - {errorContent}");
            }
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine($"Login request error: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Debug.WriteLine($"Login JSON parsing error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"An unexpected error occurred during login: {ex.Message}");
        }

        _currentUser = null;
        _tokenProvider.ClearToken(); // Đảm bảo token được xóa nếu có lỗi
        return null;
    }

    public Task LogoutAsync()
    {
        _currentUser = null;
        _tokenProvider.ClearToken(); // ***** XÓA TOKEN KHỎI TOKENPROVIDER *****
        Debug.WriteLine("User logged out.");
        return Task.CompletedTask;
    }

    public async Task<bool> IsUserAuthenticatedAsync()
    {
        // Nếu có token, thử lấy thông tin user để xác thực lại
        // Hoặc đơn giản là kiểm tra _currentUser và token trong TokenProvider
        string? token = _tokenProvider.GetToken();
        if (!string.IsNullOrEmpty(token) && _currentUser == null)
        {
            Debug.WriteLine("IsUserAuthenticatedAsync: Token exists, but CurrentUser is null. Attempting to re-fetch user details might be needed here.");
            // Ví dụ:
            // Guid? userIdFromToken = YourJwtTokenParsingLogic.GetUserId(token); 
            // if(userIdFromToken.HasValue && _userService != null)
            // {
            //     _currentUser = await _userService.GetUserByIdAsync(userIdFromToken.Value);
            //     if (_currentUser == null) _tokenProvider.ClearToken(); 
            // }
            // else
            // {
            //    _tokenProvider.ClearToken(); 
            // }
        }
        return !string.IsNullOrEmpty(_tokenProvider.GetToken()) && _currentUser != null;
    }
}