using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using WpfTaskManagerApp.Models;
using System.Net.Http.Json;
using WpfTaskManagerApp.Configs;
namespace WpfTaskManagerApp.Services;
public class ApiUserService : IUserService
{
    private readonly HttpClient _httpClient;
    // ***** THAY IAuthenticationService BẰNG ITokenProvider *****
    private readonly ITokenProvider _tokenProvider;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    // ***** CẬP NHẬT CONSTRUCTOR *****
    public ApiUserService(HttpClient httpClient, ITokenProvider tokenProvider)
    {
        _httpClient = httpClient;
        _tokenProvider = tokenProvider; // ***** LƯU TRỮ ITokenProvider *****
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }

    private async Task SetAuthHeader()
    {
        // ***** LẤY TOKEN TỪ TOKENPROVIDER *****
        var token = _tokenProvider.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<User>> GetUsersAsync(string? searchTerm = null, bool includeInactive = false)
    {
        await SetAuthHeader();
        var queryParams = new List<string>();
        if (!string.IsNullOrWhiteSpace(searchTerm)) queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
        if (includeInactive) queryParams.Add("includeInactive=true");

        string requestUri = $"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}";
        if (queryParams.Any()) requestUri += "?" + string.Join("&", queryParams);

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadFromJsonAsync<List<User>>(_jsonSerializerOptions);
                return users ?? new List<User>();
            }
            Debug.WriteLine($"Error fetching users: {response.StatusCode}");
        }
        catch (Exception ex) { Debug.WriteLine($"GetUsersAsync error: {ex.Message}"); }
        return new List<User>();
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        await SetAuthHeader();
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}?id={userId}");
            if (response.IsSuccessStatusCode)
            {
                var r = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("STRINGGGGG:" + r);
                User? user = await response.Content.ReadFromJsonAsync<User>(_jsonSerializerOptions);
                if (user != null)
                {
                    Debug.WriteLine($"ApiUserService.GetUserByIdAsync: Fetched user {userId}, FullName: '{user.FullName}', Email: '{user.Email}', Role (from User API): {user.Role}");
                }
                return user;
            }
            Debug.WriteLine($"Error fetching user {userId}: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex) { Debug.WriteLine($"GetUserByIdAsync error for {userId}: {ex.Message}"); }
        return null;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        await SetAuthHeader();
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}/username/{Uri.EscapeDataString(username)}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>(_jsonSerializerOptions);
            }
            Debug.WriteLine($"Error fetching user by username {username}: {response.StatusCode}");
        }
        catch (Exception ex) { Debug.WriteLine($"GetUserByUsernameAsync error: {ex.Message}"); }
        return null;
    }


    public async Task<User?> AddUserAsync(User user, string password)
    {
        await SetAuthHeader();
        var createUserRequest = new
        {
            user.Username,
            user.Email,
            Password = password,
            user.Role,
            user.FullName,
            user.Address,
            user.IsActive
        };
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}", createUserRequest, _jsonSerializerOptions);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>(_jsonSerializerOptions);
            }
            Debug.WriteLine($"Error adding user: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex) { Debug.WriteLine($"AddUserAsync error: {ex.Message}"); }
        return null;
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        await SetAuthHeader();
        var updateUserRequest = new
        {
            user.Username,
            user.Email,
            user.Role,
            user.FullName,
            user.Address,
            user.IsActive
        };
        try
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}/{user.Id}", updateUserRequest, _jsonSerializerOptions);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) { Debug.WriteLine($"UpdateUserAsync error: {ex.Message}"); }
        return false;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        await SetAuthHeader();
        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}/{userId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) { Debug.WriteLine($"DeleteUserAsync error: {ex.Message}"); }
        return false;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordModel changePasswordModel)
    {
        await SetAuthHeader();
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}/{userId}/change-password", changePasswordModel, _jsonSerializerOptions);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) { Debug.WriteLine($"ChangePasswordAsync error: {ex.Message}"); }
        return false;
    }
}
