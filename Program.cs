using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

var logFolder = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
Directory.CreateDirectory(logFolder);
var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
var logFilePath = Path.Combine(logFolder, $"log-{timestamp}.txt");

builder.Services.AddSingleton<IAppLogger, AppLogger>();
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
//builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);
builder.Logging.AddFileLogger(logFilePath);

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var connectionString = "Server=localhost;Port=3306;Database=webapp;User=root;Password=mysqlroot;";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

builder.Services.AddRazorPages();
builder.Services.AddScoped<AuthorizationService>();
builder.Services.AddScoped<RoleService>();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

if (app.Environment.IsDevelopment())
{
    logger.LogWarning("Web Application has been started in development environment...");
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await DbInitializer.SeedRolesAsync(roleManager);
    }
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

logger.LogInformation("Starting web application...");
app.Run();
