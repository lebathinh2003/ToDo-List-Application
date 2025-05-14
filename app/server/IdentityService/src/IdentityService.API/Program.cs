using Duende.IdentityServer.Services;
using IdentityService.API.Extensions;
using IdentityService.API.Services;
using IdentityService.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddIdentityAndIdentityServer(builder.Configuration);
builder.Services.AddScoped<IProfileService, CustomProfileService>();
builder.Services.AddInfrastructure(builder.Configuration);

// For HttpClientFactory
builder.Services.AddHttpClient();

//Jwt authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.WebHost.UseUrls($"http://*:5001");
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

var app = builder.Build();

// Configure middleware
app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();  

app.UseIdentityServer();

app.MapControllers();
app.Run();
