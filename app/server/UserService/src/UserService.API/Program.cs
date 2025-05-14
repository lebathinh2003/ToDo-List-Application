using Contract.Extension;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using UserService.API.Extensions;
using UserService.Application;
using UserService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

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
//builder.WebHost.UseUrls($"http://*:5002");
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5002, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });

    options.ListenLocalhost(6002, listenOptions =>
    {
        listenOptions.UseHttps();
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

builder.Services.AddGrpcServices();

builder.Services.AddApplication();



var app = builder.Build();

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

app.Run();
