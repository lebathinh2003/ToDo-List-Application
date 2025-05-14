using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UserProto;
using UserService.Application.Configs;

namespace UserService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // Register automapper
        IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
        services.AddSingleton(mapper);
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddGrpcClientService();

        return services;
    }

    private static void AddGrpcClientService(this IServiceCollection services)
    {


    }
}
