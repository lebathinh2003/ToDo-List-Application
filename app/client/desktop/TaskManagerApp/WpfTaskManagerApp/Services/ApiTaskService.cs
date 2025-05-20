using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using WpfTaskManagerApp.Configs;
using WpfTaskManagerApp.Core;
using WpfTaskManagerApp.Interfaces;
using WpfTaskManagerApp.Models;
using TaskStatus = WpfTaskManagerApp.Core.TaskStatus;

namespace WpfTaskManagerApp.Services;

// API service for task operations.
public class ApiTaskService : ITaskService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenProvider _tokenProvider;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    // Constructor.
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

    // Sets authorization header.
    private async Task SetAuthHeader()
    {
        var token = _tokenProvider.GetToken();
        _httpClient.DefaultRequestHeaders.Authorization = !string.IsNullOrEmpty(token)
            ? new AuthenticationHeaderValue("Bearer", token)
            : null;
        await Task.CompletedTask;
    }

    // Gets all tasks with filters.
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
        if (status?.Status != null) queryParams.Add($"status={status.Status.ToString()}"); // Check Status property
        if (includeInactive) queryParams.Add("includeInactive=true");

        string requestUri = $"{ApiConfig.BaseUrl}/{ApiConfig.TaskEndPoint}";
        if (queryParams.Any()) requestUri += "?" + string.Join("&", queryParams);

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PaginatedResult<TaskItem>>(_jsonSerializerOptions);
            }
        }
        catch (Exception)
        { /* Log error */
        }
        return null;
    }

    // Gets tasks by assignee with filters.
    public async Task<PaginatedResult<TaskItem>?> GetTasksByAssigneeAsync(
        Guid assigneeId,
        int skip = 0,
        int limit = 10,
        string? sortBy = null,
        string? sortOrder = "asc",
        string? keyword = null,
        TaskStatusItem? status = null)
    {
        await SetAuthHeader();
        var queryParams = new List<string> { $"skip={skip}", $"limit={limit}" };
        if (!string.IsNullOrWhiteSpace(sortBy)) queryParams.Add($"sortBy={Uri.EscapeDataString(sortBy)}");
        if (!string.IsNullOrWhiteSpace(sortOrder)) queryParams.Add($"sortOrder={Uri.EscapeDataString(sortOrder)}");
        if (!string.IsNullOrWhiteSpace(keyword)) queryParams.Add($"keyword={Uri.EscapeDataString(keyword)}");
        if (status?.Status != null) queryParams.Add($"status={status.Status.ToString()}"); // Check Status property
        queryParams.Add("includeInactive=false");

        string requestUri = $"{ApiConfig.BaseUrl}/{ApiConfig.TaskEndPoint}/assignee/id/{assigneeId}";
        if (queryParams.Any()) requestUri += "?" + string.Join("&", queryParams);

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PaginatedResult<TaskItem>>(_jsonSerializerOptions);
            }
        }
        catch (Exception)
        { /* Log error */
        }
        return null;
    }

    // Gets a task by ID.
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
        catch (Exception)
        { /* Log error */
        }
        return null;
    }

    // Adds a new task.
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
        catch (Exception)
        { /* Log error */
        }
        return null;
    }

    // Updates an existing task.
    public async Task<bool> UpdateTaskAsync(TaskItem task)
    {
        await SetAuthHeader();
        try
        {
            var taskToUpdate = new { task.Title, task.Description, task.AssigneeId, Status = task.Status.ToString(), task.IsActive, task.DueDate };
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.TaskEndPoint}/id/{task.Id}", taskToUpdate, _jsonSerializerOptions);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        { /* Log error */
        }
        return false;
    }

    // Deletes a task.
    public async Task<bool> DeleteTaskAsync(Guid taskId)
    {
        await SetAuthHeader();
        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{ApiConfig.BaseUrl}/{ApiConfig.TaskEndPoint}/delete/id/{taskId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        { /* Log error */
        }
        return false;
    }

    // Restores a task.
    public async Task<bool> RestoreTaskAsync(Guid taskId)
    {
        await SetAuthHeader();
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsync($"{ApiConfig.BaseUrl}/{ApiConfig.TaskEndPoint}/restore/id/{taskId}", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        { /* Log error */
        }
        return false;
    }

    // Updates task status.
    public async Task<bool> UpdateTaskStatusAsync(Guid taskId, TaskStatus newStatus)
    {
        await SetAuthHeader();
        try
        {
            var statusUpdate = new { status = newStatus.ToString() };
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.TaskEndPoint}/status/id/{taskId}", statusUpdate, _jsonSerializerOptions);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        { /* Log error */
        }
        return false;
    }
}