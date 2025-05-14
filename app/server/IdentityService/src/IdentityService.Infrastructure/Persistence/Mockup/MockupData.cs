using IndentityService.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Infrastructure.Persistence.Mockup;

public class MockupData
{
    private UserManager<ApplicationUser> _userManager;
    private RoleManager<IdentityRole<Guid>> _roleManager;
    public MockupData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAllData()
    {
        Console.WriteLine("Seed identity service data...");
        await SeedRoleAsync();
        await SeedAccountAsync();
    }

    private async Task SeedRoleAsync()
    {
        Console.WriteLine("Seed role...");
        foreach (var roleName in RoleData.Data)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                Console.WriteLine($"Role added: {roleName}");
            }
        }
    }

    public async Task SeedAccountAsync()
    {
        Console.WriteLine("Seed account...");
        foreach (var seedAccount in AccountData.Data)
        {
            var account = new ApplicationUser
            {
                Id = seedAccount.Id,
                UserName = seedAccount.Username,
                Email = seedAccount.Email,
                IsActive = true,
            };

            var isUserExist = _userManager.Users.Any(a => a.Id == seedAccount.Id);
            if (!isUserExist)
            {
                var result = _userManager.CreateAsync(account, seedAccount.Password).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Console.WriteLine($"Account added: {account.UserName}");
                await _userManager.AddToRoleAsync(account, seedAccount.Role);

            }
        }
    }
}
