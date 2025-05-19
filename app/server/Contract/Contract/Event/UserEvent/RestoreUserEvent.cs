using MassTransit;
namespace Contract.Event.UserEvent;
[EntityName("RestoreUserEvent")]
public class RestoreUserEvent
{
    public Guid UserId { get; set; }
}
