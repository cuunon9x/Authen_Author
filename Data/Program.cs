using _10_Authen_TrinhCV.Data.DB.EF;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDb"));
});
app.MapGet("/", () => "Hello World!");

app.Run();
