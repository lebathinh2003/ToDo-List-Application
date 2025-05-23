﻿using IndentityService.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityService.Infrastructure;
using IdentityService.API.Configs;
using IdentityService.API.Services;
using IndentityService.Domain.Interfaces;
namespace IdentityService.API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityAndIdentityServer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;

        // Thêm DbContext cho ASP.NET Identity
        services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Cấu hình ASP.NET Identity
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddIdentityServer(options =>
        {
            options.EmitStaticAudienceClaim = true;
            options.IssuerUri = "http://localhost:5001";
        })
        .AddInMemoryClients(IdentityServerInMemoryConfig.GetClients()) // Không có client nếu dùng ResourceOwnerPassword
        .AddInMemoryIdentityResources(IdentityServerInMemoryConfig.GetIdentityResources()) // Không cần identity resource như openid, profile
        .AddAspNetIdentity<ApplicationUser>()
        .AddProfileService<CustomProfileService>()
        .AddDeveloperSigningCredential();

        services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
        });

        return services;
    }

}
