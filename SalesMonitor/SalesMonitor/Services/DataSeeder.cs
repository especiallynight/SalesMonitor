using Microsoft.EntityFrameworkCore;
using SalesMonitor.Data;
using SalesMonitor.Models;

namespace SalesMonitor.Services
{
    public class DataSeeder
    {
        private readonly AppDbContext _db;

        public DataSeeder(AppDbContext db) => _db = db;

        public async Task SeedAsync()
        {
            if (await _db.Products.AnyAsync()) return;

            var products = new List<Product>
            {
                new() { Name = "Кофе в зёрнах", CostPrice = 350, SellingPrice = 500, IsOnSale = true, DateAdded = DateTime.Today.AddMonths(-6) },
                new() { Name = "Молоко 3,2%", CostPrice = 55, SellingPrice = 80, IsOnSale = true, DateAdded = DateTime.Today.AddMonths(-4) },
                new() { Name = "Хлеб цельнозерновой", CostPrice = 30, SellingPrice = 50, IsOnSale = false, DateAdded = DateTime.Today.AddMonths(-3) }
            };

            _db.Products.AddRange(products);
            await _db.SaveChangesAsync();

            var random = new Random();
            foreach (var product in products)
            {
                var daysBack = product.IsOnSale ? 10 : 45;
                var sales = new List<Sale>();

                for (int i = 0; i < 15; i++)
                {
                    sales.Add(new Sale
                    {
                        ProductId = product.Id,
                        SaleDate = DateTime.Today.AddDays(-random.Next(1, daysBack)),
                        Quantity = random.Next(1, 5),
                        TotalAmount = product.SellingPrice * random.Next(1, 5),
                        Margin = product.SellingPrice - product.CostPrice
                    });
                }

                _db.Sales.AddRange(sales);
                await _db.SaveChangesAsync();

                var lastSale = sales.Max(s => s.SaleDate);
                var intervals = sales.Select(s => s.SaleDate).OrderBy(d => d).ToList();
                double avgInterval = 1;

                if (intervals.Count > 1)
                    avgInterval = intervals.Zip(intervals.Skip(1), (a, b) => (b - a).TotalDays).Average();

                _db.ClientActivities.Add(new ClientActivity
                {
                    ProductId = product.Id,
                    LastSaleDate = lastSale,
                    AverageOrderInterval = (decimal)avgInterval
                });
            }

            await _db.SaveChangesAsync();
        }
    }
}