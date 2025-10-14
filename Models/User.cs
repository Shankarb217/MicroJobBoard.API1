namespace MicroJobBoard.API.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Seeker"; // Seeker, Employer, Admin
    public string? Phone { get; set; }
    public string? Location { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Job> PostedJobs { get; set; } = new List<Job>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
