using Microsoft.EntityFrameworkCore;
using SalesMonitor.Models;

namespace SalesMonitor.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Sale> Sales => Set<Sale>();
        public DbSet<ClientActivity> ClientActivities => Set<ClientActivity>();
    }
}