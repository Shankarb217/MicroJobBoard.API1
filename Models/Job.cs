namespace MicroJobBoard.API.Models;

public class Job
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty; // Full-time, Part-time, Contract, Internship
    public string Category { get; set; } = string.Empty;
    public string Salary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    public DateTime PostedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Foreign keys
    public int EmployerId { get; set; }
    public User Employer { get; set; } = null!;

    // Navigation properties
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
