using System.Diagnostics;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using WpfTaskManagerApp.Configs;
using WpfTaskManagerApp.Models;

namespace WpfTaskManagerApp.Services;
public class SignalRService : ISignalRService
{
    private HubConnection? _hubConnection;
    // ***** THAY IAuthenticationService BẰNG ITokenProvider *****
    private readonly ITokenProvider _tokenProvider;
    private readonly string _hubUrl;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public event Action<TaskItem>? NewTaskAssigned;
    public event Action<TaskItem>? TaskStatusUpdated;

    // ***** CẬP NHẬT CONSTRUCTOR ĐỂ NHẬN ITokenProvider *****
    public SignalRService(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider; // ***** LƯU TRỮ ITokenProvider *****
        _hubUrl = $"{ApiConfig.BaseUrl.Replace("/api", "")}/yourTaskHub";

        InitializeHubConnection();
    }

    private void InitializeHubConnection()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl, options =>
            {
                // ***** SỬ DỤNG _tokenProvider ĐỂ LẤY TOKEN *****
                options.AccessTokenProvider = () => Task.FromResult(_tokenProvider.GetToken());
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<TaskItem>("ReceiveNewTaskAssignment", (newTask) =>
        {
            Debug.WriteLine($"SignalR: New task assigned - ID: {newTask.Id}, Title: {newTask.Title}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                NewTaskAssigned?.Invoke(newTask);
            });
        });

        _hubConnection.On<TaskItem>("ReceiveTaskUpdate", (updatedTask) =>
        {
            Debug.WriteLine($"SignalR: Task updated - ID: {updatedTask.Id}, Status: {updatedTask.Status}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                TaskStatusUpdated?.Invoke(updatedTask);
            });
        });

        _hubConnection.Closed += async (error) =>
        {
            Debug.WriteLine($"SignalR connection closed: {error?.Message}");
            // await Task.Delay(new Random().Next(0, 5) * 1000);
            // await ConnectAsync(); 
        };

        _hubConnection.Reconnecting += (error) =>
        {
            Debug.WriteLine($"SignalR connection reconnecting: {error?.Message}");
            return Task.CompletedTask;
        };

        _hubConnection.Reconnected += (connectionId) =>
        {
            Debug.WriteLine($"SignalR connection reconnected with ID: {connectionId}");
            return Task.CompletedTask;
        };
    }


    public async Task ConnectAsync()
    {
        if (_hubConnection == null)
        {
            Debug.WriteLine("SignalR: HubConnection is not initialized.");
            return;
        }

        if (!IsConnected)
        {
            // ***** SỬ DỤNG _tokenProvider ĐỂ KIỂM TRA TOKEN *****
            if (string.IsNullOrEmpty(_tokenProvider.GetToken()))
            {
                Debug.WriteLine("SignalR: Cannot connect. No authentication token available from TokenProvider.");
                return;
            }

            try
            {
                Debug.WriteLine($"SignalR: Attempting to connect to {_hubUrl}...");
                await _hubConnection.StartAsync();
                Debug.WriteLine("SignalR: Connection established.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SignalR connection error: {ex.Message}");
            }
        }
    }

    public async Task DisconnectAsync()
    {
        if (_hubConnection == null) return;

        if (IsConnected)
        {
            try
            {
                await _hubConnection.StopAsync();
                Debug.WriteLine("SignalR: Connection stopped.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SignalR disconnection error: {ex.Message}");
            }
        }
    }
}