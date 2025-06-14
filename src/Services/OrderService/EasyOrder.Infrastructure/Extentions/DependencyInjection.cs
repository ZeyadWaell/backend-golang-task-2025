
using EasyOrder.Application.Contracts.Interfaces.GrpcServices;
using EasyOrder.Application.Contracts.Interfaces.InternalServices;
using EasyOrder.Application.Contracts.Interfaces.Main;
using EasyOrder.Application.Contracts.Interfaces.Repository;
using EasyOrder.Application.Contracts.Interfaces.Services;
using EasyOrder.Application.Contracts.Services;
using EasyOrder.Application.Queries.Services;
using EasyOrder.Infrastructure.GrpcClients;
using EasyOrder.Infrastructure.Persistence.Context;
using EasyOrder.Infrastructure.Persistence.Repositories;
using EasyOrder.Infrastructure.Persistence.Repositories.Main;
using EasyOrder.Infrastructure.Services.Internal;
using EasyOrderProduct.Application.Contracts.Protos;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace EasyOrder.Infrastructure.Extentions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
        {
            AddGrpcClients(services, configuration);
            AddGrpcsServices(services);
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
        private static void AddGrpcClients(IServiceCollection services, IConfiguration configuration)
        {
            var inventoryUrl = configuration["GrpcSettings:InventoryUrl"];

            services.AddSingleton(services =>
            {
                var channel = GrpcChannel.ForAddress(inventoryUrl);
                return new InventoryChecker.InventoryCheckerClient(channel);
            });
        }
        private static void AddGrpcsServices(IServiceCollection services)
        {
            services.AddScoped<IInventoryChecker, GrpcInventoryChecker>();
        }
        private static void AddServices(IServiceCollection services) 
        {
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IOrderService, OrderService>();
        }
    }
}
