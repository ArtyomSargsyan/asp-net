using Microsoft.EntityFrameworkCore;
using ToDoApi.Domain;
using ToDoApi.Models;

namespace ToDoApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<User> Users { get; set; }
        public DbSet<Currency> Currencies { get; set; } = null!;
        public DbSet<ProductModel> ProductModels { get; set; } = null!;
        public DbSet<Article> Articles { get; set; } = null!;
        public DbSet<Contact> Contacts { get; set; } = null!;

        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Coupon> Coupons { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(r => r.Token)
                .IsUnique();

            // ProductModel → Product relation
            modelBuilder.Entity<ProductModel>()
                .HasOne(pm => pm.Product)
                .WithMany(p => p.ProductModels)
                .HasForeignKey(pm => pm.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
