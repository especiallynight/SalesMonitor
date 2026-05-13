using Microsoft.AspNetCore.Mvc;
using SalesMonitor.Services;

namespace SalesMonitor.Controllers
{
    [Route("api/export")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly ExportService _export;
        public ExportController(ExportService export) => _export = export;

        [HttpGet("sales")]
        public async Task<IActionResult> ExportSales(DateTime from, DateTime to)
        {
            var bytes = await _export.ExportSalesAsync(from, to);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Продажи_{from:dd.MM.yyyy}-{to:dd.MM.yyyy}.xlsx");
        }

        [HttpGet("abc")]
        public async Task<IActionResult> ExportAbc(DateTime from, DateTime to)
        {
            var bytes = await _export.ExportAbcAsync(from, to);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"ABC_{from:dd.MM.yyyy}-{to:dd.MM.yyyy}.xlsx");
        }
    }
}