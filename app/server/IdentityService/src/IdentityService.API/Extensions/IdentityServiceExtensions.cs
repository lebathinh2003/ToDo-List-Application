using IndentityService.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityService.Infrastructure;
using IdentityService.API.Configs;
using IdentityService.API.Services;
namespace IdentityService.API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityAndIdentityServer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;

        // Thêm DbContext cho ASP.NET Identity
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Cấu hình ASP.NET Identity
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddIdentityServer(options =>
        {
            options.EmitStaticAudienceClaim = true;
        })
        .AddInMemoryClients(IdentityServerInMemoryConfig.GetClients()) // Không có client nếu dùng ResourceOwnerPassword
        .AddInMemoryIdentityResources(IdentityServerInMemoryConfig.GetIdentityResources()) // Không cần identity resource như openid, profile
        .AddAspNetIdentity<ApplicationUser>()
        .AddProfileService<CustomProfileService>()
        .AddDeveloperSigningCredential();
        return services;
    }

}
