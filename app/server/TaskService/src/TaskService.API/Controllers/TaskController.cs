using System.IdentityModel.Tokens.Jwt;
using Contract.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskService.API.Requests;
using TaskService.Application.Tasks.Commands;
using TaskService.Application.Tasks.Queries;
using Newtonsoft.Json;
using System.Security.Claims;
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

    [Authorize(Roles = "Admin")]
    [HttpGet("get-tasks")]
    public async Task<IActionResult> AdminGetTasks([FromQuery] PaginatedDTO paginatedDTO)
    {
        Console.WriteLine("Ok ne 1");
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("No 'NameIdentifier' claim found in the token.");
        }

        var result = await _sender.Send(new GetFullTaskQuery
        {
            UserId = Guid.Parse(userIdClaim!),
            PaginatedDTO = paginatedDTO
        });
        result.ThrowIfFailure();
        return Ok(result.Value);
    }
}
