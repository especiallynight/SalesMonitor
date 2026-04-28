using Microsoft.EntityFrameworkCore;
using SalesMonitor.Data;
using SalesMonitor.Hubs;
using SalesMonitor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<DataSeeder>();
builder.Services.AddHostedService<ChurnMonitorService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapHub<AlertHub>("/alertHub");
app.MapFallbackToPage("/_Host");

app.Run();