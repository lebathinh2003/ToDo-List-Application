using Contract.Common;
using Contract.Constants;
using Contract.DTOs.SignalRDTOs;
using Contract.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Errors;
using TaskService.Domain.Interfaces;
using UserProto;
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
    private readonly GrpcUser.GrpcUserClient _grpcUserClient;
    private readonly ISignalRService _signalRService;

    public UpdateTaskStatusCommandHandler(IApplicationDbContext context, ISignalRService signalRService, GrpcUser.GrpcUserClient grpcUserClient)
    {
        _context = context;
        _signalRService = signalRService;
        _grpcUserClient = grpcUserClient;
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


            var response = await _grpcUserClient.GetUserDetailByIdAsync(new GrpcIdRequest
            {
                Id = task.AssigneeId.ToString()
            }, cancellationToken: cancellationToken);

            if (response == null)
            {
                return Result<bool?>.Failure(TaskError.NotFound, "User not found");
            }

            await _signalRService.InvokeAction(SignalRAction.PushTaskUpdateNotification.ToString(), new SignalRTaskItemWithRecipentsDTO
            {
                TaskItem = new SignalRTaskItemDTO
                {
                    Id = task.Id,
                    AssigneeId = task.AssigneeId,
                    AssigneeName = response.FullName,
                    AssigneeUsername = response.Usrname,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status.ToString(),
                    CreatedDate = task.CreatedAt,
                    DueDate = task.DueDate,
                    IsActive = task.IsActive,
                },
                Recipients = new List<Guid>(),
                ExcludeRecipients = new List<Guid>()
            });

            return Result<bool?>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool?>.Failure(TaskError.AddTaskFail, ex.Message);
        }
   
    }
}
