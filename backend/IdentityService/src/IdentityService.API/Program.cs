using System.Text;
using FluentValidation;
using IdentityService.Application.Common.Behaviors;
using IdentityService.Application.Common.Exceptions;
using IdentityService.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Platform.Common.Logging;
using Platform.Common.Observability;
using Platform.Common.ServiceDiscovery;
using IdentityService.Application.Auth.Handlers;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommandHandler).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserCommandHandler).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddProblemDetails();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPlatformMetrics("identity");

var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
});

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    var startupLogger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    var pending = await db.Database.GetPendingMigrationsAsync();
    var pendingList = pending.ToList();
    if (pendingList.Count > 0)
    {
        startupLogger.LogInformation("Applying pending Identity migrations: {Migrations}", string.Join(", ", pendingList));
    }

    await db.Database.MigrateAsync();
    try
    {
        await AdminSeed.SeedAsync(db, builder.Configuration, startupLogger);
    }
    catch (Exception ex)
    {
        startupLogger.LogWarning(ex, "AdminSeed skipped due to error.");
    }
}

app.UsePlatformMetrics();
app.UsePlatformConsul(builder.Configuration);
app.MapGet("/health", () => Results.Ok());

app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
        var (statusCode, title) = exception switch
        {
            IdentityService.Application.Common.Exceptions.ValidationException => (StatusCodes.Status400BadRequest, "Validation failed"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception?.Message,
            Instance = context.Request.Path
        };

        if (exception is IdentityService.Application.Common.Exceptions.ValidationException validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();