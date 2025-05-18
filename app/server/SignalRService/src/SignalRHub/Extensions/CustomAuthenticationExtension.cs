using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace SignalRHub.Extensions;

public static class CustomAuthenticationExtension
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration, string hubEndPoint)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = jwtSettings["Issuer"];
                options.RequireHttpsMetadata = false;
                options.Audience = jwtSettings["Audience"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
                    NameClaimType = ClaimTypes.NameIdentifier,     // Nếu bạn muốn dùng User.Identity.Name
                    RoleClaimType = ClaimTypes.Role      // Nếu bạn muốn dùng [Authorize(Roles = "Admin")]
                };
                // For development only
                // We have to hook the OnMessageReceived event in order to
                // allow the JWT authentication handler to read the access
                // token from the query string when a WebSocket or 
                // Server-Sent Events request comes in.

                // Sending the access token in the query string is required when using WebSockets or ServerSentEvents
                // due to a limitation in Browser APIs. We restrict it to only calls to the
                // SignalR hub in this code.
                // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
                // for more information about security considerations when using
                // the query string to transmit the access token.
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        Console.WriteLine($"[JwtBearer] OnMessageReceived: path={path}, token={(string.IsNullOrEmpty(accessToken) ? "(none)" : accessToken + "...")}");
                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments(hubEndPoint))
                        {
                            context.Token = accessToken;
                            Console.WriteLine("[JwtBearer] Token assigned to context.Token");
                        }
                        return Task.CompletedTask;
                    }
                };


            });
        return services;
    }
}
