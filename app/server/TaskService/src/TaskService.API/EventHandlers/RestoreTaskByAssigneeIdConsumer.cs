using Contract.Common;
using Contract.Constants;
using Contract.Event.UserEvent;
using MassTransit;
using MediatR;
using TaskService.Application.Tasks.Commands;
namespace TaskService.API.EventHandlers;
[QueueName(RabbitMQConstant.QUEUE.NAME.RESTORE_USER,
exchangeName: RabbitMQConstant.EXCHANGE.NAME.RESTORE_USER)]
public class RestoreTaskByAssigneeIdConsumer : IConsumer<RestoreUserEvent>
{
    private readonly ISender _sender;

    public RestoreTaskByAssigneeIdConsumer(ISender sender)
    {
        _sender = sender;
    }

    public async Task Consume(ConsumeContext<RestoreUserEvent> context)
    {
        var result = await _sender.Send(new RestoreTaskByAssigneeIdCommand
        {
            UserId = context.Message.UserId,
        });
        result.ThrowIfFailure();
    }
}
