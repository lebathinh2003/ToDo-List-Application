using Contract.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Errors;
using TaskService.Domain.Interfaces;
namespace TaskService.Application.Tasks.Commands;
public record DeleteTaskByAssigneeIdCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
}
public class DeleteTaskByAssigneeIdCommandHandler : IRequestHandler<DeleteTaskByAssigneeIdCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteTaskByAssigneeIdCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteTaskByAssigneeIdCommand request, CancellationToken cancellationToken)
    {
        try {
            var tasks = await _context.Tasks.Where(x => x.AssigneeId == request.UserId).ToListAsync(cancellationToken);

            if (tasks != null && tasks.Count != 0)
            {
                foreach (var task in tasks)
                {
                    task.IsActive = false;
                }
                _context.Tasks.UpdateRange(tasks);
                await _context.Instance.SaveChangesAsync();
            }
            return Result.Success();

        }
        catch (Exception ex)
        {
            return Result.Failure(TaskError.DeleteTaskFail, ex.Message);
        }
    }
}
