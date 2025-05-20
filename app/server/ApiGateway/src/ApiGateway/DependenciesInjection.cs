using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace ApiGateway;

public static class DependenciesInjection
{
    public static WebApplicationBuilder AddApiGateway(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        var host = builder.Host;

        builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

        var jwtSettings = builder.Configuration.GetSection("Jwt");

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenLocalhost(5000, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
            });

            options.ListenLocalhost(6000, listenOptions =>
            {
                listenOptions.UseHttps();
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });

        builder.Services.AddAuthentication()
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = jwtSettings["Issuer"]; // URL của Identity Server
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

        // Add Ocelot to services
        builder.Services.AddOcelot();

        return builder;
    }

    public static WebApplication UseApiGateway(this WebApplication app)
    {
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseWebSockets();
        app.UseOcelot().Wait();   

        return app;
    }
}
