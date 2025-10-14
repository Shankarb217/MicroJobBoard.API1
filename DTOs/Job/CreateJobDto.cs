namespace MicroJobBoard.API.DTOs.Job;

public class CreateJobDto
{
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Salary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
