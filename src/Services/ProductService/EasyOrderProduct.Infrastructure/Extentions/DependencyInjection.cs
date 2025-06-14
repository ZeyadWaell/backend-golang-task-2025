using EasyOrder.Infrastructure.Persistence.Repositories.Main;
using EasyOrderProduct.Application.Contract.Interfaces.GrpsServices;
using EasyOrderProduct.Application.Contract.Interfaces.Repository;
using EasyOrderProduct.Application.Contract.Interfaces.Services;
using EasyOrderProduct.Application.Contract.Messaging;
using EasyOrderProduct.Application.Contract.Services;
using EasyOrderProduct.Application.Contracts.Interfaces.InternalServices;
using EasyOrderProduct.Application.Contracts.Interfaces.Main;
using EasyOrderProduct.Application.Contracts.Protos;
using EasyOrderProduct.Application.Contracts.Services;
using EasyOrderProduct.Infrastructure.Messaging.Handlers;
using EasyOrderProduct.Infrastructure.Persistence.Context;
using EasyOrderProduct.Infrastructure.Persistence.Repositories;
using EasyOrderProduct.Infrastructure.Persistence.Repositories.Main;
using EasyOrderProduct.Infrastructure.Services.Internal;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Routing.TypeBased;


namespace EasyOrderProduct.Infrastructure.Extentions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        AddServices(services);
        AddDatabaseContext(services, configuration);
        AddRepositories(services);
        AddingMessaging(services, configuration); // Ensure this method is called

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

    private static void AddingMessaging(IServiceCollection services, IConfiguration configuration)
    {
        services.AddRebus(cfg => cfg
                   .Transport(t => t.UseRabbitMq(
                       configuration["RabbitMq:ConnectionString"], "inventory-queue"))
                   .Routing(r => r.TypeBased()
                       .Map<ReserveInventory>("inventory-queue")
                       .Map<ReleaseInventory>("inventory-queue"))
                   .Sagas(s => s.StoreInSqlServer(
            configuration.GetConnectionString("WriteDb"), // your DB conn
            dataTableName: "OrderSagas",                 // saga data table
            indexTableName: "OrderSagasIndex",            // saga index table
            automaticallyCreateTables: true))
    );

        services.AutoRegisterHandlersFromAssemblyOf<ReserveInventoryHandler>();
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IProductItemRepository, ProductItemRepository>();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
    }
}
