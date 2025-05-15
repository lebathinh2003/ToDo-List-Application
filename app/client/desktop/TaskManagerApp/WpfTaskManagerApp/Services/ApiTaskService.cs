using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using WpfTaskManagerApp.Models;
using TaskStatus = WpfTaskManagerApp.Core.TaskStatus;
using System.Net.Http.Json;
using WpfTaskManagerApp.Configs;

namespace WpfTaskManagerApp.Services;
public class ApiTaskService : ITaskService
{
    private readonly HttpClient _httpClient;
    // ***** THAY IAuthenticationService BẰNG ITokenProvider *****
    private readonly ITokenProvider _tokenProvider;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    // ***** CẬP NHẬT CONSTRUCTOR *****
    public ApiTaskService(HttpClient httpClient, ITokenProvider tokenProvider)
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

    public async Task<IEnumerable<TaskItem>> GetAllTasksAsync(string? searchTerm = null, TaskStatus? status = null, bool includeInactive = false)
    {
        await SetAuthHeader();
        var queryParams = new List<string>();
        if (!string.IsNullOrWhiteSpace(searchTerm)) queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
        if (status.HasValue) queryParams.Add($"status={status.Value}");
        if (includeInactive) queryParams.Add("includeInactive=true");

        string requestUri = $"{ApiConfig.BaseUrl}/{ApiConfig.TaskEndPoint}";
        if (queryParams.Any()) requestUri += "?" + string.Join("&", queryParams);

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var tasks = await response.Content.ReadFromJsonAsync<List<TaskItem>>(_jsonSerializerOptions);
                return tasks ?? new List<TaskItem>();
            }
            Debug.WriteLine($"Error fetching all tasks: {response.StatusCode}");
        }
        catch (Exception ex) { Debug.WriteLine($"GetAllTasksAsync error: {ex.Message}"); }
        return new List<TaskItem>();
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByAssigneeAsync(Guid assigneeId, string? searchTerm = null, TaskStatus? status = null, bool includeInactive = false)
    {
        await SetAuthHeader();
        var queryParams = new List<string>();
        if (!string.IsNullOrWhiteSpace(searchTerm)) queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");
        if (status.HasValue) queryParams.Add($"status={status.Value}");
        if (includeInactive) queryParams.Add("includeInactive=true");

        string requestUri = $"{ApiConfig.BaseUrl}/{ApiConfig.TaskEndPoint}/assignee/{assigneeId}";
        if (queryParams.Any()) requestUri += "?" + string.Join("&", queryParams);

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var tasks = await response.Content.ReadFromJsonAsync<List<TaskItem>>(_jsonSerializerOptions);
                return tasks ?? new List<TaskItem>();
            }
            Debug.WriteLine($"Error fetching tasks for assignee {assigneeId}: {response.StatusCode}");
        }
        catch (Exception ex) { Debug.WriteLine($"GetTasksByAssigneeAsync error: {ex.Message}"); }
        return new List<TaskItem>();
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
            Debug.WriteLine($"Error fetching task {taskId}: {response.StatusCode}");
        }
        catch (Exception ex) { Debug.WriteLine($"GetTaskByIdAsync error: {ex.Message}"); }
        return null;
    }

    public async Task<TaskItem?> AddTaskAsync(TaskItem task)
    {
        await SetAuthHeader();
        try
        {
            var taskToAdd = new { task.Title, task.Description, task.AssigneeId, task.Status, task.IsActive, task.DueDate };
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.TaskEndPoint}", taskToAdd, _jsonSerializerOptions);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TaskItem>(_jsonSerializerOptions);
            }
            Debug.WriteLine($"Error adding task: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex) { Debug.WriteLine($"AddTaskAsync error: {ex.Message}"); }
        return null;
    }

    public async Task<bool> UpdateTaskAsync(TaskItem task)
    {
        await SetAuthHeader();
        try
        {
            var taskToUpdate = new { task.Title, task.Description, task.AssigneeId, task.Status, task.IsActive, task.DueDate };
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
            var statusUpdate = new { Status = newStatus };
            HttpResponseMessage response = await _httpClient.PatchAsJsonAsync($"{ApiConfig.BaseUrl}/{ApiConfig.TaskEndPoint}/{taskId}/status", statusUpdate, _jsonSerializerOptions);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex) { Debug.WriteLine($"UpdateTaskStatusAsync error: {ex.Message}"); }
        return false;
    }
}