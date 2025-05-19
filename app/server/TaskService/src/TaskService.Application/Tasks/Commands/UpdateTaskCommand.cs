using Contract.Common;
using Contract.Constants;
using Contract.DTOs.SignalRDTOs;
using Contract.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskService.Application.DTOs;
using TaskService.Domain.Errors;
using TaskService.Domain.Interfaces;
using UserProto;
using TaskStatus = TaskService.Domain.Models.TaskStatus;
namespace TaskService.Application.Tasks.Commands;
public record UpdateTaskCommand : IRequest<Result<TaskDTO?>>
{
    public Guid AdminId { get; set; }
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Guid AssigneeId { get; set; }
    public string Status { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime? DueDate { get; set; } = null;
}

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Result<TaskDTO?>>
{
    private readonly IApplicationDbContext _context;
    private readonly GrpcUser.GrpcUserClient _grpcUserClient;
    private readonly ISignalRService _signalRService;
    public UpdateTaskCommandHandler(IApplicationDbContext context, GrpcUser.GrpcUserClient grpcUserClient, ISignalRService signalRService)
    {
        _context = context;
        _grpcUserClient = grpcUserClient;
        _signalRService = signalRService;
    }

    public async Task<Result<TaskDTO?>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        try {

            if(request.Id == Guid.Empty ||
                request.AdminId == Guid.Empty ||
                request.AssigneeId == Guid.Empty || 
                string.IsNullOrEmpty(request.Title) ||
                string.IsNullOrEmpty(request.Description) ||
                string.IsNullOrEmpty(request.Status) ||
                !Enum.TryParse<TaskStatus>(request.Status, out var status))
            {
                return Result<TaskDTO?>.Failure(TaskError.NullParameters, "Invalid input data");
            }
              
            var task = await _context.Tasks.SingleOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (task == null)
            {
                return Result<TaskDTO?>.Failure(TaskError.NotFound, "Task not found");
            }

            var oldAssigneeId = task.AssigneeId;

            task.AssigneeId = request.AssigneeId;
            task.Title = request.Title;
            task.Description = request.Description;
            task.IsActive = request.IsActive;
            task.Status = status;
            task.UpdatedAt = DateTime.UtcNow;
            task.DueDate = request.DueDate;

            var response = await _grpcUserClient.GetUserDetailByIdAsync(new GrpcIdRequest
            {
                Id = task.AssigneeId.ToString()
            }, cancellationToken: cancellationToken);

            if (response == null)
            {
                return Result<TaskDTO?>.Failure(TaskError.NotFound, "User not found");
            }

            var result = new TaskDTO
            {
                AssigneeId = task.AssigneeId,
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsActive = task.IsActive,
                AssigneeName = response.FullName,
                AssigneeUsername = response.Usrname,
                UpdatedAt = task.UpdatedAt,
                CreatedAt = task.CreatedAt,
                Status = task.Status.ToString(),
                DueDate = task.DueDate,
            };

            _context.Tasks.Update(task);
            await _context.Instance.SaveChangesAsync(cancellationToken);

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
                Recipients = (new HashSet<Guid>() { task.AssigneeId, oldAssigneeId }).ToList(),
                ExcludeRecipients = new List<Guid>() { request.AdminId }
            });
                

            return Result<TaskDTO?>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<TaskDTO?>.Failure(TaskError.AddTaskFail, ex.Message);
        }
   
    }
}
