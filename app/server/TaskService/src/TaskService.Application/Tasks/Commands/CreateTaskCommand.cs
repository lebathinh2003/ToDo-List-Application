using Contract.Common;
using Contract.Constants;
using Contract.DTOs.SignalRDTOs;
using Contract.Interfaces;
using MediatR;
using TaskService.Application.DTOs;
using TaskService.Domain.Errors;
using TaskService.Domain.Interfaces;
using UserProto;
using Task = TaskService.Domain.Models.Task;
using TaskStatus = TaskService.Domain.Models.TaskStatus;
namespace TaskService.Application.Tasks.Commands;
public record CreateTaskCommand : IRequest<Result<TaskDTO?>>
{
    public Guid AdminId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Guid AssigneeId { get; set; }
    public string Status { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime? DueDate { get; set; } = null;
}

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<TaskDTO?>>
{
    private readonly IApplicationDbContext _context;
    private readonly GrpcUser.GrpcUserClient _grpcUserClient;
    private readonly ISignalRService _signalRService;

    public CreateTaskCommandHandler(IApplicationDbContext context, GrpcUser.GrpcUserClient grpcUserClient, ISignalRService signalRService)
    {
        _context = context;
        _grpcUserClient = grpcUserClient;
        _signalRService = signalRService;
    }

    public async Task<Result<TaskDTO?>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        try {

            if(request.AssigneeId == Guid.Empty ||
                request.AdminId == Guid.Empty ||
                string.IsNullOrEmpty(request.Title) ||
                string.IsNullOrEmpty(request.Description) ||
                string.IsNullOrEmpty(request.Status) ||
                !Enum.TryParse<TaskStatus>(request.Status, out var status))
            {
                return Result<TaskDTO?>.Failure(TaskError.NullParameters, "Invalid input data");
            }
                
            var task = new Task
            {
                AssigneeId = request.AssigneeId,
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                IsActive = request.IsActive,
                Status = status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                DueDate = request.DueDate,
            };

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
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                Status = task.Status.ToString(),
                DueDate = task.DueDate,
            };

            _context.Tasks.Add(task);
            await _context.Instance.SaveChangesAsync(cancellationToken);

            await _signalRService.InvokeAction(SignalRAction.PushNewTaskNotification.ToString(), new SignalRTaskItemWithRecipentsDTO {
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
                ExcludeRecipients = new List<Guid> { request.AdminId },
                Recipients = new List<Guid> { task.AssigneeId },
            });

            return Result<TaskDTO?>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<TaskDTO?>.Failure(TaskError.AddTaskFail, ex.Message);
        }
   
    }
}
