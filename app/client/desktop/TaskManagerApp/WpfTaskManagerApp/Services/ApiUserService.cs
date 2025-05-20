using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using WpfTaskManagerApp.Configs;
using WpfTaskManagerApp.Interfaces;
using WpfTaskManagerApp.Models;

namespace WpfTaskManagerApp.Services;

// API service for user operations.
public class ApiUserService : IUserService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenProvider _tokenProvider;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    // Constructor.
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

    // Sets authorization header.
    private async Task SetAuthHeader()
    {
        var token = _tokenProvider.GetToken();
        _httpClient.DefaultRequestHeaders.Authorization = !string.IsNullOrEmpty(token)
            ? new AuthenticationHeaderValue("Bearer", token)
            : null;
        await Task.CompletedTask;
    }

    // Gets users with pagination and filters.
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
        }
        catch (Exception) { /* Log error */ }
        return null;
    }

    // Gets a user by ID.
    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        await SetAuthHeader();
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}/id/{userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>(_jsonSerializerOptions);
            }
        }
        catch (Exception) { /* Log error */ }
        return null;
    }

    // Gets a user by username.
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
        }
        catch (Exception) { /* Log error */ }
        return null;
    }

    // Adds a new user.
    public async Task<User?> AddUserAsync(User user, string password)
    {
        await SetAuthHeader();
        var createUserRequest = new { user.Username, user.Email, Password = password, user.FullName, user.Address, user.IsActive };
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}", createUserRequest, _jsonSerializerOptions);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>(_jsonSerializerOptions);
            }
        }
        catch (Exception) { /* Log error */ }
        return null;
    }

    // Admin updates a user, optionally with a new password.
    public async Task<bool> AdminUpdateUserAsync(Guid userId, User userToUpdate, string? newPassword = null)
    {
        await SetAuthHeader();
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
                Password = newPassword // API needs to support optional password field.
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
            return response.IsSuccessStatusCode;
        }
        catch (Exception) { /* Log error */ }
        return false;
    }

    // Current user updates their profile.
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
        catch (Exception) { /* Log error */ }
        return false;
    }

    // Deletes a user.
    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        await SetAuthHeader();
        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}/delete/id/{userId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception) { /* Log error */ }
        return false;
    }

    // Restores a deleted user.
    public async Task<bool> RestoreUserAsync(Guid userId)
    {
        await SetAuthHeader();
        try
        {
            // API uses POST to a specific restore endpoint.
            HttpResponseMessage response = await _httpClient.PostAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}/restore/id/{userId}", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception) { /* Log error */ }
        return false;
    }
}