﻿using Contract.Extension;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SignalRHub.Extensions;
using SignalRHub.Filters;
using SignalRHub.Hubs;
using SignalRHub.Interfaces;
using SignalRHub.Services;

namespace SignalRHub;

public static class DependenciesInjection
{
    private static string HUB_ENDPOINT = "/hub-server";
    public static WebApplicationBuilder AddChatHubServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        var host = builder.Host;

        services.AddCommonInfrastructureServices("SignalRHub");

        services.AddSingleton<IMemoryTracker, MemoryTracker>();

        services.AddHttpContextAccessor();

        services.AddCustomAuthentication(configuration, HUB_ENDPOINT);

        services.AddSignalR(options =>
        {
            //Global filter
            options.AddFilter<GlobalLoggingFilter>();
        })
        .AddNewtonsoftJsonProtocol(options =>
        {
            options.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder
                    .SetIsOriginAllowed(_ => true)  // allow any origin
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
            );
        });

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenLocalhost(7000, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
            });
        });

        return builder;
    }

    public static WebApplication UseChatHubService(this WebApplication app)
    {
        app.UseRouting();
        app.UseCors("AllowAll"); // Enable CORS
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapHub<HubServer>(HUB_ENDPOINT);
        return app;
    }
}
