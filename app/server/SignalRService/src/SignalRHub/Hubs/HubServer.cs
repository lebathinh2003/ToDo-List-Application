using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http.Extensions;
using SignalRHub.DTOs;
using Contract.Interfaces;
using SignalRHub.Interfaces;
using SignalRHub.Constants;
using System.Security.Claims;
using Contract.DTOs.SignalRDTOs;

namespace SignalRHub.Hubs;

public class HubServer : Hub<IHubClient>
{
    //Use the dictionary to map the userId and userConnectionId
    private static readonly ConcurrentDictionary<string, Guid> UserConnectionMap = new();
    private readonly IHttpContextAccessor _httpContextAccessor;
    // rabbitmq
    private readonly IServiceBus _bus;
    private readonly IMemoryTracker _memoryTracker;
    public HubServer(IHttpContextAccessor httpContextAccessor,
                     IServiceBus bus,
                     IMemoryTracker memoryTracker)
    {
        _httpContextAccessor = httpContextAccessor;
        _bus = bus;
        _memoryTracker = memoryTracker;
    }

    // The url would be like "https://yourhubURL:port?userId=abc&access_token=abc"
    public override async Task OnConnectedAsync()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            Console.WriteLine($"User {userId} connected");
            await ConnectWithUserIdAsync(userId);
        }
        else
        {
            var RequestUrl = Context.GetHttpContext()?.Request.GetDisplayUrl() ?? "Unknown";
            Console.WriteLine($"Service with url {RequestUrl} has connected to signalR successfully!");
        }
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Disconnecting from signalR!");
        if (UserConnectionMap.TryRemove(Context.ConnectionId, out Guid userDisconnectedId))
        {
            Console.WriteLine($"Connection {Context.ConnectionId} disconnected and removed from UserConnectionMap.");
            await Clients.All.Disconnected(userDisconnectedId);
            if (Context.User != null)
            {
                var roleType = Context.User.Claims.FirstOrDefault(c => c.Type == "role");
                if (roleType != null && roleType.Value == "Staff")
                {
                    _memoryTracker.UserDisconnected(userDisconnectedId.ToString());
                }
            }
        }
        else
        {
            Console.WriteLine($"Connection {Context.ConnectionId} disconnected, but it was not found in UserConnectionMap.");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task TestEvent()
    {
        await Clients.All.ReceiveTest();
    }

    public async Task TestEventWithParams(string text)
    {
        await Clients.All.ReceiveTest(text);
    }

    public async Task TestEventWithObjectParams(TestObject obj)
    {
        await Clients.All.ReceiveTest(obj);
    }

    public async Task PushNewTaskNotification(SignalRTaskItemWithRecipentsDTO taskItemAndRecipents)
    {
        var receiverConnectionIds = UserConnectionMap.Where(pair => taskItemAndRecipents.Recipients.Contains(pair.Value)).Select(pair => pair.Key).ToList();
        var excludedConnectionIds = UserConnectionMap.Where(pair => taskItemAndRecipents.ExcludeRecipients.Contains(pair.Value)).Select(pair => pair.Key).ToList();

        var tasks = receiverConnectionIds
            .Select(receiverConnectionId => Clients.Client(receiverConnectionId).ReceiveNewTaskAssignment(taskItemAndRecipents.TaskItem))
            .ToArray();

        Console.WriteLine($"Send task to all Admin except admin who invoke action");
        await Clients.GroupExcept(ROLE_BASED_GROUP.ADMIN, excludedConnectionIds).ReceiveNewTaskAssignment(taskItemAndRecipents.TaskItem);

        foreach (var receiverConnectionId in receiverConnectionIds)
        {
            Console.WriteLine($"Send task to {receiverConnectionId}");
        }

        await Task.WhenAll(tasks);
    }

    public async Task PushTaskUpdateNotification(SignalRTaskItemWithRecipentsDTO taskItemAndRecipents)
    {
        var receiverConnectionIds = UserConnectionMap.Where(pair => taskItemAndRecipents.Recipients.Contains(pair.Value)).Select(pair => pair.Key).ToList();
        var excludedConnectionIds = UserConnectionMap.Where(pair => taskItemAndRecipents.ExcludeRecipients.Contains(pair.Value)).Select(pair => pair.Key).ToList();

        var tasks = receiverConnectionIds
            .Select(receiverConnectionId => Clients.Client(receiverConnectionId).ReceiveTaskUpdate(taskItemAndRecipents.TaskItem))
            .ToArray();

        Console.WriteLine($"Send task to all Admin except admin who invoke action");
        await Clients.GroupExcept(ROLE_BASED_GROUP.ADMIN, excludedConnectionIds).ReceiveTaskUpdate(taskItemAndRecipents.TaskItem);

        foreach (var receiverConnectionId in receiverConnectionIds)
        {
            Console.WriteLine($"Send task to {receiverConnectionId}");
        }

        await Task.WhenAll(tasks);
    }

    public async Task TriggerLogout(Guid userId) {
        var receiverConnectionId = UserConnectionMap.SingleOrDefault(pair => pair.Value == userId).Key;

        if (receiverConnectionId == null)
        {
            return;
        }
        await Clients.Client(receiverConnectionId).ReceiveForceLogout("Your account was banned!");
        Console.WriteLine($"Send task to {receiverConnectionId}");
    }

    public async Task TriggerReload(RecipentsDTO recipents)
    {
        var receiverConnectionIds = UserConnectionMap.Where(pair => recipents.Recipients.Contains(pair.Value)).Select(pair => pair.Key).ToList();
        var excludedConnectionIds = UserConnectionMap.Where(pair => recipents.ExcludeRecipients.Contains(pair.Value)).Select(pair => pair.Key).ToList();

        var tasks = receiverConnectionIds
            .Select(receiverConnectionId => Clients.Client(receiverConnectionId).ReceiveForceReload())
            .ToArray();

        Console.WriteLine($"Send task to all Admin except admin who invoke action");
        await Clients.GroupExcept(ROLE_BASED_GROUP.ADMIN, excludedConnectionIds).ReceiveForceReload();

        foreach (var receiverConnectionId in receiverConnectionIds)
        {
            Console.WriteLine($"Send task to {receiverConnectionId}");
        }

        await Task.WhenAll(tasks);
    }

    #region Helper method
    private async Task ConnectWithUserIdAsync(Guid userId)
    {
        UserConnectionMap[Context.ConnectionId] = userId;
        Console.WriteLine($"Map user complete with {Context.ConnectionId} and {userId}");
        Console.WriteLine(userId + " Connected");
        // Admin user
        if (Context.User != null)
        {
            var roleClaim = Context.User?.FindFirst(ClaimTypes.Role);
            if (roleClaim != null && (roleClaim.Value == ROLE_BASED_GROUP.ADMIN))
            {
                Console.WriteLine("Add admin to group Admin");
                await Groups.AddToGroupAsync(Context.ConnectionId, ROLE_BASED_GROUP.ADMIN);
                return;
            }

            if (roleClaim != null && (roleClaim.Value == ROLE_BASED_GROUP.STAFF))
            {
                Console.WriteLine("Add staff to group Staff");
                await Groups.AddToGroupAsync(Context.ConnectionId, ROLE_BASED_GROUP.STAFF);
                return;
            }
        }

        foreach (var key in UserConnectionMap.Keys)
        {
            Console.WriteLine($"{key}: {UserConnectionMap[key]}");
        }
        var userIdOnlineList = UserConnectionMap.Select(uc => uc.Value);
        await Clients.All.Connected(userIdOnlineList);
    }
    #endregion
}
