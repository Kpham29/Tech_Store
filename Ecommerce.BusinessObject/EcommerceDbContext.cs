using Ecommerce.BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.BusinessObject;

public class EcommerceDbContext : DbContext
{
    public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) :  base(options){}
    
    #region DbSets

    public DbSet<Users> Users { get; set; }
    public DbSet<Roles> Roles { get; set; }

    public DbSet<Products> Products { get; set; }
    public DbSet<Categories> Categories { get; set; }
    public DbSet<Brands> Brands { get; set; }
    public DbSet<CategoryBrands> CategoryBrands { get; set; }

    public DbSet<Attributes> Attributes { get; set; }
    public DbSet<ProductAttributes> ProductAttributes { get; set; }
    public DbSet<ProductImages> ProductImages { get; set; }

    public DbSet<Carts> Carts { get; set; }
    public DbSet<CartItems> CartItems { get; set; }

    public DbSet<Orders> Orders { get; set; }
    public DbSet<OrderDetails> OrderDetails { get; set; }
    public DbSet<Payments> Payments { get; set; }

    public DbSet<Reviews> Reviews { get; set; }
    public DbSet<Requests> Requests { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // CategoryBrand (Many-to-Many)
        modelBuilder.Entity<CategoryBrands>()
            .HasKey(cb => new { cb.CategoryId, cb.BrandId });

        modelBuilder.Entity<CategoryBrands>()
            .HasOne(cb => cb.Category)
            .WithMany(c => c.CategoryBrands)
            .HasForeignKey(cb => cb.CategoryId);

        modelBuilder.Entity<CategoryBrands>()
            .HasOne(cb => cb.Brand)
            .WithMany(b => b.CategoryBrands)
            .HasForeignKey(cb => cb.BrandId);

        // One User - One Cart
        modelBuilder.Entity<Carts>()
            .HasIndex(c => c.UserId)
            .IsUnique();

        // One Order - One Payment
        modelBuilder.Entity<Payments>()
            .HasIndex(p => p.OrderId)
            .IsUnique();

        // Decimal precision
        modelBuilder.Entity<Products>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Orders>()
            .Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderDetails>()
            .Property(od => od.UnitPrice)
            .HasPrecision(18, 2);
        modelBuilder.Entity<Requests>()
            .HasOne(r => r.User)
            .WithMany(u => u.Requests)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}