using Contract.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Errors;
using TaskService.Domain.Interfaces;
using TaskStatus = TaskService.Domain.Models.TaskStatus;
namespace TaskService.Application.Tasks.Commands;
public record UpdateTaskStatusCommand : IRequest<Result<bool?>>
{
    public Guid Id { get; set; }
    public string Status { get; set; } = null!;
}

public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, Result<bool?>>
{
    private readonly IApplicationDbContext _context;

    public UpdateTaskStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool?>> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        try {

            if(request.Id == Guid.Empty ||
                string.IsNullOrEmpty(request.Status) ||
                !Enum.TryParse<TaskStatus>(request.Status, out var status))
            {
                return Result<bool?>.Failure(TaskError.NullParameters, "Invalid input data");
            }
              
            var task = await _context.Tasks.SingleOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (task == null)
            {
                return Result<bool?>.Failure(TaskError.NotFound, "Task not found");
            }

            task.Status = status;
            task.UpdatedAt = DateTime.UtcNow;
            _context.Tasks.Update(task);
            await _context.Instance.SaveChangesAsync(cancellationToken);
            return Result<bool?>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool?>.Failure(TaskError.AddTaskFail, ex.Message);
        }
   
    }
}
