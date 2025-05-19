using Contract.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Contract.Services;

public sealed class SignalRService : ISignalRService
{
    public HubConnection HubConnection { get; init; }

    public SignalRService()
    {
        var webSocketUri = "http://localhost:7000/hub-server";
        Console.WriteLine("Connect to chat hub " + webSocketUri);

        HubConnection = new HubConnectionBuilder()
            .WithUrl($"{webSocketUri}")
            .WithAutomaticReconnect(
            [
                TimeSpan.Zero,
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(30)
            ])
            .ConfigureLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .Build();
    }

    public async Task StartConnectionAsync()
    {
        Console.WriteLine("Connecting to signalR");
        await HubConnection.StartAsync();
        Console.WriteLine("SignalR connected");
    }

    public async Task StopConnectionAsync()
    {
        await HubConnection.StopAsync();
        Console.WriteLine("SignalR disconnected");
    }
    public async Task InvokeAction<T>(string action, T obj)
    {
        Console.WriteLine($"Begin to invoke {action} with DTO: ${JsonConvert.SerializeObject(obj)}");
        await HubConnection.InvokeAsync(action, obj);
        Console.WriteLine($"Done invoking action");
    }

    public async Task InvokeAction(string action)
    {
        await HubConnection.InvokeAsync(action);
    }
}
