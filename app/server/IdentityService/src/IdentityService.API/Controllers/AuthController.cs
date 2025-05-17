using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IndentityService.Domain.Models;
using IdentityService.API.Request;
using Duende.IdentityModel.Client;
using IdentityService.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/auth")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISender _sender;

    public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IHttpClientFactory httpClientFactory, ISender sender)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _httpClientFactory = httpClientFactory;
        _sender = sender;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Not found id from token");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Unauthorized("User not found");

        var isValid = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
        if (!isValid)
            return BadRequest("Old password is incorrect");

        // Đổi mật khẩu
        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("Password changed successfully");
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user == null) return Unauthorized("Invalid credentials");

        Console.WriteLine("User isactive:"+user.IsActive);

        if(!user.IsActive) return Unauthorized("User is not active");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded) return Unauthorized("Invalid credentials");

        var client = _httpClientFactory.CreateClient();
        var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5001"); // IdentityServer URL

        if (disco.IsError)
            return StatusCode(500, disco.Error);

        var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = "my-client",
            ClientSecret = "secret",
            Scope = "openid roles",
            UserName = request.Username,
            Password = request.Password
        });

        if (tokenResponse.IsError)
            return Unauthorized(tokenResponse.Error);

        return Ok(new LoginResonseDTO {
            UserId = user.Id,
            Token = tokenResponse.AccessToken ?? "",
            Email = user.Email ?? "",
            Username = user.UserName ?? "",
            Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? ""
        });

    }
}

