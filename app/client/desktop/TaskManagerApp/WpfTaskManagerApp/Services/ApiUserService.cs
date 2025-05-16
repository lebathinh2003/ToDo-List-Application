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
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
        await Task.CompletedTask;
    }

    // ***** CẬP NHẬT TRIỂN KHAI GetUsersAsync *****
    public async Task<PaginatedResult<User>?> GetUsersAsync(
        int skip = 0,
        int limit = 10,
        string? sortBy = null,
        string? sortOrder = "asc",
        string? keyword = null,
        bool includeInactive = false)
    {
        await SetAuthHeader();
        var queryParams = new List<string>
            {
                $"skip={skip}",
                $"limit={limit}"
            };

        if (!string.IsNullOrWhiteSpace(sortBy)) queryParams.Add($"sortBy={Uri.EscapeDataString(sortBy)}");
        if (!string.IsNullOrWhiteSpace(sortOrder)) queryParams.Add($"sortOrder={Uri.EscapeDataString(sortOrder)}");
        if (!string.IsNullOrWhiteSpace(keyword)) queryParams.Add($"keyword={Uri.EscapeDataString(keyword)}");
        if (includeInactive) queryParams.Add("includeInactive=true"); // Giả sử API có tham số này

        string requestUri = $"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}";
        if (queryParams.Any()) requestUri += "?" + string.Join("&", queryParams);

        Debug.WriteLine($"ApiUserService.GetUsersAsync: Requesting URL: {requestUri}");

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PaginatedResult<User>>(_jsonSerializerOptions);
                if (result?.Metadata != null)
                {
                    Debug.WriteLine($"ApiUserService.GetUsersAsync: Received {result.PaginatedData?.Count} users. TotalRows: {result.Metadata.TotalRow}, TotalPages: {result.Metadata.TotalPage}");
                }
                return result;
            }
            Debug.WriteLine($"Error fetching users: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex) { Debug.WriteLine($"GetUsersAsync error: {ex.Message}"); }
        return null; // Trả về null nếu có lỗi hoặc không thành công
    }
    // ***** KẾT THÚC CẬP NHẬT TRIỂN KHAI GetUsersAsync *****


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
        catch (Exception ex) { /* ... logging ... */ }
        return null;
    }

    public async Task<User?> GetUserByUsernameAsync(string username) { /* ... */ return null; }
    public async Task<User?> AddUserAsync(User user, string password) { /* ... */ return null; }
    public async Task<bool> UpdateUserAsync(User user) { /* ... */ return false; }

    public async Task<bool> DeleteUserAsync(Guid userId) // Đây sẽ là soft delete (set IsActive = false)
    {
        await SetAuthHeader();
        try
        {
            // API của bạn có thể là DELETE /users/{userId} (thực hiện soft delete ở backend)
            // Hoặc một PATCH/PUT request để cập nhật IsActive
            // Ví dụ với PATCH:
            // var patchDoc = new[] { new { op = "replace", path = "/isActive", value = false } };
            // HttpResponseMessage response = await _httpClient.PatchAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}/{userId}", patchDoc);
            // Nếu API Delete thực hiện soft delete:
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}/delete/id/{userId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) { Debug.WriteLine($"DeleteUserAsync (soft delete) error: {ex.Message}"); }
        return false;
    }

    // ***** THÊM PHƯƠNG THỨC RestoreUserAsync *****
    public async Task<bool> RestoreUserAsync(Guid userId)
    {
        await SetAuthHeader();
        try
        {
            // Giả sử API có một endpoint riêng để restore, hoặc dùng PATCH để set IsActive = true
            // Ví dụ với PATCH:
            //var patchDoc = new[] { new { op = "replace", path = "/isActive", value = true } };
            //HttpResponseMessage response = await _httpClient.PatchAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}/{userId}/restore", patchDoc);
            // Hoặc nếu endpoint là /users/{userId}/restore và body rỗng:
            HttpResponseMessage response = await _httpClient.PostAsync($"{ApiConfig.BaseUrl}/{ApiConfig.UserEndPoint}/restore/id/{userId}", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) { Debug.WriteLine($"RestoreUserAsync error: {ex.Message}"); }
        return false;
    }
}