using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Common.Messaging;

public static class MassTransitExtensions
{
    public static void AddPlatformMassTransit(this IServiceCollection services, Action<IBusRegistrationConfigurator> configure)
    {
        services.AddMassTransit(x =>
        {
            configure(x);
        });
    }
}