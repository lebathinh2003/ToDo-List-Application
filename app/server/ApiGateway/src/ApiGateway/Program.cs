using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Load cấu hình từ file ocelot.json
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Lấy thông tin cấu hình JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");

//builder.WebHost.UseUrls($"http://*:5000");
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5000, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });

    options.ListenLocalhost(6000, listenOptions =>
    {
        listenOptions.UseHttps();
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = jwtSettings["Issuer"]; // URL của Identity Server
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

// Add Ocelot to services
builder.Services.AddOcelot();

var app = builder.Build();

// Use necessary middleware
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();
app.UseOcelot().Wait();    // Ocelot processes the request

app.Run();
