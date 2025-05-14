using MassTransit;
namespace Contract.Event.UserEvent;
[EntityName("DeleteUserEvent")]
public class DeleteUserEvent
{
    public Guid UserId { get; set; }
}
