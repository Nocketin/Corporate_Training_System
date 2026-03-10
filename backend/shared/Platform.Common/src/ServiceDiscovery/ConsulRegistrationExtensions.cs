using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Common.ServiceDiscovery;

public static class ConsulRegistrationExtensions
{
    public static IApplicationBuilder UsePlatformConsul(this IApplicationBuilder app, IConfiguration configuration)
    {
        var consulService = new ConsulService(configuration);

        // регистрация при старте
        consulService.RegisterAsync().GetAwaiter().GetResult();

        // отписка при остановке
        var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStopping.Register(() =>
        {
            consulService.DeregisterAsync().GetAwaiter().GetResult();
        });

        return app;
    }
}