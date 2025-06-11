// File: DependencyInjection.cs
// Project: EasyOrder.Infrastructure
// Namespace: EasyOrder.Infrastructure

using EasyOrder.Application.Contracts.Interfaces.InternalServices;
using EasyOrder.Application.Contracts.Interfaces.Main;
using EasyOrder.Application.Contracts.Interfaces.Repository;
using EasyOrder.Application.Contracts.Interfaces.Services;
using EasyOrder.Application.Queries.Services;
using EasyOrder.Infrastructure.Persistence.Context;        // ReadDbContext, WriteDbContext
using EasyOrder.Infrastructure.Persistence.Repositories;   // OrderRepository
using EasyOrder.Infrastructure.Persistence.Repositories.Main; // GenericRepository<>
using EasyOrder.Infrastructure.Services.Internal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EasyOrder.Infrastructure.Extentions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
        {
            AddServices(services);
            AddDatabaseContext(services, configuration);
            AddRepositories(services);

            return services;
        }

        // ----- PRIVATE HELPERS -----

        private static void AddDatabaseContext(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ReadDbContext>(opts =>
                opts.UseSqlServer(configuration.GetConnectionString("ReadDatabase")));
            services.AddDbContext<WriteDbContext>(opts =>
                opts.UseSqlServer(configuration.GetConnectionString("WriteDatabase")));
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static void AddServices(IServiceCollection services) 
        {
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IOrderService, OrderService>();
        }
    }
}
