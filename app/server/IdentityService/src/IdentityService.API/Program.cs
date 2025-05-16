using Contract.Extension;
using Duende.IdentityServer.Services;
using IdentityService.API.Extensions;
using IdentityService.API.Services;
using IdentityService.Application;
using IdentityService.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddIdentityAndIdentityServer(builder.Configuration);
builder.Services.AddScoped<IProfileService, CustomProfileService>();
builder.Services.AddInfrastructure(builder.Configuration);

// For HttpClientFactory
builder.Services.AddHttpClient();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

// Jwt authentication
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
    options.ListenLocalhost(6001, listenOptions =>
    {
        listenOptions.UseHttps();
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

builder.Services.AddGrpcServices();
builder.Services.AddApplication();

var app = builder.Build();
app.UseCommonServices();

// Middleware configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseIdentityServer();

app.UseAuthentication();     // After UseRouting
app.UseAuthorization();

app.MapControllers();        // Top-level route registration

app.UseGrpcServices();

app.Run();
