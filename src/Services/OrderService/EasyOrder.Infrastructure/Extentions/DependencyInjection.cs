
using EasyOrder.Application.Contracts.Interfaces.GrpcServices;
using EasyOrder.Application.Contracts.Interfaces.InternalServices;
using EasyOrder.Application.Contracts.Interfaces.Main;
using EasyOrder.Application.Contracts.Interfaces.Repository;
using EasyOrder.Application.Contracts.Interfaces.Services;
using EasyOrder.Application.Contracts.Messaging;
using EasyOrder.Application.Contracts.Services;
using EasyOrder.Application.Queries.Services;
using EasyOrder.Application.Services;
using EasyOrder.Infrastructure.GrpcClients;
using EasyOrder.Infrastructure.Persistence.Context;
using EasyOrder.Infrastructure.Persistence.Repositories;
using EasyOrder.Infrastructure.Persistence.Repositories.Main;
using EasyOrder.Infrastructure.Sagas;
using EasyOrder.Infrastructure.Services.Internal;
using EasyOrderProduct.Application.Contracts.Protos;
using Grpc.Net.Client;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Routing.TypeBased;


namespace EasyOrder.Infrastructure.Extentions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddGrpcClients(services, configuration);
            AddGrpcsServices(services);
            AddServices(services);
            AddDatabaseContext(services, configuration);
            AddRepositories(services);
            AddHandFireString(services, configuration);
            return services;
        }

        // ----- PRIVATE HELPERS -----

        private static void AddDatabaseContext(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ReadDbContext>(opts =>
                opts.UseSqlServer(configuration.GetConnectionString("ReadDatabaseOrder")));
            services.AddDbContext<WriteDbContext>(opts =>
                opts.UseSqlServer(configuration.GetConnectionString("WriteDatabaseOrder")));

            services.AddDbContext<ApplicationDbHandFireContext>(options =>
options.UseSqlServer(configuration.GetConnectionString("HangfireConnection"),
  b => b.MigrationsAssembly("EasyOrder.Infrastructure")));
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
        private static void AddHandFireString(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(cfg => cfg.UseSimpleAssemblyNameTypeSerializer().UseRecommendedSerializerSettings().UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.FromSeconds(15),
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            }
)
);
            services.AddSignalR();

            services.AddHangfireServer();
        }
        //private static void AddingMessaging(IServiceCollection services, IConfiguration configuration)
        //{
        //    services.AddRebus(busConfig => busConfig
        //        .Transport(t => t.UseRabbitMq(configuration["RabbitMq"], "order-queue"))
        //        .Routing(r => r.TypeBased()
        //            .Map<SagaMessages.ReserveInventory>("inventory-queue")
        //            .Map<SagaMessages.ChargePayment>("payment-queue")
        //            .Map<SagaMessages.ReleaseInventory>("inventory-queue"))
        //        .Sagas(s => s.StoreInSqlServer(
        //            configuration.GetConnectionString("WriteDb"), // your DB conn
        //            dataTableName: "OrderSagas",                 // saga data table
        //            indexTableName: "OrderSagasIndex",            // saga index table
        //            automaticallyCreateTables: true))
        //    );
        //    services.AutoRegisterHandlersFromAssemblyOf<OrderSaga>();
        //}
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
            //   services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IOrderService, OrderService>();
        }
    }
}
