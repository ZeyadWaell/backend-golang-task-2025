using EasyOrder.Infrastructure.Persistence.Repositories.Main;
using EasyOrderProduct.Application.Contract.Interfaces.GrpsServices;
using EasyOrderProduct.Application.Contract.Interfaces.Repository;
using EasyOrderProduct.Application.Contract.Interfaces.Services;
using EasyOrderProduct.Application.Contract.Services;
using EasyOrderProduct.Application.Contracts.Interfaces.InternalServices;
using EasyOrderProduct.Application.Contracts.Interfaces.Main;
using EasyOrderProduct.Application.Contracts.Protos;
using EasyOrderProduct.Application.Contracts.Services;
using EasyOrderProduct.Infrastructure.Persistence.Context;
using EasyOrderProduct.Infrastructure.Persistence.Repositories;
using EasyOrderProduct.Infrastructure.Persistence.Repositories.Main;
using EasyOrderProduct.Infrastructure.Services.Internal;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace EasyOrderProduct.Infrastructure.Extentions;

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
