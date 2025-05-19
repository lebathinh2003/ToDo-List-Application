using System.Diagnostics;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using WpfTaskManagerApp.Configs;
using WpfTaskManagerApp.DTOs;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.Utils;

namespace WpfTaskManagerApp.Services;
public class SignalRService : ISignalRService
{
    private HubConnection? _hubConnection;
    private readonly ITokenProvider _tokenProvider;
    private readonly string _hubUrl;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public event Action<TaskItem>? NewTaskAssigned;
    public event Action<TaskItem>? TaskStatusUpdated;
    public event Action<string?>? ForceLogoutReceived;
    public event Action? ReloadReceived;


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
        _hubConnection.On<SignalRTaskItemDTO>("ReceiveNewTaskAssignment", (newTask) =>
        {
            Debug.WriteLine($"SignalR: Received new task assignment: {newTask.Title}");
            Application.Current.Dispatcher.Invoke(() => NewTaskAssigned?.Invoke(TaskItemMapper.FromDTO(newTask)));
        });

        // Lắng nghe sự kiện "ReceiveTaskUpdate" từ server
        // Server sẽ gửi TaskItem đã được cập nhật
        _hubConnection.On<SignalRTaskItemDTO>("ReceiveTaskUpdate", (updatedTask) =>
        {
            Debug.WriteLine($"SignalR: Received task update: {updatedTask.Title}, Status: {updatedTask.Status}");
            Application.Current.Dispatcher.Invoke(() => TaskStatusUpdated?.Invoke(TaskItemMapper.FromDTO(updatedTask)));
        });

        _hubConnection.On<string?>("ReceiveForceLogout", (reason) =>
        {
            Debug.WriteLine($"SignalR: Received ForceLogout. Reason: {reason ?? "No reason provided."}");
            Application.Current.Dispatcher.Invoke(() => ForceLogoutReceived?.Invoke(reason));
        });

        _hubConnection.On("ReceiveForceReload", () =>
        {
            Debug.WriteLine($"SignalR: Received ReceiveForceReload.");
            Application.Current.Dispatcher.Invoke(() => ReloadReceived?.Invoke());
        });

        _hubConnection.Closed += async (error) =>
        {
            Debug.WriteLine($"SignalR: Connection closed. Error: {error?.Message}");
            await Task.Delay(3 * 1000);
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