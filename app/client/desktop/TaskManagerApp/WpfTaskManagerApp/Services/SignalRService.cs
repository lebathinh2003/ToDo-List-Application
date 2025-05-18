using System.Diagnostics;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using WpfTaskManagerApp.Configs;
using WpfTaskManagerApp.Models;

namespace WpfTaskManagerApp.Services;
public class SignalRService : ISignalRService
{
    private HubConnection? _hubConnection;
    private readonly ITokenProvider _tokenProvider;
    private readonly string _hubUrl;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    // Event cho staff khi được gán task mới
    public event Action<TaskItem>? NewTaskAssigned;
    // Event chung khi một task được cập nhật (status, title, etc.)
    public event Action<TaskItem>? TaskStatusUpdated;

    public SignalRService(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
        // ***** SỬ DỤNG URL HUB BẠN CUNG CẤP *****
        _hubUrl = $"{ApiConfig.BaseUrl}/{ApiConfig.HubEndPoint}";
        // ***** KẾT THÚC SỬ DỤNG URL HUB *****
        InitializeHubConnection();
    }

    private void InitializeHubConnection()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(_tokenProvider.GetToken());
            })
            .WithAutomaticReconnect()
            .Build();

        // Lắng nghe sự kiện "ReceiveNewTaskAssignment" từ server
        // Server sẽ gửi TaskItem khi một task mới được gán cho user hiện tại
        _hubConnection.On<TaskItem>("ReceiveNewTaskAssignment", (newTask) =>
        {
            Debug.WriteLine($"SignalR: Received new task assignment: {newTask.Title}");
            Application.Current.Dispatcher.Invoke(() => NewTaskAssigned?.Invoke(newTask));
        });

        // Lắng nghe sự kiện "ReceiveTaskUpdate" từ server
        // Server sẽ gửi TaskItem đã được cập nhật
        _hubConnection.On<TaskItem>("ReceiveTaskUpdate", (updatedTask) =>
        {
            Debug.WriteLine($"SignalR: Received task update: {updatedTask.Title}, Status: {updatedTask.Status}");
            Application.Current.Dispatcher.Invoke(() => TaskStatusUpdated?.Invoke(updatedTask));
        });

        _hubConnection.Closed += async (error) =>
        {
            Debug.WriteLine($"SignalR: Connection closed. Error: {error?.Message}");
            // Có thể thử kết nối lại sau một khoảng thời gian ngắn
            await Task.Delay(10 * 1000);
            await ConnectAsync();
        };
    }
    public async Task ConnectAsync()
    {
        if (_hubConnection == null || string.IsNullOrEmpty(_tokenProvider.GetToken()))
        {
            Debug.WriteLine("SignalR: Cannot connect. HubConnection is null or no token.");
            return;
        }
        if (!IsConnected)
        {
            try
            {
                await _hubConnection.StartAsync();
                Debug.WriteLine("SignalR: Connection established.");
            }
            catch (Exception ex) { Debug.WriteLine($"SignalR connection error: {ex.Message}"); }
        }
        else
        {
            Debug.WriteLine("SignalR: Already connected.");
        }
    }
    public async Task DisconnectAsync()
    {
        if (_hubConnection != null && IsConnected)
        {
            try
            {
                await _hubConnection.StopAsync();
                Debug.WriteLine("SignalR: Connection stopped.");
            }
            catch (Exception ex) { Debug.WriteLine($"SignalR disconnection error: {ex.Message}"); }
        }
    }
}