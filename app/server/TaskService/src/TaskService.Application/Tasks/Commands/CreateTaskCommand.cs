using MediatR;
using TaskService.Application.DTOs;
using TaskService.Domain.Interfaces;
using Task = TaskService.Domain.Models.Task;
namespace TaskService.Application.Tasks.Commands;
public record CreateTaskCommand : IRequest<TaskDTO>
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Guid AssigneeId { get; set; }
    public string Status { get; set; } = null!;
}

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDTO>
{
    private readonly IApplicationDbContext _context;

    public CreateTaskCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TaskDTO> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new Task
        {
            AssigneeId = request.AssigneeId,
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            IsActive = true,
            Status = (Domain.Models.TaskStatus)Enum.Parse(typeof(Domain.Models.TaskStatus), request.Status),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _context.Tasks.AddAsync(task);

        return new TaskDTO
        {
            AssigneeId = task.AssigneeId,
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsActive = task.IsActive,
            AssigneeName = "",
            AssigneeUsername = "",
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            Status = task.Status.ToString(),
        };
    }
}
