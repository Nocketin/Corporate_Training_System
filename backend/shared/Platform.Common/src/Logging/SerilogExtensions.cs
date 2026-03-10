using Microsoft.Extensions.Hosting;
using Serilog;

namespace Platform.Common.Logging;

public static class SerilogExtensions
{
    public static IHostBuilder AddPlatformLogging(this IHostBuilder builder)
    {
        return builder.UseSerilog((context, config) =>
        {
            config.ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext();
        });
    }
}