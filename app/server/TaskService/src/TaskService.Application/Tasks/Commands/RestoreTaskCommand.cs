﻿using Contract.Common;
using Contract.Constants;
using Contract.DTOs.SignalRDTOs;
using Contract.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Errors;
using TaskService.Domain.Interfaces;
namespace TaskService.Application.Tasks.Commands;
public record RestoreTaskCommand : IRequest<Result>
{
    public Guid TaskId { get; set; }
    public Guid AdminId { get; set; }

}
public class RestoreTaskCommandHandler : IRequestHandler<RestoreTaskCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ISignalRService _signalRService;
    public RestoreTaskCommandHandler(IApplicationDbContext context, ISignalRService signalRService)
    {
        _context = context;
        _signalRService = signalRService;
    }

    public async Task<Result> Handle(RestoreTaskCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.TaskId == Guid.Empty)
            {
                return Result.Failure(TaskError.NullParameters, "TaskId or AdminId is null");
            }
            var task = await _context.Tasks.SingleOrDefaultAsync(t => t.Id == request.TaskId);

            if (task == null)
            {
                return Result.Failure(TaskError.NotFound);
            }

            if (task.IsActive == true)
            {
                return Result.Failure(TaskError.TaskAlreadyActive);
            }

            task.IsActive = true;

            _context.Tasks.Update(task);
            await _context.Instance.SaveChangesAsync(cancellationToken);

            await _signalRService.InvokeAction(SignalRAction.TriggerReload.ToString(), new RecipentsDTO
            {
                ExcludeRecipients = new List<Guid> { request.AdminId },
                Recipients = new List<Guid> { task.AssigneeId },
            });

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(TaskError.RestoreTaskFail, ex.Message);
        }

    }
}
