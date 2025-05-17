using System.Security.Claims;
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
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest createUserRequest)
    {
        var result = await _sender.Send(new CreateUserCommand
        {
            Username = createUserRequest.Username,
            Email = createUserRequest.Email,
            Password = createUserRequest.Password,
            Fullname = createUserRequest.FullName,
            Address = createUserRequest.Address,
            IsActive = createUserRequest.IsActive,
        });
        result.ThrowIfFailure();
        return Ok(result.Value);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete/id/{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _sender.Send(new DeleteUserCommand
        {
            UserId = id,
        });
        result.ThrowIfFailure();
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("restore/id/{id}")]
    public async Task<IActionResult> RestoreUser(Guid id)
    {
        var result = await _sender.Send(new RestoreUserCommand
        {
            UserId = id,
        });
        result.ThrowIfFailure();
        return Ok();
    }

    [Authorize]
    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        foreach(var claim in HttpContext.User.Claims)
        {
            Console.WriteLine($"{claim.Type}: {claim.Value}");
        }

        var result = await _sender.Send(new GetUserByIdQuery
        {
            Id = id,
        });
        
        return Ok(result.Value);
    }

    //[Authorize(Roles = "Admin")]
    [HttpGet("testhu")]
    public async Task<IActionResult> AdminGetUsers([FromQuery] PaginatedDTO paginatedDTO)
    {
        var result = await _sender.Send(new GetFullUserQuery
        {
            PaginatedDTO = paginatedDTO
        });
        result.ThrowIfFailure();
        return Ok(result.Value);
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest updateUserRequest)
    {

        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("Id is null.");
        }

        var result = await _sender.Send(new UpdateUserCommand
        {
            Id = Guid.Parse(userId),
            Address = updateUserRequest.Address,
            Fullname = updateUserRequest.FullName,
            Email = updateUserRequest.Email,
            IsActive = null,
        });
        
        result.ThrowIfFailure();
        return Ok(result.Value);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("id/{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest updateUserRequest)
    {
        Console.WriteLine("Passworddddddddđ:" + updateUserRequest.Password);
        if (id == Guid.Empty)
        {
            return BadRequest("Id is null.");
        }

        var result = await _sender.Send(new UpdateFullUserWithIdCommand
        {
            Id = id,
            Address = updateUserRequest.Address,
            Fullname = updateUserRequest.FullName,
            Email = updateUserRequest.Email,
            Username = updateUserRequest.Username,
            Password = updateUserRequest.Password,
            IsActive = updateUserRequest.IsActive,
        });

        result.ThrowIfFailure();
        return Ok(result.Value);
    }
}
