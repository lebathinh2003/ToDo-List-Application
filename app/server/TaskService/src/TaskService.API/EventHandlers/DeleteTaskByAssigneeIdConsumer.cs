using Contract.Common;
using Contract.Constants;
using Contract.Event.UserEvent;
using MassTransit;
using MediatR;
using TaskService.Application.Tasks.Commands;
namespace TaskService.API.EventHandlers;
[QueueName(RabbitMQConstant.QUEUE.NAME.DELETE_USER,
exchangeName: RabbitMQConstant.EXCHANGE.NAME.DELETE_USER)]
public class DeleteTaskByAssigneeIdConsumer : IConsumer<DeleteUserEvent>
{
    private readonly ISender _sender;

    public DeleteTaskByAssigneeIdConsumer(ISender sender)
    {
        _sender = sender;
    }

    public async Task Consume(ConsumeContext<DeleteUserEvent> context)
    {
        var result = await _sender.Send(new DeleteTaskByAssigneeIdCommand
        {
            UserId = context.Message.UserId,
        });
        result.ThrowIfFailure();
    }
}
