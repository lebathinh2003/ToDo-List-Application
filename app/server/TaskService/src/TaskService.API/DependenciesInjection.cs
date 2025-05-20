using Contract.Extension;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using TaskService.API.Extensions;
using TaskService.Application;
using TaskService.Infrastructure;
namespace TaskService.API;

public static class DependenciesInjection
{
    public static WebApplicationBuilder AddAPIServices(this WebApplicationBuilder builder)
    {
        // Add services
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddErrorValidation();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddInfrastructure(builder.Configuration);

        //Authentication & Authorization
        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = "http://localhost:5001";
                options.RequireHttpsMetadata = false;
                options.Audience = "http://localhost:5001/resources";
            });

        builder.Services.AddAuthorization();
        //builder.WebHost.UseUrls($"http://*:5003");
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenLocalhost(5003, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
            });

            options.ListenLocalhost(6003, listenOptions =>
            {
                listenOptions.UseHttps();
                listenOptions.Protocols = HttpProtocols.Http2;
            });
        });

        builder.Services.AddGrpcServices();

        builder.Services.AddApplication();

        return builder;
    }

    public static async Task<WebApplication> UseAPIServicesAsync(this WebApplication app)
    {
        // Middleware
        app.UseCommonServices();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.UseGrpcServices();

        await app.UseSignalRServiceAsync();

        return app;
    }
}

