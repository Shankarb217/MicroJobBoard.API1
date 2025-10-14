namespace MicroJobBoard.API.DTOs.Job;

public class JobDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Salary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime PostedDate { get; set; }
    public int EmployerId { get; set; }
    public string EmployerName { get; set; } = string.Empty;
    public int ApplicationsCount { get; set; }
}
