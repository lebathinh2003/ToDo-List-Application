using Microsoft.AspNetCore.SignalR.Client;
using WpfTaskManagerApp.Configs;
using WpfTaskManagerApp.DTOs;
using WpfTaskManagerApp.Interfaces;
using WpfTaskManagerApp.Models;
using WpfTaskManagerApp.Utils;
namespace WpfTaskManagerApp.Services;

// Service for SignalR real-time communication.
public class SignalRService : ISignalRService
{
    private HubConnection? _hubConnection;
    private readonly ITokenProvider _tokenProvider;
    private readonly string _hubUrl;

    // True if currently connected to the hub.
    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    // Event for new task assignments.
    public event Action<TaskItem>? NewTaskAssigned;
    // Event for task status updates.
    public event Action<TaskItem>? TaskStatusUpdated;
    // Event for forced logout requests.
    public event Action<string?>? ForceLogoutReceived;
    // Event for general reload requests.
    public event Action? ReloadReceived;

    // Constructor.
    public SignalRService(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
        _hubUrl = $"{ApiConfig.BaseUrl}/{ApiConfig.HubEndPoint}"; // Hub URL from config.
        InitializeHubConnection();
    }

    // Initializes the SignalR hub connection and event handlers.
    private void InitializeHubConnection()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl, options =>
            {
                // Provide token for authentication.
                options.AccessTokenProvider = () => Task.FromResult(_tokenProvider.GetToken());
            })
            .WithAutomaticReconnect() // Handles temporary disconnections.
            .Build();

        // Listen for new task assignments.
        _hubConnection.On<SignalRTaskItemDTO>("ReceiveNewTaskAssignment", (newTaskDto) =>
        {
            Application.Current.Dispatcher.Invoke(() => NewTaskAssigned?.Invoke(TaskItemMapper.FromDTO(newTaskDto)));
        });

        // Listen for task updates.
        _hubConnection.On<SignalRTaskItemDTO>("ReceiveTaskUpdate", (updatedTaskDto) =>
        {
            Application.Current.Dispatcher.Invoke(() => TaskStatusUpdated?.Invoke(TaskItemMapper.FromDTO(updatedTaskDto)));
        });

        // Listen for force logout commands.
        _hubConnection.On<string?>("ReceiveForceLogout", (reason) =>
        {
            Application.Current.Dispatcher.Invoke(() => ForceLogoutReceived?.Invoke(reason));
        });

        // Listen for force reload commands.
        _hubConnection.On("ReceiveForceReload", () =>
        {
            Application.Current.Dispatcher.Invoke(() => ReloadReceived?.Invoke());
        });

        // Handle connection closure and attempt to reconnect.
        _hubConnection.Closed += async (error) =>
        {
            await Task.Delay(TimeSpan.FromSeconds(3)); // Wait before reconnecting.
            await ConnectAsync();
        };
    }

    // Connects to the SignalR hub.
    public async Task ConnectAsync()
    {
        if (_hubConnection == null || string.IsNullOrEmpty(_tokenProvider.GetToken()))
        {
            return; // Cannot connect without hub or token.
        }

        if (!IsConnected)
        {
            try
            {
                await _hubConnection.StartAsync();
            }
            catch (Exception) { /* Log connection error. */ }
        }
    }

    // Disconnects from the SignalR hub.
    public async Task DisconnectAsync()
    {
        if (_hubConnection != null && IsConnected)
        {
            try
            {
                await _hubConnection.StopAsync();
            }
            catch (Exception) { /* Log disconnection error. */ }
        }
    }
}