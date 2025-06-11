using EasyOrder.Infrastructure.Persistence.Repositories.Main;
using EasyOrderProduct.Application.Contracts.Interfaces.Main;
using EasyOrderProduct.Infrastructure.Persistence.Context;
using EasyOrderProduct.Infrastructure.Persistence.Repositories.Main;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace EasyOrderProduct.Infrastructure.Extentions
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
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static void AddServices(IServiceCollection services) 
        {

        }
    }
}
