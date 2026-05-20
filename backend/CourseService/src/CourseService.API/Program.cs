using System.Text;
using CourseService.Application.Common.Behaviors;
using CourseService.Application.Courses.Handlers;
using CourseService.Application.Mapping;
using CourseService.Infrastructure;
using CourseService.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Platform.Common.Logging;
using Platform.Common.Observability;
using Platform.Common.ServiceDiscovery;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddPlatformLogging();

CourseMappingConfig.Register();

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
    cfg.RegisterServicesFromAssembly(typeof(GetCoursesQueryHandler).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(GetCoursesQueryHandler).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
builder.Services.AddProblemDetails();

builder.Services.AddCourseInfrastructure(builder.Configuration);
builder.Services.AddScoped<CourseService.Application.Courses.Services.LessonResourceProcessor>();

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

builder.Services.AddAuthorization();
builder.Services.AddPlatformMetrics("courseservice");

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CourseDbContext>();
    await db.Database.MigrateAsync();
    // Demo catalog is seeded by migration SeedDemoCourses; fallback if DB was created before that migration.
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("CourseDataSeeder");
    await CourseDataSeeder.SeedAsync(db, logger);
}

app.UsePlatformMetrics();
app.UsePlatformConsul(builder.Configuration);
app.MapGet("/health", () => Results.Ok());
// Browsers request /favicon.ico on any URL; without this, DevTools shows a harmless 404 next to a 200 JSON body.
app.MapMethods("/favicon.ico", new[] { "GET", "HEAD" }, () => Results.NoContent());

app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
        var (statusCode, title) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Validation failed"),
            InvalidOperationException => (StatusCodes.Status400BadRequest, "Invalid operation"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Not found"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error")
        };

        var detail = exception?.Message;
        if (exception is DbUpdateException dbEx)
        {
            detail = dbEx.InnerException is PostgresException pg
                ? FormatPostgresError(dbEx.Message, pg)
                : $"{dbEx.Message} | {dbEx.InnerException?.Message}";
        }
        else if (exception?.InnerException is PostgresException pgQuery)
        {
            detail = FormatPostgresError(exception.Message, pgQuery);
        }

        static string FormatPostgresError(string prefix, PostgresException pg)
        {
            var suffix = string.IsNullOrEmpty(pg.Detail) ? string.Empty : $" | {pg.Detail}";
            return $"{prefix} | {pg.SqlState}: {pg.MessageText}{suffix}";
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
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
