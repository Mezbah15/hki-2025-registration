using System.Net;
using hki_2025_registration.Models;
using hki_2025_registration.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();

var serverVersion = new MySqlServerVersion(new Version(8, 0, 40));
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseMySql(builder.Configuration.GetConnectionString("MySQLConnection"), serverVersion));

builder.Services.Configure<BkashSettings>(builder.Configuration.GetSection("BkashSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddMemoryCache();
builder.Services.AddScoped<BkashService>();

var app = builder.Build();

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
