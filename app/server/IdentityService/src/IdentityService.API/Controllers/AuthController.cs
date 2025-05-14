using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IndentityService.Domain.Models;
using IdentityService.API.Request;
using Duende.IdentityModel.Client;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IHttpClientFactory httpClientFactory)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user == null) return Unauthorized("Invalid credentials");

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

        return Ok(new
        {
            access_token = tokenResponse.AccessToken,
            expires_in = tokenResponse.ExpiresIn
        });
    }
}

