using Contract.DTOs.SignalRDTOs;
using SignalRHub.DTOs;

namespace SignalRHub.Hubs;

public interface IHubClient
{
    Task Connected(IEnumerable<Guid>? userIdOnlineList);
    Task Disconnected(Guid userDisconnectedId);
    Task ReceiveTest();
    Task ReceiveTest(string response);
    Task ReceiveTest(TestObject obj);
    Task ReceiveForceLogout(string? message);
    Task ReceiveNewTaskAssignment(SignalRTaskItemDTO taskItem);
    Task ReceiveTaskUpdate(SignalRTaskItemDTO taskItem);
    Task ReceiveForceReload();
}
