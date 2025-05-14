using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskService.API.Requests;
using TaskService.Application.Tasks.Commands;
namespace TaskService.API.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ISender _sender;

    public TasksController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateTask([FromQuery] CreateTaskRequest createTaskRequest)
    {
        var result = await _sender.Send(new CreateTaskCommand
        {
            AssigneeId = createTaskRequest.AssigneeId,
            Status = createTaskRequest.Status,
            Description = createTaskRequest.Description,
            Title = createTaskRequest.Title,
        });
        return Ok(result);
    }
}
