using Microsoft.EntityFrameworkCore;
using SalesSystem.Domain.Entities;

namespace SalesSystem.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relación: Sale → SaleItems (Cascade Delete)
        modelBuilder.Entity<Sale>()
            .HasMany(s => s.SaleItems)
            .WithOne()
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación: SaleItem → Product (sin navegación obligatoria)
        modelBuilder.Entity<SaleItem>()
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(si => si.ProductId);

        // Seed: Usuario admin
        var adminId = new Guid("11111111-1111-1111-1111-111111111111");
        var passwordHash = ComputeHash("admin123");
        
        modelBuilder.Entity<User>().HasData(
            new User("admin", "admin@salessystem.com", passwordHash) 
            { 
                Id = adminId
            }
        );
    }

    private static string ComputeHash(string input)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLower();
    }
}
