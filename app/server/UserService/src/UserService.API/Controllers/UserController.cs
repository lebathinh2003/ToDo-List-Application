using MediatR;
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

    [HttpGet]
    public async Task<IActionResult> GetUserById([FromQuery] Guid id)
    {
        var result = await _sender.Send(new GetUserByIdQuery
        {
            Id = id,
        });
        
        return Ok(result);
    }
}
