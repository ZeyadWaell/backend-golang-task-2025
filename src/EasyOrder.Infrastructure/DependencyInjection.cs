// File: DependencyInjection.cs
// Project: EasyOrder.Infrastructure
// Namespace: EasyOrder.Infrastructure

using EasyOrder.Application.Contracts.Interfaces.Main;
using EasyOrder.Application.Contracts.Interfaces.Repository;
using EasyOrder.Application.Contracts.Interfaces.Services;
using EasyOrder.Application.Queries.Services;
using EasyOrder.Infrastructure.Persistence.Context;        // ReadDbContext, WriteDbContext
using EasyOrder.Infrastructure.Persistence.Repositories;   // OrderRepository
using EasyOrder.Infrastructure.Persistence.Repositories.Main; // GenericRepository<>
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyOrder.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
        {
            AddingHealthCheck(services, configuration);
            AddHangfireString(services, configuration);
            AddingClientConnection(services);
            AddIdentity(services);
            AddingHangfireString(services);
            AddMapping(services);
            AddConfiguration(services, configuration);
            AddDatabaseContext(services, configuration);
            AddRepositories(services);
            AddServices(services);

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

        private static void AddingHealthCheck(IServiceCollection services, IConfiguration configuration) { }
        private static void AddHangfireString(IServiceCollection services, IConfiguration configuration) { }
        private static void AddingClientConnection(IServiceCollection services) { }
        private static void AddIdentity(IServiceCollection services) { }
        private static void AddingHangfireString(IServiceCollection services) { }
        private static void AddServices(IServiceCollection services) 
        {
            services.AddScoped<IOrderService, OrderService>();
        }
        private static void AddMapping(IServiceCollection services) { }
        private static void AddConfiguration(IServiceCollection services, IConfiguration configuration) { }
    }
}
