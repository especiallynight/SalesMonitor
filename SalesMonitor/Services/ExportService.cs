using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using SalesMonitor.Data;
using SalesMonitor.Models;

namespace SalesMonitor.Services
{
    public class ExportService
    {
        private readonly AppDbContext _db;
        private readonly AnalyticsService _analyticsService;

        public ExportService(AppDbContext db, AnalyticsService analyticsService)
        {
            _db = db;
            _analyticsService = analyticsService;
        }

        public async Task<byte[]> ExportSalesAsync(DateTime from, DateTime to)
        {
            var sales = await _db.Sales
                .Include(s => s.Product)
                .Where(s => s.SaleDate >= from && s.SaleDate <= to)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Продажи");

            ws.Cell(1, 1).Value = "Дата";
            ws.Cell(1, 2).Value = "Продукт";
            ws.Cell(1, 3).Value = "Количество";
            ws.Cell(1, 4).Value = "Сумма чека";
            ws.Cell(1, 5).Value = "Маржа";

            var headerRow = ws.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#2d3436");
            headerRow.Style.Font.FontColor = XLColor.White;

            for (int i = 0; i < sales.Count; i++)
            {
                var row = i + 2;
                ws.Cell(row, 1).Value = sales[i].SaleDate.ToString("dd.MM.yyyy");
                ws.Cell(row, 2).Value = sales[i].Product.Name;
                ws.Cell(row, 3).Value = sales[i].Quantity;
                ws.Cell(row, 4).Value = (double)sales[i].TotalAmount;
                ws.Cell(row, 5).Value = (double)sales[i].Margin;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> ExportAbcAsync(DateTime from, DateTime to)
        {
            var data = await _analyticsService.GetAbcAnalysisAsync(from, to);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("ABC-анализ");

            ws.Cell(1, 1).Value = "Продукт";
            ws.Cell(1, 2).Value = "Выручка";
            ws.Cell(1, 3).Value = "Доля %";
            ws.Cell(1, 4).Value = "Накоп. %";
            ws.Cell(1, 5).Value = "Категория";

            var headerRow = ws.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#2d3436");
            headerRow.Style.Font.FontColor = XLColor.White;

            for (int i = 0; i < data.Count; i++)
            {
                var row = i + 2;
                ws.Cell(row, 1).Value = data[i].ProductName;
                ws.Cell(row, 2).Value = (double)data[i].Revenue;
                ws.Cell(row, 3).Value = data[i].RevenueShare;
                ws.Cell(row, 4).Value = data[i].CumulativeShare;
                ws.Cell(row, 5).Value = data[i].Category;

                var fillColor = data[i].Category switch
                {
                    "A" => XLColor.FromHtml("#d4edda"),
                    "B" => XLColor.FromHtml("#fff3cd"),
                    _ => XLColor.FromHtml("#f8d7da")
                };
                ws.Row(row).Style.Fill.BackgroundColor = fillColor;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}