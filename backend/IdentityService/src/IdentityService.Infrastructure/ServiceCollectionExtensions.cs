using IdentityService.Application.Auth.Abstractions;
using IdentityService.Domain;
using IdentityService.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IOutboxWriter, Persistence.EfOutboxWriter>();
            services.AddScoped<Application.Common.Abstractions.IUnitOfWork>(sp =>
                sp.GetRequiredService<IdentityDbContext>());

            services.AddHostedService<Messaging.OutboxDispatcherHostedService>();

            return services;
        }
    }
}