using System;
using System.IO;
using EasyOrder.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EasyOrder.Infrastructure.Persistence.Context
{
    public class ApplicationDbHandFireContextFactory
        : IDesignTimeDbContextFactory<ApplicationDbHandFireContext>
    {
        public ApplicationDbHandFireContext CreateDbContext(string[] args)
        {
            // 1) Starting in Infrastructure folder:
            var infrastructureDir = Directory.GetCurrentDirectory();
            // 2) Move up one to the OrderService folder:
            var serviceDir = Directory.GetParent(infrastructureDir)!.FullName;
            // 3) Point to the Api project folder:
            var apiDir = Path.Combine(serviceDir, "EasyOrder.Api");

            var config = new ConfigurationBuilder()
                .SetBasePath(apiDir)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var conn = config.GetConnectionString("HangfireConnection");
            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException("ConnectionStrings:HangfireConnection not found in Api/appsettings.json");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbHandFireContext>();
            optionsBuilder.UseSqlServer(
                conn,
                sql => sql.MigrationsAssembly(typeof(ApplicationDbHandFireContext).Assembly.GetName().Name)
            );

            return new ApplicationDbHandFireContext(optionsBuilder.Options);
        }
    }
}
