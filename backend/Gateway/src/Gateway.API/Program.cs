using Consul;
using Gateway.API;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Yarp.ReverseProxy;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddReverseProxy();

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

var app = builder.Build();

app.UseCors();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Detail = exceptionHandlerPathFeature?.Error.Message
        };

        context.Response.StatusCode = problem.Status.Value;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    });
});

app.MapReverseProxy();

app.Run();

