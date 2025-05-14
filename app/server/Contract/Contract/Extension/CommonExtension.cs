using Contract.Interfaces;
using Contract.Services;
using Microsoft.Extensions.DependencyInjection;
using Contract.Utilities;
using Contract.Common;
using MassTransit;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Contract.Middleware;

namespace Contract.Extension;

public static class CommonExtension
{
    public static WebApplicationBuilder ConfigureCommonAPIServices(this WebApplicationBuilder builder)
    {
        EnvUtility.LoadEnvFile();

        builder.ConfigureSerilog();
        builder.ConfigureKestrel();
        builder.ConfigureHealthCheck();
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

        services.AddCustomDownstreamAuthentication();

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
        services.AddCustomAuthentication();

        return services;
    }

    private static IServiceCollection AddCustomAuthentication(this IServiceCollection services)
    {
        var retryPolicy = Polly.Policy.Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 100,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(attempt),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    // Log the retry attempt
                    Log.Warning($"Retry {retryCount} encountered an error: {exception.Message}. Waiting {timeSpan} before next retry.");
                });

        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var identityUri = retryPolicy.ExecuteAsync(() =>
                {
                    var uri = "CONSUL_IDENTITY";
                    return uri == null ? throw new Exception("Identity service URI not found.") : Task.FromResult(uri);
                }).GetAwaiter().GetResult();

                Log.Information("Connect to Identity Provider: " + identityUri!.ToString());

                options.RequireHttpsMetadata = false;
                options.Authority = identityUri!.ToString();
                // Clear default Microsoft's JWT claim mapping
                // Ref: https://stackoverflow.com/questions/70766577/asp-net-core-jwt-token-is-transformed-after-authentication
                options.MapInboundClaims = false;

                options.TokenValidationParameters.ValidTypes = ["at+jwt"];

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                // For development only
                options.IncludeErrorDetails = true;

                options.BackchannelHttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger("JwtBearer");
                        logger.LogInformation("Token validated successfully.");
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger("JwtBearer");
                        logger.LogError(context.Exception, "Token authentication failed.");
                        return Task.CompletedTask;
                    }
                };
            });
        return services;
    }

    /**
     * <summary>
     *  Authenticate for downstream service, ignore jwt validation because api gateway does all the heavy work
     * </summary>
     */
    private static IServiceCollection AddCustomDownstreamAuthentication(this IServiceCollection services)
    {

        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            // Clear default Microsoft's JWT claim mapping
            // Ref: https://stackoverflow.com/questions/70766577/asp-net-core-jwt-token-is-transformed-after-authentication
            options.MapInboundClaims = false;
            options.SaveToken = true;

            // Completely disable token validations
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = false,
                RequireSignedTokens = false,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.Zero,
                /*  
                 *  Return JwtSecurityToken(token) will cause this error
                 *  
                 *  Microsoft.IdentityModel.Tokens.SecurityTokenInvalidSignatureException: IDX10506: Signature validation failed.
                 *  The user defined 'Delegate' specified on TokenValidationParameters did not return a 'Microsoft.IdentityModel.JsonWebTokens.JsonWebToken',
                 *  but returned a 'System.IdentityModel.Tokens.Jwt.JwtSecurityToken' when validating token: '[PII of type 'Microsoft.IdentityModel.JsonWebTokens.
                 *  JsonWebToken' is hidden. For more details, see https://aka.ms/IdentityModel/PII.]'
                */
                SignatureValidator = (token, parameters) => new Microsoft.IdentityModel.JsonWebTokens.JsonWebToken(token)
            };
            // For development only
            options.IncludeErrorDetails = true;
        });
        return services;
    }

    public static WebApplication UseCommonServices(this WebApplication app, string serviceName)
    {
        app.UseSerilogServices();

        app.UseRouting();
        app.UseCustomHealthCheck();

        app.UseMiddleware<GlobalHandlingErrorMiddleware>();
        app.UseMiddleware<ValidateGatewayRequestMiddleware>();

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
