using IdentityService.API.Extensions;
using IdentityService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddIdentityAndIdentityServer(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

//Jwt authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Thêm middleware JWT Authentication và Authorization
app.UseAuthentication();  // Đảm bảo rằng app sẽ thực hiện xác thực
app.UseAuthorization();   // Đảm bảo rằng app sẽ kiểm tra quyền

app.UseIdentityServer();

app.MapControllers();
app.Run();
