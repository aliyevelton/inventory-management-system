using Inventory_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Inventory_Management_System.Contexts;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Brand> Brands { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<ProductImage> ProductImages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>()
            .Property(p => p.Price).HasPrecision(18, 2);
        builder.Entity<Product>()
            .Property(p => p.OldPrice).HasPrecision(18, 2);
        builder.Entity<Product>()
            .Property(p => p.DiscountedPrice).HasPrecision(18, 2);

        builder.Entity<Sale>()
            .Property(s => s.TotalPrice).HasPrecision(18, 2);
    }
}
