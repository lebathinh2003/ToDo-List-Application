using System.Security.Claims;
using Contract.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskService.API.Requests;
using TaskService.Application.Tasks.Commands;
using TaskService.Application.Tasks.Queries;
using TaskStatus = TaskService.Domain.Models.TaskStatus;
namespace TaskService.API.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TasksController(ISender sender, IHttpContextAccessor httpContextAccessor)
    {
        _sender = sender;
        _httpContextAccessor = httpContextAccessor;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest createTaskRequest)
    {
        var adminId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(adminId))
        {
            return BadRequest("AdminId is null.");
        }
        var result = await _sender.Send(new CreateTaskCommand
        {
            AdminId = Guid.Parse(adminId),
            AssigneeId = createTaskRequest.AssigneeId,
            Status = createTaskRequest.Status,
            Description = createTaskRequest.Description,
            Code = createTaskRequest.Code,
            Title = createTaskRequest.Title,
            IsActive = createTaskRequest.IsActive,
            DueDate = createTaskRequest.DueDate,
        });
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> AdminGetTasks([FromQuery] PaginatedDTO paginatedDTO, TaskStatus? status)
    {
        var result = await _sender.Send(new GetFullTaskQuery
        {
            PaginatedDTO = paginatedDTO,
            Status = status,
        });
        result.ThrowIfFailure();
        return Ok(result.Value);
    }

    [Authorize(Roles = "Staff")]
    [HttpGet("assignee/id/{id}")]
    public async Task<IActionResult> GetTasksByAssingeeId([FromQuery] PaginatedDTO paginatedDTO, TaskStatus? status, Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("StaffId is null.");
        }
        var result = await _sender.Send(new GetFullTaskQuery
        {
            PaginatedDTO = paginatedDTO,
            Status = status,
            AssigneeId = id,
        });
        result.ThrowIfFailure();
        return Ok(result.Value);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("id/{id}")]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskRequest updateTaskRequest)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Id is null.");
        }

        var adminId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(adminId))
        {
            return BadRequest("AdminId is null.");
        }


        var result = await _sender.Send(new UpdateTaskCommand
        {
            AdminId = Guid.Parse(adminId),
            Id = id,
            AssigneeId = updateTaskRequest.AssigneeId,
            Status = updateTaskRequest.Status,
            Description = updateTaskRequest.Description,
            Code = updateTaskRequest.Code,
            Title = updateTaskRequest.Title,
            IsActive = updateTaskRequest.IsActive,
            DueDate = updateTaskRequest.DueDate,
        });

        result.ThrowIfFailure();
        return Ok(result.Value);
    }

    [Authorize(Roles = "Staff")]
    [HttpPut("status/id/{id}")]
    public async Task<IActionResult> UpdateTaskStatus(Guid id, [FromBody] UpdateTaskStatusRequest updateTaskStatusRequest)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Id is null.");
        }

        var result = await _sender.Send(new UpdateTaskStatusCommand
        {
            Id = id,
            Status = updateTaskStatusRequest.Status,
        });

        result.ThrowIfFailure();
        return Ok(result.Value);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete/id/{id}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var adminId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(adminId))
        {
            return BadRequest("AdminId is null.");
        }
        var result = await _sender.Send(new DeleteTaskCommand
        {
            TaskId = id,
            AdminId = Guid.Parse(adminId),
        });
        result.ThrowIfFailure();
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("restore/id/{id}")]
    public async Task<IActionResult> RestoreTask(Guid id)
    {
        var adminId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(adminId))
        {
            return BadRequest("AdminId is null.");
        }
        var result = await _sender.Send(new RestoreTaskCommand
        {
            TaskId = id,
            AdminId = Guid.Parse(adminId),
        });
        result.ThrowIfFailure();
        return Ok();
    }
}
