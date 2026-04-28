namespace SalesMonitor.Models
{
    public class AlertNotification
    {
        public string ProductName { get; set; } = string.Empty;
        public string IssueType { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}