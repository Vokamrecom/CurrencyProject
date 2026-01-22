using CurrencyProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyProject.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Currency> Currencies { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Rate).HasPrecision(18, 4);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
            
            entity.HasMany(e => e.Favorites)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "UserCurrency",
                    j => j.HasOne<Currency>().WithMany().HasForeignKey("CurrencyId"),
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j => j.HasKey("UserId", "CurrencyId"));
        });
    }
}
