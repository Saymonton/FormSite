using FormSite.Models;
using Microsoft.EntityFrameworkCore;

namespace FormSite.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

    // === IMPORTANTE: estes DbSet<> precisam existir ===
    public DbSet<Option> Options => Set<Option>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Índice único: uma reservation por OptionId
        modelBuilder.Entity<Reservation>()
            .HasIndex(r => r.OptionId)
            .IsUnique();

        // Seed opcional (IDs fixos para não colidir)
        modelBuilder.Entity<Option>().HasData(
            new Option { Id = 1, Name = "Item A" },
            new Option { Id = 2, Name = "Item B" },
            new Option { Id = 3, Name = "Item C" }
        );
    }
}
