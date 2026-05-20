using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Common.Messaging;

/// <summary>
/// Shared MassTransit registration. Kafka uses a <i>Rider</i> (see CourseService); EF transactional outbox in MassTransit targets bus transports (e.g. RabbitMQ),
/// so CourseService implements the same persistence pattern with a local <c>OutboxMessages</c> table plus rider producer dispatch.
/// </summary>
public static class MassTransitExtensions
{
    public static IServiceCollection AddPlatformMassTransit(this IServiceCollection services, Action<IBusRegistrationConfigurator> configure)
    {
        services.AddMassTransit(x => configure(x));
        return services;
    }
}