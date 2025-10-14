namespace MicroJobBoard.API.Models;

public class Application
{
    public int Id { get; set; }
    public string CoverLetter { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Accepted, Rejected
    public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Foreign keys
    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public int ApplicantId { get; set; }
    public User Applicant { get; set; } = null!;
}
