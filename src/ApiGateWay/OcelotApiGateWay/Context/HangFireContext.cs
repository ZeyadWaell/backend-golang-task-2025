using Microsoft.EntityFrameworkCore;
using OcelotApiGateWay.Entites;

namespace OcelotApiGateWay.Context
{
    public class HangFireContext : DbContext
    {
        public HangFireContext(DbContextOptions<HangFireContext> options)
            : base(options) { }

        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RequestPath).IsRequired();
                entity.Property(e => e.RequestBody).IsRequired(false);
                entity.Property(e => e.Response).IsRequired();
                entity.Property(e => e.ResponseCode).IsRequired();
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
