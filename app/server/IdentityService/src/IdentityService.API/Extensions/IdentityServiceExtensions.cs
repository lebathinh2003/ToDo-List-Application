using IndentityService.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityService.Infrastructure;
namespace IdentityService.API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityAndIdentityServer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


        services.AddIdentityServer(options =>
        {
            options.EmitStaticAudienceClaim = true;
        })
        .AddAspNetIdentity<ApplicationUser>()
        .AddConfigurationStore(options =>
        {
            options.ConfigureDbContext = b =>
                b.UseSqlServer(connectionString,
                    sql => sql.MigrationsAssembly(typeof(Program).Assembly.FullName));
        })
        .AddOperationalStore(options =>
        {
            options.ConfigureDbContext = b =>
                b.UseSqlServer(connectionString,
                    sql => sql.MigrationsAssembly(typeof(Program).Assembly.FullName));
            options.EnableTokenCleanup = true;
        })
        .AddDeveloperSigningCredential();

        return services;
    }

}
