using AutoMapper;
using IdentityProto;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Security;
using System.Reflection;
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
        services.AddGrpcClient<GrpcIdentity.GrpcIdentityClient>(options =>
        {
            options.Address = new Uri("https://localhost:6001");
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new SocketsHttpHandler
            {
                EnableMultipleHttp2Connections = true,
                SslOptions = new SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (sender, cert, chain, errors) => true
                }
            };
        });

    }
}
