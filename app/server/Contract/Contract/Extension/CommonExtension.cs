using Contract.Interfaces;
using Contract.Services;
using Microsoft.Extensions.DependencyInjection;
using Contract.Utilities;
using Contract.Common;
using MassTransit;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using Contract.Middleware;

namespace Contract.Extension;

public static class CommonExtension
{
    public static WebApplicationBuilder ConfigureCommonAPIServices(this WebApplicationBuilder builder)
    {
        EnvUtility.LoadEnvFile();

        builder.ConfigureKestrel();
        return builder;
    }

    /**
     * <summary>
     *   Add ErrorValidation, Controller, HttpContextAccessor and Authentication
     * </summary>
     */
    public static IServiceCollection AddCommonAPIServices(this IServiceCollection services)
    {
        services.AddErrorValidation();
        services.AddControllers()
            // Prevent circular JSON reach max depth of the object when serialization
            //.AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            //    options.JsonSerializerOptions.WriteIndented = true;
            //})
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
            });

        services.AddHttpContextAccessor();

        //services.AddCustomDownstreamAuthentication();

        return services;
    }

    /**
     * <summary>
     *   Only usable for Identity server
     *   Add ErrorValidation, Controller, HttpContextAccessor
     * </summary>
     */
    public static IServiceCollection AddCommonAPIWithoutAuthServices(this IServiceCollection services)
    {
        services.AddErrorValidation();
        services.AddControllers()
            // Prevent circular JSON reach max depth of the object when serialization
            //.AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            //    options.JsonSerializerOptions.WriteIndented = true;
            //})
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
            });

        services.AddHttpContextAccessor();

        return services;
    }


    /**
     * <summary>
     *   Add Consul, MassTransit and PaginateDataUtility
     * </summary>
     */
    public static IServiceCollection AddCommonInfrastructureServices(this IServiceCollection services, string apiPrjName)
    {
        services.AddScoped(typeof(IPaginateDataUtility<,>), typeof(PaginateDataUtility<,>));
        services.AddMassTransitService(apiPrjName);

        return services;
    }


    /**
     * <summary>
     *   Add Auth, only usable for api gateway
     * </summary>
     */
    public static IServiceCollection AddAPIGatewayAPIServices(this IServiceCollection services)
    {
        EnvUtility.LoadEnvFile();
        //services.AddCustomAuthentication();

        return services;
    }

    public static WebApplication UseCommonServices(this WebApplication app)
    {

        app.UseRouting();

        app.UseMiddleware<GlobalHandlingErrorMiddleware>();

        return app;
    }

    // ================ Config masstransit ==========================================
    private static IServiceCollection AddMassTransitService(this IServiceCollection services, string apiPrjName)
    {
        services.AddMassTransit(busConfig =>
        {
            busConfig.SetKebabCaseEndpointNameFormatter();

            var applicationAssembly = AppDomain.CurrentDomain.Load(apiPrjName);
            busConfig.AddConsumers(applicationAssembly);

            busConfig.UsingRabbitMq((context, config) =>
            {
                var username = DotNetEnv.Env.GetString("RABBITMQ_DEFAULT_USER", "admin");
                var password = DotNetEnv.Env.GetString("RABBITMQ_DEFAULT_PASS", "pass");
                var rabbitMQHost = DotNetEnv.Env.GetString("RABBITMQ_HOST", "localhost:5672");

                config.Host(new Uri($"amqp://{rabbitMQHost}/"), h =>
                {
                    h.Username(username);
                    h.Password(password);

                    h.Heartbeat(TimeSpan.FromSeconds(10));
                });

                config.UseMessageRetry(retryConfig =>
                {
                    retryConfig.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
                });

                RegisterEndpointsFromAttributes(context, config, applicationAssembly);

                config.ConfigureEndpoints(context);
            });
        });
        services.AddScoped<IServiceBus, MassTransitServiceBus>();
        return services;
    }

    private static void RegisterEndpointsFromAttributes(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator config, Assembly assembly)
    {
        var consumerTypes = assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>)));

        foreach (var consumerType in consumerTypes)
        {
            var queueNameAttribute = consumerType.GetCustomAttribute<QueueNameAttribute>();
            if (queueNameAttribute == null)
            {
                continue;
            }
            config.ReceiveEndpoint(queueNameAttribute.QueueName, endpoint =>
            {
                endpoint.ConfigureConsumer(context, consumerType);

                endpoint.Bind(queueNameAttribute.ExchangeName);
            });
        }
    }
}
