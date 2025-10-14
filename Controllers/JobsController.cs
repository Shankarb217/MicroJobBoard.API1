using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MicroJobBoard.API.DTOs.Job;
using MicroJobBoard.API.Services.Interfaces;

namespace MicroJobBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobDto>>> GetAllJobs(
        [FromQuery] string? keyword,
        [FromQuery] string? category,
        [FromQuery] string? location)
    {
        var jobs = await _jobService.GetAllJobsAsync(keyword, category, location);
        return Ok(jobs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobDto>> GetJobById(int id)
    {
        try
        {
            var job = await _jobService.GetJobByIdAsync(id);
            return Ok(job);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Employer,Admin")]
    [HttpPost]
    public async Task<ActionResult<JobDto>> CreateJob([FromBody] CreateJobDto createJobDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var job = await _jobService.CreateJobAsync(createJobDto, userId);
            return CreatedAtAction(nameof(GetJobById), new { id = job.Id }, job);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Employer,Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<JobDto>> UpdateJob(int id, [FromBody] CreateJobDto updateJobDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var job = await _jobService.UpdateJobAsync(id, updateJobDto, userId);
            return Ok(job);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [Authorize(Roles = "Employer,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJob(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _jobService.DeleteJobAsync(id, userId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
