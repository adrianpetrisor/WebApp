using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

var builder = WebApplication.CreateBuilder(args);

var logFolder = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
Directory.CreateDirectory(logFolder);

var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
var logFilePath = Path.Combine(logFolder, $"log-{timestamp}.txt");

builder.Services.AddSingleton<IAppLogger, AppLogger>();
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddFileLogger(logFilePath);


builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("DataSource=C:\\Users\\negatiwe\\source\\repos\\WebApp\\webapp.db"));


builder.Services.AddRazorPages();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
logger.LogInformation("The system is now operational!");
