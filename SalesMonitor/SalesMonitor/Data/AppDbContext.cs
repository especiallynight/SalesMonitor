using Microsoft.EntityFrameworkCore;
using SalesMonitor.Models;

namespace SalesMonitor.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<ClientActivity> ClientActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Product)
                .WithMany(p => p.Sales)
                .HasForeignKey(s => s.ProductId);

            modelBuilder.Entity<ClientActivity>()
                .HasOne(ca => ca.Product)
                .WithOne(p => p.ClientActivity)
                .HasForeignKey<ClientActivity>(ca => ca.ProductId);
        }
    }
}