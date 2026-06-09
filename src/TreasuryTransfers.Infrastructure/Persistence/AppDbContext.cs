using Microsoft.EntityFrameworkCore;
using TreasuryTransfers.Domain.Entities;

namespace TreasuryTransfers.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<LedgerTransaction> LedgerTransactions => Set<LedgerTransaction>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LedgerTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OperationId).IsRequired();
            entity.HasIndex(e => e.OperationId).IsUnique();
            entity.Property(e => e.Status).IsRequired()
                .HasConversion<string>();
            entity.Property(e => e.SourceAccountId).IsRequired();
            entity.Property(e => e.TargetAccountId).IsRequired();
            entity.Property(e => e.AmountDebited).HasColumnType("decimal(18,2)");
            entity.Property(e => e.AmountCredited).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedAt).IsRequired();
        });
    }
}
