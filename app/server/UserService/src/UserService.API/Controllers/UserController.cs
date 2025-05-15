using Contract.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Requests;
using UserService.Application.Users.Commands;
using UserService.Application.Users.Queries;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly ISender _sender;

    public UserController(ISender sender)
    {
        _sender = sender;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromQuery] CreateUserRequest createUserRequest)
    {
        var result = await _sender.Send(new CreateUserCommand
        {
            Username = createUserRequest.Username,
            Email = createUserRequest.Email,
            Password = createUserRequest.Password,
            Fullname = createUserRequest.FullName,
            Address = createUserRequest.Address,
        });
        result.ThrowIfFailure();
        return Ok(result.Value);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromQuery] Guid id)
    {
        var result = await _sender.Send(new DeleteUserCommand
        {
            UserId = id,
        });
        result.ThrowIfFailure();
        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserById([FromQuery] Guid id)
    {
        var result = await _sender.Send(new GetUserByIdQuery
        {
            Id = id,
        });
        
        return Ok(result.Value);
    }

    //[Authorize(Roles = "Admin")]
    [HttpGet("get-users")]
    public async Task<IActionResult> AdminGetUsers([FromQuery] PaginatedDTO paginatedDTO)
    {
        var result = await _sender.Send(new GetFullUserQuery
        {
            PaginatedDTO = paginatedDTO
        });
        result.ThrowIfFailure();
        return Ok(result.Value);
    }
}
