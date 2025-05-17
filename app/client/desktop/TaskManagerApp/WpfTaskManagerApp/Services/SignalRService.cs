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
    public event Action<TaskItem>? NewTaskAssigned;
    public event Action<TaskItem>? TaskStatusUpdated;

    public SignalRService(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
        _hubUrl = $"{ApiConfig.BaseUrl.Replace("/api", "")}/taskHub"; // Giả sử hub tên là "taskHub"
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

        _hubConnection.On<TaskItem>("ReceiveNewTaskAssignment", (newTask) =>
        {
            Application.Current.Dispatcher.Invoke(() => NewTaskAssigned?.Invoke(newTask));
        });
        _hubConnection.On<TaskItem>("ReceiveTaskUpdate", (updatedTask) =>
        {
            Application.Current.Dispatcher.Invoke(() => TaskStatusUpdated?.Invoke(updatedTask));
        });
        // ... (các event handler khác của HubConnection) ...
    }
    public async Task ConnectAsync()
    {
        if (_hubConnection == null || string.IsNullOrEmpty(_tokenProvider.GetToken())) return;
        if (!IsConnected)
        {
            try { await _hubConnection.StartAsync(); Debug.WriteLine("SignalR: Connection established."); }
            catch (Exception ex) { Debug.WriteLine($"SignalR connection error: {ex.Message}"); }
        }
    }
    public async Task DisconnectAsync()
    {
        if (_hubConnection != null && IsConnected)
        {
            try { await _hubConnection.StopAsync(); Debug.WriteLine("SignalR: Connection stopped."); }
            catch (Exception ex) { Debug.WriteLine($"SignalR disconnection error: {ex.Message}"); }
        }
    }
}