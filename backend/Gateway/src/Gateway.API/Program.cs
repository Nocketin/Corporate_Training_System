using Consul;
using Gateway.API;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Http.Resilience;
using Platform.Common.Logging;
using Platform.Common.Observability;
using Yarp.ReverseProxy;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddPlatformLogging();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:3001")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services
    .AddReverseProxy()
    .ConfigureHttpClient((_, handler) =>
    {
        handler.ConnectTimeout = TimeSpan.FromSeconds(5);
        handler.ActivityHeadersPropagator = null;
    });

builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler(options =>
    {
        options.Retry.MaxRetryAttempts = 2;
        options.Retry.Delay = TimeSpan.FromMilliseconds(200);
        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.MinimumThroughput = 5;
        options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
    });
});

builder.Services.AddProblemDetails();

builder.Services.AddSingleton<IConsulClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var consulAddress = configuration["Consul:Address"] ?? "http://localhost:8500";
    return new ConsulClient(config =>
    {
        config.Address = new Uri(consulAddress);
    });
});

builder.Services.AddSingleton<IProxyConfigProvider, ConsulYarpConfigProvider>();

builder.Services.AddControllers();
builder.Services.AddPlatformMetrics("gateway");

var app = builder.Build();

app.UsePlatformMetrics();
app.UseCors();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var statusCode = StatusCodes.Status503ServiceUnavailable;
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = "Gateway request processing failed.",
            Detail = exceptionHandlerPathFeature?.Error.Message
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    });
});

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));
app.UseMiddleware<GatewayProxyErrorMiddleware>();
app.MapReverseProxy();

app.Run();

