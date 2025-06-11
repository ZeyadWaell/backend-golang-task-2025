using EasyOrderIdentity.Domain.Entites;
using EasyOrderIdentity.Infrastructure.Interceptors;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System.Reflection.Emit;

namespace EasyOrderIdentity.Infrastructure.Persistence.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IConfiguration _configuration;
        private readonly AuditableEntityInterceptor _auditableEntityInterceptor;
        public AppDbContext(DbContextOptions<AppDbContext> options, AuditableEntityInterceptor auditableEntityInterceptor, IConfiguration configuration) : base(options) 
        {
            _auditableEntityInterceptor = auditableEntityInterceptor;
            _configuration = configuration;

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            SoftDelete(builder);

            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    _configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Cleaning.Infrastructure")
                );
            }
            optionsBuilder.AddInterceptors(_auditableEntityInterceptor);
        }

        private void SoftDelete(ModelBuilder modelbuilder)
        {
            //modelbuilder.Entity<Property>().HasQueryFilter(p => !p.IsDeleted);
            //modelbuilder.Entity<Room>().HasQueryFilter(r => !r.IsDeleted);
            //modelbuilder.Entity<Reservation>().HasQueryFilter(r => !r.IsDeleted);
            //modelbuilder.Entity<RatePlan>().HasQueryFilter(r => !r.IsDeleted);
            //modelbuilder.Entity<Reservation>().HasQueryFilter(r => !r.IsDeleted);
            //modelbuilder.Entity<RoomDateAvailability>().HasQueryFilter(r => !r.IsDeleted);
            base.OnModelCreating(modelbuilder);

        }
    }
}
