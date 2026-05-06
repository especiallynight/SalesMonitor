namespace SalesMonitor.Models
{
    public class AbcResult
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public double RevenueShare { get; set; }
        public double CumulativeShare { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    public class XyzResult
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public double CoefficientOfVariation { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    public class SalesForecast
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public List<decimal> HistoricalData { get; set; } = new();
        public List<decimal> ForecastValues { get; set; } = new();
        public decimal NextMonthForecast { get; set; }
    }

    public class PlanFactResult
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Plan { get; set; }
        public decimal Fact { get; set; }
        public decimal Deviation { get; set; }
        public double DeviationPercent { get; set; }
    }

    public class DeviationResult
    {
        public int SaleId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public double Mean { get; set; }
        public bool IsOutlier { get; set; }
    }
}