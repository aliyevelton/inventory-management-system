using Inventory_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection.Emit;

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
    public DbSet<UserRole> CustomUserRoles { get; set; }
    public DbSet<Sale> Sales { get; set; } = null!;
    public DbSet<SaleItem> SaleItems { get; set; } = null!;
    public DbSet<Purchase> Purchases { get; set; } = null!;
    public DbSet<PurchaseItem> PurchaseItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>()
            .Property(p => p.Price).HasPrecision(18, 2);
        builder.Entity<Product>()
            .Property(p => p.DiscountedPrice).HasPrecision(18, 2);

        builder.Entity<Sale>()
            .Property(s => s.TotalPrice).HasPrecision(18, 2);
        builder.Entity<Sale>()
            .Property(s => s.DiscountAmount).HasPrecision(18, 2);

        builder.Entity<SaleItem>()
            .Property(si => si.UnitPrice).HasPrecision(18, 2);

        builder.Entity<Purchase>()
            .Property(p => p.TotalCost).HasPrecision(18, 2);
        builder.Entity<PurchaseItem>()
            .Property(pi => pi.UnitCost).HasPrecision(18, 2);

        builder.Entity<Product>()
        .HasIndex(p => p.SKU)
        .IsUnique();



        builder.Entity<UserRole>().HasData(
        new UserRole { Id = 1, Name = "Store Manager" },
        new UserRole { Id = 2, Name = "Salesperson" },
        new UserRole { Id = 3, Name = "Cashier" },
        new UserRole { Id = 4, Name = "Inventory Clerk" },
        new UserRole { Id = 6, Name = "Purchasing Officer" },
        new UserRole { Id = 7, Name = "Admin" },
        new UserRole { Id = 8, Name = "Accountant" }
        );

        builder.Entity<AppUser>()
        .HasOne(u => u.UserRole)
        .WithMany(r => r.Users)
        .HasForeignKey(u => u.UserRoleId)
        .OnDelete(DeleteBehavior.SetNull);
    }
}
