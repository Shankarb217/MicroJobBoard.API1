using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroJobBoard.API.Data;
using MicroJobBoard.API.DTOs.Job;
using MicroJobBoard.API.Services.Interfaces;

namespace MicroJobBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IJobService _jobService;

    public AdminController(AppDbContext context, IJobService jobService)
    {
        _context = context;
        _jobService = jobService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.Role,
                JoinedDate = u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpPut("users/{userId}/role")]
    public async Task<IActionResult> UpdateUserRole(int userId, [FromBody] UpdateRoleDto updateRoleDto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        if (updateRoleDto.Role != "Seeker" && updateRoleDto.Role != "Employer" && updateRoleDto.Role != "Admin")
        {
            return BadRequest(new { message = "Invalid role" });
        }

        user.Role = updateRoleDto.Role;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = "User role updated successfully", user });
    }

    [HttpGet("pending-jobs")]
    public async Task<ActionResult<IEnumerable<JobDto>>> GetPendingJobs()
    {
        var jobs = await _jobService.GetPendingJobsAsync();
        return Ok(jobs);
    }

    [HttpPut("jobs/{jobId}/approve")]
    public async Task<IActionResult> ApproveJob(int jobId)
    {
        try
        {
            await _jobService.ApproveJobAsync(jobId);
            return Ok(new { message = "Job approved successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("dashboard/stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var stats = new
        {
            TotalUsers = await _context.Users.CountAsync(),
            TotalJobs = await _context.Jobs.CountAsync(),
            TotalApplications = await _context.Applications.CountAsync(),
            ActiveJobs = await _context.Jobs.CountAsync(j => j.Status == "Approved")
        };

        return Ok(stats);
    }

    [HttpGet("reports")]
    public async Task<IActionResult> GetReports()
    {
        var reports = await _context.Reports
            .OrderByDescending(r => r.GeneratedDate)
            .ToListAsync();

        return Ok(reports);
    }
}

public class UpdateRoleDto
{
    public string Role { get; set; } = string.Empty;
}
