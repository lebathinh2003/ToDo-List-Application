using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Requests;
using UserService.Application.Users.Commands;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/user")]
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
            UserName = createUserRequest.Username,
            Email = createUserRequest.Email,
            Password = createUserRequest.Password,
            Fullname = createUserRequest.FullName,
        });
        return Ok(result);
    }
}
