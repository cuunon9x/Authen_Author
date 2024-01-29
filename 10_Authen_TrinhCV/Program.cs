using _10_Authen_TrinhCV.Data.DB.EF;
using _10_Authen_TrinhCV.Services;
using _10_Authen_TrinhCV.ServicesCommon;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
    builder =>
    {
        builder.WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDb"));
});
builder.Services.AddTransient<IUserServices, UserServices>();
builder.Services.AddTransient<IRoleServices, RoleServices>();
builder.Services.AddTransient<ISecurityService, JwtSecurityService>();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
