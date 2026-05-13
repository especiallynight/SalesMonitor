using MathNet.Numerics.Statistics;
using Microsoft.EntityFrameworkCore;
using SalesMonitor.Data;
using SalesMonitor.Models;

namespace SalesMonitor.Services
{
    public class AnalyticsService
    {
        private readonly AppDbContext _db;

        public AnalyticsService(AppDbContext db) => _db = db;

        public async Task<List<AbcResult>> GetAbcAnalysisAsync(DateTime? from = null, DateTime? to = null)
        {
            var query = _db.Sales.Include(s => s.Product).AsQueryable();

            if (from != null) query = query.Where(s => s.SaleDate >= from.Value);
            if (to != null) query = query.Where(s => s.SaleDate <= to.Value);

            var sales = await query.ToListAsync();

            var productRevenue = sales
                .GroupBy(s => new { s.ProductId, s.Product.Name })
                .Select(g => new
                {
                    g.Key.ProductId,
                    g.Key.Name,
                    Revenue = g.Sum(s => s.TotalAmount)
                })
                .OrderByDescending(x => x.Revenue)
                .ToList();

            var totalRevenue = productRevenue.Sum(x => x.Revenue);
            double cumulative = 0;
            var results = new List<AbcResult>();

            foreach (var item in productRevenue)
            {
                var share = totalRevenue > 0
                    ? (double)(item.Revenue / totalRevenue) * 100
                    : 0;
                cumulative += share;

                string category;
                if (cumulative <= 80) category = "A";
                else if (cumulative <= 95) category = "B";
                else category = "C";

                results.Add(new AbcResult
                {
                    ProductId = item.ProductId,
                    ProductName = item.Name,
                    Revenue = item.Revenue,
                    RevenueShare = Math.Round(share, 1),
                    CumulativeShare = Math.Round(cumulative, 1),
                    Category = category
                });
            }

            return results;
        }

        public async Task<List<XyzResult>> GetXyzAnalysisAsync()
        {
            var sales = await _db.Sales
                .Include(s => s.Product)
                .Select(s => new
                {
                    s.ProductId,
                    s.Product.Name,
                    s.SaleDate.Year,
                    s.SaleDate.Month,
                    s.TotalAmount
                })
                .ToListAsync();

            var results = new List<XyzResult>();

            foreach (var group in sales.GroupBy(s => new { s.ProductId, s.Name }))
            {
                var monthlySales = group
                    .GroupBy(s => new { s.Year, s.Month })
                    .Select(g => (double)g.Sum(s => s.TotalAmount))
                    .ToList();

                if (monthlySales.Count < 2) continue;

                var cv = StatisticsHelper.CoefficientOfVariation(monthlySales);

                string category;
                if (cv < 10) category = "X";
                else if (cv < 25) category = "Y";
                else category = "Z";

                results.Add(new XyzResult
                {
                    ProductId = group.Key.ProductId,
                    ProductName = group.Key.Name,
                    CoefficientOfVariation = Math.Round(cv, 1),
                    Category = category
                });
            }

            return results.OrderBy(r => r.CoefficientOfVariation).ToList();
        }

        public async Task<List<PlanFactResult>> GetPlanFactAsync(DateTime from, DateTime to)
        {
            var products = await _db.Products.ToListAsync();
            var results = new List<PlanFactResult>();

            foreach (var product in products)
            {
                var fact = await _db.Sales
                    .Where(s => s.ProductId == product.Id
                             && s.SaleDate >= from
                             && s.SaleDate <= to)
                    .SumAsync(s => s.TotalAmount);

                var daysInPeriod = (to - from).Days + 1;
                var planForPeriod = product.PlanSales * daysInPeriod / 30m;
                var deviation = fact - planForPeriod;
                var deviationPct = planForPeriod != 0
                    ? (double)(deviation / planForPeriod) * 100
                    : 0;

                results.Add(new PlanFactResult
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Plan = planForPeriod,
                    Fact = fact,
                    Deviation = deviation,
                    DeviationPercent = Math.Round(deviationPct, 1)
                });
            }

            return results;
        }

        public async Task<List<DeviationResult>> GetDeviationsAsync(DateTime from, DateTime to)
        {
            var sales = await _db.Sales
                .Include(s => s.Product)
                .Where(s => s.SaleDate >= from && s.SaleDate <= to)
                .ToListAsync();

            if (sales.Count < 5) return new List<DeviationResult>();

            var amounts = sales.Select(s => (double)s.TotalAmount).ToList();
            var mean = amounts.Mean();
            var stdDev = amounts.StandardDeviation();
            const double threshold = 2.5;

            return sales
                .Select(s => new DeviationResult
                {
                    SaleId = s.Id,
                    ProductName = s.Product.Name,
                    SaleDate = s.SaleDate,
                    TotalAmount = s.TotalAmount,
                    Mean = Math.Round(mean, 2),
                    IsOutlier = Math.Abs((double)s.TotalAmount - mean) > threshold * stdDev
                })
                .OrderByDescending(d => Math.Abs((double)d.TotalAmount - mean))
                .Take(20)
                .ToList();
        }

        public async Task<List<DayData>> GetDailyDataAsync(DateTime from, DateTime to)
        {
            var sales = await _db.Sales
                .Where(s => s.SaleDate >= from && s.SaleDate <= to)
                .ToListAsync();

            return sales
                .GroupBy(s => s.SaleDate.Date)
                .Select(g => new DayData
                {
                    Date = g.Key,
                    Value = g.Sum(s => s.TotalAmount)
                })
                .OrderBy(d => d.Date)
                .ToList();
        }

        public static class StatisticsHelper
        {
            public static double CoefficientOfVariation(IEnumerable<double> values)
            {
                var data = values.ToList();
                var mean = data.Mean();
                return mean != 0
                    ? data.StandardDeviation() / mean * 100
                    : 0;
            }
        }
    }

    public class DayData
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
    }
}