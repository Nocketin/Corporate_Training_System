using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.API;

public class GatewayProxyErrorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GatewayProxyErrorMiddleware> _logger;

    public GatewayProxyErrorMiddleware(RequestDelegate next, ILogger<GatewayProxyErrorMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.Response.HasStarted)
        {
            return;
        }

        if (context.Response.StatusCode is not (502 or 503 or 504))
        {
            return;
        }

        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            return;
        }

        _logger.LogWarning(
            "Upstream unavailable for {Method} {Path}, status {StatusCode}",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode);

        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status503ServiceUnavailable,
            Title = "Service temporarily unavailable",
            Detail = "The downstream microservice is unavailable or the circuit breaker is open. Please try again later.",
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(problem);
    }
}
