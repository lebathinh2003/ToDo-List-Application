using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IndentityService.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityService.API.Services;

public class CustomProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomProfileService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim("sub", user.Id.ToString()),
            new Claim("role", string.Join(",", roles)) 
        };

        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        context.IsActive = user != null;
    }
}
