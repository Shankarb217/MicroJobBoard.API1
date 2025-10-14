using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MicroJobBoard.API.DTOs.Application;
using MicroJobBoard.API.Services.Interfaces;

namespace MicroJobBoard.API.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationsController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [Authorize(Roles = "Seeker")]
    [HttpPost("jobs/{jobId}/apply")]
    public async Task<ActionResult<ApplicationDto>> ApplyToJob(int jobId, [FromBody] CreateApplicationDto createApplicationDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var application = await _applicationService.ApplyToJobAsync(jobId, createApplicationDto, userId);
            return CreatedAtAction(nameof(GetMyApplications), new { }, application);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Seeker")]
    [HttpGet("my-applications")]
    public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetMyApplications()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var applications = await _applicationService.GetMyApplicationsAsync(userId);
        return Ok(applications);
    }

    [Authorize(Roles = "Employer,Admin")]
    [HttpGet("jobs/{jobId}/applications")]
    public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetApplicationsByJob(int jobId)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var applications = await _applicationService.GetApplicationsByJobAsync(jobId, userId);
            return Ok(applications);
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
    [HttpPut("applications/{applicationId}/status")]
    public async Task<ActionResult<ApplicationDto>> UpdateApplicationStatus(
        int applicationId,
        [FromBody] UpdateApplicationStatusDto updateStatusDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var application = await _applicationService.UpdateApplicationStatusAsync(
                applicationId,
                updateStatusDto.Status,
                userId);
            return Ok(application);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class UpdateApplicationStatusDto
{
    public string Status { get; set; } = string.Empty;
}
