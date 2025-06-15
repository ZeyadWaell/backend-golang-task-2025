using EasyOrder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Infrastructure.Persistence.Context
{
    public class ReadDbContext : DbContext, IAppDbContext
    {
        public ReadDbContext(DbContextOptions<ReadDbContext> options)
            : base(options)
        { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        DbSet<TEntity> IAppDbContext.Set<TEntity>() => Set<TEntity>();

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => base.SaveChangesAsync(cancellationToken);

        public DatabaseFacade Database => base.Database;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

    //        builder.Entity<Order>()
    //.ToTable(tb => tb.HasTrigger("trg_SyncOrders"));
    //        builder.Entity<OrderItem>()
    //            .ToTable(tb => tb.HasTrigger("trg_SyncOrderItems"));
    //        builder.Entity<Payment>()
    //            .ToTable(tb => tb.HasTrigger("trg_SyncPayments"));

            // configure your projections:
            // builder.Entity<OrderReadModel>().HasKey(x => x.Id);
        }
    }
}
