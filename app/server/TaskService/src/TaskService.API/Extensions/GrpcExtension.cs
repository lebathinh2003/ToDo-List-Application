using TaskService.API.GrpcServices;

namespace TaskService.API.Extensions;

public static class GrpcExtension
{
    public static IServiceCollection AddGrpcServices(this IServiceCollection services)
    {
        services.AddGrpc();
        return services;
    }

    public static WebApplication UseGrpcServices(this WebApplication app)
    {
        app.UseRouting();
        app.MapGrpcService<GrpcTaskService>();
        return app;
    }
}