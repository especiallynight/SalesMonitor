using Microsoft.EntityFrameworkCore;
using SalesMonitor.Data;

namespace SalesMonitor.Services
{
    public class ComparisonService
    {
        private readonly AppDbContext _db;
        public ComparisonService(AppDbContext db) => _db = db;

        public async Task<ComparisonResult> ComparePeriodsAsync(DateTime from1, DateTime to1, DateTime from2, DateTime to2)
        {
            var sales1 = await _db.Sales
                .Where(s => s.SaleDate >= from1 && s.SaleDate <= to1)
                .ToListAsync();

            var sales2 = await _db.Sales
                .Where(s => s.SaleDate >= from2 && s.SaleDate <= to2)
                .ToListAsync();

            var total1 = sales1.Sum(s => s.TotalAmount);
            var total2 = sales2.Sum(s => s.TotalAmount);
            var count1 = sales1.Count;
            var count2 = sales2.Count;
            var avg1 = count1 > 0 ? total1 / count1 : 0;
            var avg2 = count2 > 0 ? total2 / count2 : 0;

            return new ComparisonResult
            {
                Period1Label = $"{from1:dd.MM.yyyy} - {to1:dd.MM.yyyy}",
                Period2Label = $"{from2:dd.MM.yyyy} - {to2:dd.MM.yyyy}",
                TotalRevenue1 = total1,
                TotalRevenue2 = total2,
                RevenueChange = total1 != 0 ? (double)((total2 - total1) / total1) * 100 : 0,
                SalesCount1 = count1,
                SalesCount2 = count2,
                AvgCheck1 = avg1,
                AvgCheck2 = avg2,
                AvgCheckChange = avg1 != 0 ? (double)((avg2 - avg1) / avg1) * 100 : 0
            };
        }

        public async Task<List<PeriodDayData>> GetDailyComparisonAsync(DateTime from1, DateTime to1, DateTime from2, DateTime to2)
        {
            var days1 = Math.Max(1, (to1 - from1).Days);
            var days2 = Math.Max(1, (to2 - from2).Days);

            var result = new List<PeriodDayData>();

            for (int i = 0; i < Math.Max(days1, days2); i++)
            {
                var date1 = from1.AddDays(i);
                var date2 = from2.AddDays(i);

                result.Add(new PeriodDayData
                {
                    Day = i + 1,
                    Period1Value = await _db.Sales.Where(s => s.SaleDate == date1).SumAsync(s => s.TotalAmount),
                    Period2Value = await _db.Sales.Where(s => s.SaleDate == date2).SumAsync(s => s.TotalAmount)
                });
            }

            return result;
        }
    }

    public class ComparisonResult
    {
        public string Period1Label { get; set; } = string.Empty;
        public string Period2Label { get; set; } = string.Empty;
        public decimal TotalRevenue1 { get; set; }
        public decimal TotalRevenue2 { get; set; }
        public double RevenueChange { get; set; }
        public int SalesCount1 { get; set; }
        public int SalesCount2 { get; set; }
        public decimal AvgCheck1 { get; set; }
        public decimal AvgCheck2 { get; set; }
        public double AvgCheckChange { get; set; }
    }

    public class PeriodDayData
    {
        public int Day { get; set; }
        public decimal Period1Value { get; set; }
        public decimal Period2Value { get; set; }
    }
}