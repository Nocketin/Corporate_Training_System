using CourseService.Application.Abstractions;
using CourseService.Application.Caching;
using CourseService.Infrastructure.Caching;
using CourseService.Infrastructure.Messaging;
using CourseService.Infrastructure.Persistence;
using CourseService.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CourseService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCourseInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioS3Options>(configuration.GetSection("Minio"));
        services.AddSingleton<IObjectStorage, MinioS3ObjectStorage>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<CourseDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            // EF 9 throws if the snapshot and runtime model differ on metadata; schema SQL may still match.
            options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:Configuration"];
        });

        services.AddScoped<ICourseReadRepository, EfCourseReadRepository>();
        services.AddScoped<ILessonResourceReadRepository, EfLessonResourceReadRepository>();
        services.AddScoped<ICourseWriteRepository, EfCourseWriteRepository>();
        services.AddScoped<IOutboxWriter, EfOutboxWriter>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CourseDbContext>());
        services.AddScoped<ICourseCacheInvalidator, RedisCourseCacheInvalidator>();

        services.AddHostedService<OutboxDispatcherHostedService>();

        return services;
    }
}
