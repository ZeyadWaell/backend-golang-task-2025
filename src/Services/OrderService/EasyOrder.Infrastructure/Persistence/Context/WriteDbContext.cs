using EasyOrder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Infrastructure.Persistence.Context
{
    public class WriteDbContext : DbContext, IAppDbContext
    {
        public WriteDbContext(DbContextOptions<WriteDbContext> options)
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


            builder.Entity<Order>().ToTable("Orders", tb =>
            {
                tb.HasTrigger("trg_SyncOrders");
                tb.UseSqlOutputClause(false);
            });

            // OrderItems
            builder.Entity<OrderItem>().ToTable("OrderItems", tb =>
            {
                tb.HasTrigger("trg_SyncOrderItems");
                tb.UseSqlOutputClause(false);
            });

            // Payments
            builder.Entity<Payment>().ToTable("Payments", tb =>
            {
                tb.HasTrigger("trg_SyncPayments");
                tb.UseSqlOutputClause(false);
            });

            // e.g. builder.ApplyConfiguration(new OrderConfiguration());
        }
    }
  }
