using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using Microsoft.EntityFrameworkCore;
using SalesMonitor.Data;
using SalesMonitor.Models;

namespace SalesMonitor.Services
{
    public class ForecastService
    {
        private readonly AppDbContext _db;
        public ForecastService(AppDbContext db) => _db = db;

        public async Task<List<SalesForecast>> GetAllForecastsAsync(int horizon = 30)
        {
            var products = await _db.Products.ToListAsync();
            var results = new List<SalesForecast>();

            foreach (var product in products)
            {
                var forecast = await GetForecastAsync(product.Id, horizon);
                if (forecast != null) results.Add(forecast);
            }

            return results;
        }

        public async Task<SalesForecast?> GetForecastAsync(int productId, int horizon = 30)
        {
            var sales = await _db.Sales
                .Where(s => s.ProductId == productId)
                .OrderBy(s => s.SaleDate)
                .ToListAsync();

            if (sales.Count < 10) return null;

            var product = await _db.Products.FindAsync(productId);
            if (product == null) return null;

            var dailySales = sales
                .GroupBy(s => s.SaleDate)
                .Select(g => (float)g.Sum(s => (double)s.TotalAmount))
                .ToList();

            if (dailySales.Count < 10) return null;

            try
            {
                var mlContext = new MLContext();

                var data = dailySales.Select((value, index) => new TimeSeriesData
                {
                    Date = DateTime.Today.AddDays(-dailySales.Count + index + 1),
                    Value = value
                }).ToList();

                var dataView = mlContext.Data.LoadFromEnumerable(data);

                var pipeline = mlContext.Forecasting.ForecastBySsa(
                    outputColumnName: "Forecast",
                    inputColumnName: "Value",
                    windowSize: Math.Min(7, dailySales.Count / 2),
                    seriesLength: dailySales.Count,
                    trainSize: dailySales.Count,
                    horizon: horizon,
                    confidenceLevel: 0.95f
                );

                var model = pipeline.Fit(dataView);
                var forecastEngine = model.CreateTimeSeriesEngine<TimeSeriesData, ForecastResult>(mlContext);
                var forecast = forecastEngine.Predict();

                return new SalesForecast
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    HistoricalData = dailySales.Select(f => (decimal)f).ToList(),
                    ForecastValues = forecast.Forecast?.Select(f => (decimal)f).ToList() ?? new(),
                    NextMonthForecast = forecast.Forecast?.Sum(f => (decimal)f) ?? 0
                };
            }
            catch
            {
                return null;
            }
        }
    }

    public class TimeSeriesData
    {
        public DateTime Date { get; set; }
        public float Value { get; set; }
    }

    public class ForecastResult
    {
        public float[]? Forecast { get; set; }
    }
}