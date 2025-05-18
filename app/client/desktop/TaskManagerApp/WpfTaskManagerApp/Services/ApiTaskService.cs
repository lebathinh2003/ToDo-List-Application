using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using WpfTaskManagerApp.Models;
using TaskStatus = WpfTaskManagerApp.Core.TaskStatus;
using System.Net.Http.Json;
using WpfTaskManagerApp.Configs;
using WpfTaskManagerApp.Core;

namespace WpfTaskManagerApp.Services;
public class ApiTaskService : ITaskService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenProvider _tokenProvider;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ApiTaskService(HttpClient httpClient, ITokenProvider tokenProvider)
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

    // ***** CẬP NHẬT TRIỂN KHAI GetAllTasksAsync *****
    public async Task<PaginatedResult<TaskItem>?> GetAllTasksAsync(
        int skip = 0,
        int limit = 10,
        string? sortBy = null,
        string? sortOrder = "asc",
        string? keyword = null,
        TaskStatusItem? status = null,
        bool includeInactive = false)
    {
        await SetAuthHeader();
        var queryParams = new List<string> { $"skip={skip}", $"limit={limit}" };
        if (!string.IsNullOrWhiteSpace(sortBy)) queryParams.Add($"sortBy={Uri.EscapeDataString(sortBy)}");
        if (!string.IsNullOrWhiteSpace(sortOrder)) queryParams.Add($"sortOrder={Uri.EscapeDataString(sortOrder)}");
        if (!string.IsNullOrWhiteSpace(keyword)) queryParams.Add($"keyword={Uri.EscapeDataString(keyword)}");
        if (status != null) queryParams.Add($"status={status.Status.ToString()}");
        if (includeInactive) queryParams.Add("includeInactive=true");

        string requestUri = $"{ApiConfig.BaseUrl}/{ApiConfig.TaskEndPoint}";
        if (queryParams.Any()) requestUri += "?" + string.Join("&", queryParams);

        Debug.WriteLine($"ApiTaskService.GetAllTasksAsync: Requesting URL: {requestUri}");
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PaginatedResult<TaskItem>>(_jsonSerializerOptions);
            }
            Debug.WriteLine($"Error fetching all tasks: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex) { Debug.WriteLine($"GetAllTasksAsync error: {ex.Message}"); }
        return null;
    }

    // ***** CẬP NHẬT TRIỂN KHAI GetTasksByAssigneeAsync *****
    public async Task<PaginatedResult<TaskItem>?> GetTasksByAssigneeAsync(
        Guid assigneeId,
        int skip = 0,
        int limit = 10,
        string? sortBy = null,
        string? sortOrder = "asc",
        string? keyword = null,
        TaskStatusItem? status = null,
        bool includeInactive = false)
    {
        await SetAuthHeader();
        var queryParams = new List<string> { $"skip={skip}", $"limit={limit}" };
        if (!string.IsNullOrWhiteSpace(sortBy)) queryParams.Add($"sortBy={Uri.EscapeDataString(sortBy)}");
        if (!string.IsNullOrWhiteSpace(sortOrder)) queryParams.Add($"sortOrder={Uri.EscapeDataString(sortOrder)}");
        if (!string.IsNullOrWhiteSpace(keyword)) queryParams.Add($"keyword={Uri.EscapeDataString(keyword)}");
        if (status != null) queryParams.Add($"status={status.Status.ToString()}");
        if (includeInactive) queryParams.Add("includeInactive=true"); // API cần hỗ trợ includeInactive cho tasks của assignee

        string requestUri = $"{ApiConfig.BaseUrl}/{ApiConfig.TaskEndPoint}/assignee/{assigneeId}";
        if (queryParams.Any()) requestUri += "?" + string.Join("&", queryParams);

        Debug.WriteLine($"ApiTaskService.GetTasksByAssigneeAsync: Requesting URL: {requestUri}");
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PaginatedResult<TaskItem>>(_jsonSerializerOptions);
            }
            Debug.WriteLine($"Error fetching tasks for assignee {assigneeId}: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex) { Debug.WriteLine($"GetTasksByAssigneeAsync error: {ex.Message}"); }
        return null;
    }

    public async Task<TaskItem?> GetTaskByIdAsync(Guid taskId)
    {
        await SetAuthHeader();
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{ApiConfig.BaseUrl}/{ApiConfig.TaskEndPoint}/{taskId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TaskItem>(_jsonSerializerOptions);
            }
        }
        catch (Exception ex) { Debug.WriteLine($"GetTaskByIdAsync error: {ex.Message}"); }
        return null;
    }

    public async Task<TaskItem?> AddTaskAsync(TaskItem task)
    {
        await SetAuthHeader();
        try
        {
            var taskToAdd = new { task.Title, task.Description, task.AssigneeId, Status = task.Status.ToString(), task.IsActive, task.DueDate };
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.TaskEndPoint}", taskToAdd, _jsonSerializerOptions);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TaskItem>(_jsonSerializerOptions);
            }
        }
        catch (Exception ex) { Debug.WriteLine($"AddTaskAsync error: {ex.Message}"); }
        return null;
    }

    public async Task<bool> UpdateTaskAsync(TaskItem task)
    {
        await SetAuthHeader();
        try
        {
            var taskToUpdate = new { task.Title, task.Description, task.AssigneeId, Status = task.Status.ToString(), task.IsActive, task.DueDate };
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.TaskEndPoint}/{task.Id}", taskToUpdate, _jsonSerializerOptions);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) { Debug.WriteLine($"UpdateTaskAsync error: {ex.Message}"); }
        return false;
    }

    public async Task<bool> DeleteTaskAsync(Guid taskId)
    {
        await SetAuthHeader();
        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{ApiConfig.BaseUrl}/{ApiConfig.TaskEndPoint}/{taskId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) { Debug.WriteLine($"DeleteTaskAsync error: {ex.Message}"); }
        return false;
    }

    public async Task<bool> UpdateTaskStatusAsync(Guid taskId, TaskStatus newStatus)
    {
        await SetAuthHeader();
        try
        {
            var statusUpdate = new { Status = newStatus.ToString() };
            HttpResponseMessage response = await _httpClient.PatchAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.TaskEndPoint}/{taskId}/status", statusUpdate, _jsonSerializerOptions);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) { Debug.WriteLine($"UpdateTaskStatusAsync error: {ex.Message}"); }
        return false;
    }
}