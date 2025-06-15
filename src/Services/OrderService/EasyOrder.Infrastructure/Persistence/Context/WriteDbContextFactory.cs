using System;
using System.IO;
using EasyOrder.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EasyOrder.Infrastructure.Persistence.Context
{
    /// <summary>
    /// Provides EF Core tools (Add-Migration, Update-Database) a way to create WriteDbContext at design time.
    /// </summary>
    public class WriteDbContextFactory : IDesignTimeDbContextFactory<WriteDbContext>
    {
        public WriteDbContext CreateDbContext(string[] args)
        {
            // 1) Determine where your API project's appsettings.json lives.
            //    We assume the folder structure:
            //    src\Services\OrderService\EasyOrder.Infrastructure  <-- current
            //                         \EasyOrder.Api               <-- settings here
            var infrastructureDir = Directory.GetCurrentDirectory();
            var serviceDir = Path.GetDirectoryName(infrastructureDir);
            var apiDir = Path.Combine(serviceDir!, "EasyOrder.Api");

            // 2) Build configuration from that folder
            var config = new ConfigurationBuilder()
                .SetBasePath(apiDir)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                // optionally: .AddJsonFile($"appsettings.{env}.json", optional: true)
                .Build();

            // 3) Read the Write-DB connection string
            var connStr = config.GetConnectionString("WriteDatabaseOrder");
            if (string.IsNullOrWhiteSpace(connStr))
                throw new InvalidOperationException("Connection string 'WriteDatabaseOrder' not found in API appsettings.json");

            // 4) Create and configure the DbContextOptions
            var builder = new DbContextOptionsBuilder<WriteDbContext>();
            builder.UseSqlServer(
                connStr,
                sql => sql.MigrationsAssembly(typeof(WriteDbContext).Assembly.GetName().Name)
            );

            // 5) Return the context
            return new WriteDbContext(builder.Options);
        }
    }
}
