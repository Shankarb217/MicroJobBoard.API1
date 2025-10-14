using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MicroJobBoard.API.DTOs.Job;
using MicroJobBoard.API.Services.Interfaces;

namespace MicroJobBoard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Employer,Admin")]
public class MyJobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public MyJobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobDto>>> GetMyJobs()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var jobs = await _jobService.GetMyJobsAsync(userId);
        return Ok(jobs);
    }
}
