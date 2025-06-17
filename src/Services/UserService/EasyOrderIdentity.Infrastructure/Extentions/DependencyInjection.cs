using EasyOrderIdentity.Application.Interfaces;
using EasyOrderIdentity.Application.Services.EasyOrderIdentity.Infrastructure.Services;
using EasyOrderIdentity.Domain.Entites;
using EasyOrderIdentity.Infrastructure.Interceptors;
using EasyOrderIdentity.Infrastructure.Persistence.Context;
using EasyOrderIdentity.Infrastructure.Seed.Identity;
using EasyOrderIdentity.Infrastructure.Services.Internal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EasyOrderIdentity.Infrastructure.ProgramServices
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddServices(services);
            AddIdentity(services, configuration);
            return services;
        }

        private static void AddServices(IServiceCollection services)
        {
            // register your interceptor here
            services.AddScoped<AuditableEntityInterceptor>();
            services.AddSignalR();

            // other application services
            services.AddScoped<IOwnershipService, OwnershipService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJWtHelper, JwtHelper>();
        }

        private static void AddIdentity(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>((provider, options) =>
            {
                options.UseSqlServer(configuration.GetConnectionString("IdentityDataBaseConnection"))
                       .AddInterceptors(provider.GetRequiredService<AuditableEntityInterceptor>());
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IdentitySeeder>();
        }
    }
}
