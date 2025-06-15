using EasyOrderProduct.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;


namespace EasyOrderProduct.Infrastructure.Persistence.Context
{
    public class WriteDbContext : DbContext, IAppDbContext
    {
        public WriteDbContext(DbContextOptions<WriteDbContext> options)
            : base(options)
        { }

        public DbSet<Product> Product { get; set; }
        public DbSet<ProductItem> ProductItem { get; set; }
        public DbSet<ProductItemOption> ProductItemOption { get; set; }
        public DbSet<Variation> Variation { get; set; }
        public DbSet<VariationOption> VariationOption { get; set; }
        public DbSet<Inventory> inventories { get; set; }

        DbSet<TEntity> IAppDbContext.Set<TEntity>() => Set<TEntity>();

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => base.SaveChangesAsync(cancellationToken);

        public DatabaseFacade Database => base.Database;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProductItem>().ToTable("ProductItem", tb =>
            {
                tb.HasTrigger("trg_SyncProductItem");
                tb.UseSqlOutputClause(false);
            });

            builder.Entity<Variation>().ToTable("Variation", tb =>
            {
                tb.HasTrigger("trg_SyncVariation");
                tb.UseSqlOutputClause(false);
            });


            builder.Entity<Inventory>().ToTable("inventories", tb =>
            {
                tb.HasTrigger("trg_SyncInventory");
                tb.UseSqlOutputClause(false);
            });

            // OrderItems
            builder.Entity<Product>().ToTable("Product", tb =>
            {
                tb.HasTrigger("trg_SyncProduct");
                tb.UseSqlOutputClause(false);
            });


            builder.Entity<ProductItemOption>()
    .HasOne(pio => pio.VariationOption)
    .WithMany(vo => vo.ProductItemOptions)
    .HasForeignKey(pio => pio.VariationOptionId)
    .OnDelete(DeleteBehavior.Restrict);
            // e.g. builder.ApplyConfiguration(new OrderConfiguration());
        }
    }
  }
