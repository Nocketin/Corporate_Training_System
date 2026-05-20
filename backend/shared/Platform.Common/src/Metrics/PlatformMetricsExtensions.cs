using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;

namespace Platform.Common.Observability;

public static class PlatformMetricsExtensions
{
    private static readonly HashSet<string> ExcludedPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/health",
        "/metrics",
        "/favicon.ico"
    };

    public static IServiceCollection AddPlatformMetrics(this IServiceCollection services, string serviceName)
    {
        services.AddSingleton(new PlatformMetricsOptions(serviceName));
        return services;
    }

    public static WebApplication UsePlatformMetrics(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<PlatformMetricsOptions>();
        var serviceName = options.ServiceName;

        var requestsTotal = Metrics.CreateCounter(
            "cts_http_requests_total",
            "Total HTTP requests processed.",
            new CounterConfiguration
            {
                LabelNames = ["service", "method", "code"]
            });

        var requestDuration = Metrics.CreateHistogram(
            "cts_http_request_duration_seconds",
            "HTTP request duration in seconds.",
            new HistogramConfiguration
            {
                LabelNames = ["service", "method", "code"],
                Buckets = Histogram.ExponentialBuckets(0.001, 2, 16)
            });

        var errorsTotal = Metrics.CreateCounter(
            "cts_http_errors_total",
            "Total HTTP responses with status code >= 400.",
            new CounterConfiguration
            {
                LabelNames = ["service", "method", "code"]
            });

        app.Use(async (context, next) =>
        {
            if (ExcludedPaths.Contains(context.Request.Path.Value ?? string.Empty))
            {
                await next();
                return;
            }

            var method = context.Request.Method;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                await next();
            }
            finally
            {
                stopwatch.Stop();
                var code = context.Response.StatusCode.ToString();
                requestsTotal.WithLabels(serviceName, method, code).Inc();
                requestDuration.WithLabels(serviceName, method, code).Observe(stopwatch.Elapsed.TotalSeconds);

                if (context.Response.StatusCode >= 400)
                {
                    errorsTotal.WithLabels(serviceName, method, code).Inc();
                }
            }
        });

        app.UseHttpMetrics();
        app.MapMetrics("/metrics");

        return app;
    }
}

public sealed record PlatformMetricsOptions(string ServiceName);
