using Contract.Common;
using Contract.Constants;
using Contract.DTOs.SignalRDTOs;
using Contract.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Errors;
using TaskService.Domain.Interfaces;
namespace TaskService.Application.Tasks.Commands;
public record RestoreTaskByAssigneeIdCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public Guid AdminId { get; set; }
}
public class RestoreTaskByAssigneeIdCommandHandler : IRequestHandler<RestoreTaskByAssigneeIdCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ISignalRService _signalRService;

    public RestoreTaskByAssigneeIdCommandHandler(IApplicationDbContext context, ISignalRService signalRService)
    {
        _context = context;
        _signalRService = signalRService;
    }

    public async Task<Result> Handle(RestoreTaskByAssigneeIdCommand request, CancellationToken cancellationToken)
    {
        try {
            var tasks = await _context.Tasks.Where(x => x.AssigneeId == request.UserId).ToListAsync(cancellationToken);

            if (tasks != null && tasks.Count != 0)
            {
                foreach (var task in tasks)
                {
                    task.IsActive = true;
                }
                _context.Tasks.UpdateRange(tasks);
                await _context.Instance.SaveChangesAsync();

                await _signalRService.InvokeAction(SignalRAction.TriggerReload.ToString(), new RecipentsDTO
                {
                    ExcludeRecipients = new List<Guid> { request.AdminId },
                    Recipients = new List<Guid> { request.UserId }
                });
            }
            return Result.Success();

        }
        catch (Exception ex)
        {
            return Result.Failure(TaskError.RestoreTaskFail, ex.Message);
        }
    }
}
