using EasyOrderProduct.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EasyOrderProduct.Infrastructure.Persistence.Context
{
    public class ReadDbContext : DbContext, IAppDbContext
    {
        public ReadDbContext(DbContextOptions<ReadDbContext> options)
            : base(options)
        { }

        public DbSet<Product> Product { get; set; }
        public DbSet<ProductItem> ProductItem { get; set; }
        public DbSet<ProductItemOption> ProductItemOption { get; set; }
        public DbSet<Variation> Variation { get; set; }
        public DbSet<VariationOption> VariationOption { get; set; }


        DbSet<TEntity> IAppDbContext.Set<TEntity>() => Set<TEntity>();

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => base.SaveChangesAsync(cancellationToken);

        public DatabaseFacade Database => base.Database;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // configure your projections:
            // builder.Entity<OrderReadModel>().HasKey(x => x.Id);
        }
    }
}
