using Microsoft.EntityFrameworkCore;
using NotificationService.Infrastructure;
using NotificationService.Infrastructure.Persistence;
using Platform.Common.Logging;
using Platform.Common.Observability;
using Platform.Common.ServiceDiscovery;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddPlatformLogging();
builder.Services.AddNotificationInfrastructure(builder.Configuration);
builder.Services.AddPlatformMetrics("notificationservice");

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    await db.Database.MigrateAsync();
}

app.UsePlatformMetrics();
app.UsePlatformConsul(builder.Configuration);
app.MapGet("/health", () => Results.Ok());

app.Run();
