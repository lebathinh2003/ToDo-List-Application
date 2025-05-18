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
    private readonly ITokenProvider _tokenProvider;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ApiUserService(HttpClient httpClient, ITokenProvider tokenProvider)
    {
        _httpClient = httpClient;
        _tokenProvider = tokenProvider;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }

    private async Task SetAuthHeader()
    {
        var token = _tokenProvider.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else { _httpClient.DefaultRequestHeaders.Authorization = null; }
        await Task.CompletedTask;
    }

    public async Task<PaginatedResult<User>?> GetUsersAsync(int skip = 0, int limit = 10, string? sortBy = null, string? sortOrder = "asc", string? keyword = null, bool includeInactive = false)
    {
        await SetAuthHeader();
        var queryParams = new List<string> { $"skip={skip}", $"limit={limit}" };
        if (!string.IsNullOrWhiteSpace(sortBy)) queryParams.Add($"sortBy={Uri.EscapeDataString(sortBy)}");
        if (!string.IsNullOrWhiteSpace(sortOrder)) queryParams.Add($"sortOrder={Uri.EscapeDataString(sortOrder)}");
        if (!string.IsNullOrWhiteSpace(keyword)) queryParams.Add($"keyword={Uri.EscapeDataString(keyword)}");
        if (includeInactive) queryParams.Add("includeInactive=true");

        string requestUri = $"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}";
        if (queryParams.Any()) requestUri += "?" + string.Join("&", queryParams);
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PaginatedResult<User>>(_jsonSerializerOptions);
            }
            // Debug.WriteLine($"Error fetching users: {response.StatusCode}");
        }
        catch (Exception ex) { Debug.WriteLine($"GetUsersAsync error: {ex.Message}"); }
        return null;
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        await SetAuthHeader();
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}/id/{userId}");
            // Debug.WriteLine($"ApiUserService.GetUserByIdAsync: User API response status for ID {userId}: {response.StatusCode}");
            if (response.IsSuccessStatusCode)
            {
                // string jsonResponse = await response.Content.ReadAsStringAsync();
                // Debug.WriteLine($"ApiUserService.GetUserByIdAsync: User API JSON response for ID {userId}: {jsonResponse}");
                User? user = await response.Content.ReadFromJsonAsync<User>(_jsonSerializerOptions);
                // if (user != null)
                // {
                //     Debug.WriteLine($"ApiUserService.GetUserByIdAsync: Deserialized user {userId} - FullName: '{user.FullName}', Email: '{user.Email}', Address: '{user.Address}', Username: '{user.Username}', Role (from User API): {user.Role}, IsActive: {user.IsActive}");
                // } else {
                //     Debug.WriteLine($"ApiUserService.GetUserByIdAsync: Deserialization returned null for user {userId}.");
                // }
                return user;
            }
            // Debug.WriteLine($"Error fetching user {userId}: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        }
        catch (JsonException jsonEx) { Debug.WriteLine($"ApiUserService.GetUserByIdAsync JSON deserialization error for {userId}: {jsonEx.Message}"); }
        catch (Exception ex) { Debug.WriteLine($"GetUserByIdAsync error for {userId}: {ex.Message}"); }
        return null;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        await SetAuthHeader();
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}/username/{Uri.EscapeDataString(username)}");
            if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<User>(_jsonSerializerOptions);
        }
        catch (Exception ex) { Debug.WriteLine($"GetUserByUsernameAsync error: {ex.Message}"); }
        return null;
    }
    public async Task<User?> AddUserAsync(User user, string password)
    {
        await SetAuthHeader();
        var createUserRequest = new { user.Username, user.Email, Password = password, user.FullName, user.Address, user.IsActive };
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}", createUserRequest, _jsonSerializerOptions);
            if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<User>(_jsonSerializerOptions);
            // Debug.WriteLine($"Error adding user: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex) { Debug.WriteLine($"AddUserAsync error: {ex.Message}"); }
        return null;
    }

    public async Task<bool> AdminUpdateUserAsync(Guid userId, User userToUpdate, string? newPassword = null)
    {
        await SetAuthHeader();
        // Tạo payload động dựa trên việc newPassword có được cung cấp hay không
        object updateUserPayload;
        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            updateUserPayload = new
            {
                userToUpdate.FullName,
                userToUpdate.Email,
                userToUpdate.Username,
                userToUpdate.Address,
                userToUpdate.IsActive,
                Password = newPassword // API backend cần hỗ trợ nhận trường Password tùy chọn
            };
        }
        else
        {
            updateUserPayload = new
            {
                userToUpdate.FullName,
                userToUpdate.Email,
                userToUpdate.Username,
                userToUpdate.Address,
                userToUpdate.IsActive
            };
        }

        try
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}/id/{userId}", updateUserPayload, _jsonSerializerOptions);
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"AdminUpdateUserAsync API error for {userId}: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) { Debug.WriteLine($"AdminUpdateUserAsync error for {userId}: {ex.Message}"); }
        return false;
    }

    public async Task<bool> UpdateCurrentUserProfileAsync(User userToUpdate)
    {
        await SetAuthHeader();
        var updateUserRequest = new
        {
            userToUpdate.FullName,
            userToUpdate.Email,
            userToUpdate.Address
        };
        try
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserProfileEndPoint}", updateUserRequest, _jsonSerializerOptions);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) { Debug.WriteLine($"UpdateCurrentUserProfileAsync error: {ex.Message}"); }
        return false;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        await SetAuthHeader();
        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}/delete/id/{userId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) { Debug.WriteLine($"DeleteUserAsync error: {ex.Message}"); }
        return false;
    }
    public async Task<bool> RestoreUserAsync(Guid userId)
    {
        await SetAuthHeader();
        try
        {
            // Giả sử API dùng PATCH hoặc POST đến một endpoint cụ thể để restore
            //var patchDoc = new[] { new { op = "replace", path = "/isActive", value = true } };
            //HttpResponseMessage response = await _httpClient.PatchAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}/id/{userId}/restore", patchDoc);
            HttpResponseMessage response = await _httpClient.PostAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}/restore/id/{userId}", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) { Debug.WriteLine($"RestoreUserAsync error: {ex.Message}"); }
        return false;
    }
}