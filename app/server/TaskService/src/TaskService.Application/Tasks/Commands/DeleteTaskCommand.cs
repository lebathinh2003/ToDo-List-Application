using Contract.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Errors;
using TaskService.Domain.Interfaces;
namespace TaskService.Application.Tasks.Commands;
public record DeleteTaskCommand : IRequest<Result>
{
    public Guid TaskId { get; set; }

}
public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteTaskCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.TaskId == Guid.Empty)
            {
                return Result.Failure(TaskError.NullParameters, "TaskId is null");
            }
            var task = await _context.Tasks.SingleOrDefaultAsync(t => t.Id == request.TaskId);

            if (task == null)
            {
                return Result.Failure(TaskError.NotFound);
            }

            if (task.IsActive == false)
            {
                return Result.Failure(TaskError.TaskAlreadyInactive);
            }

            
            task.IsActive = false;

            _context.Tasks.Update(task);

            await _context.Instance.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(TaskError.DeleteTaskFail, ex.Message);
        }
        
    }
}
