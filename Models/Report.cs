namespace MicroJobBoard.API.Models;

public class Report
{
    public int Id { get; set; }
    public string ReportType { get; set; } = string.Empty; // JobStatistics, UserActivity, ApplicationMetrics
    public string Data { get; set; } = string.Empty; // JSON data
    public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
}
