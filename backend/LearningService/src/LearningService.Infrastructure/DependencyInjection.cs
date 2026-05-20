using LearningService.Application.Abstractions;
using LearningService.Infrastructure.Catalog;
using LearningService.Infrastructure.Messaging;
using LearningService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LearningService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddLearningInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<LearningDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        services.AddScoped<ILearningRepository, EfLearningRepository>();
        services.AddScoped<ILearningAnalyticsReadRepository, EfLearningAnalyticsReadRepository>();
        services.AddScoped<IOutboxWriter, EfOutboxWriter>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<LearningDbContext>());
        services.AddScoped<ICourseProjectionSync, CourseProjectionSync>();

        var courseBaseUrl = configuration["CourseService:BaseUrl"] ?? "http://localhost:5001";
        services.AddHttpClient<ICourseCatalogClient, CourseCatalogClient>(client =>
        {
            client.BaseAddress = new Uri(courseBaseUrl.TrimEnd('/') + "/");
        });

        services.AddHostedService<OutboxDispatcherHostedService>();
        services.AddHostedService<CoursePublishedConsumerHostedService>();

        return services;
    }
}
