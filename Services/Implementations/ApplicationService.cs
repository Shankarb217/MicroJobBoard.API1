using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MicroJobBoard.API.Data;
using MicroJobBoard.API.DTOs.Application;
using MicroJobBoard.API.Services.Interfaces;

namespace MicroJobBoard.API.Services.Implementations;

public class ApplicationService : IApplicationService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ApplicationService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApplicationDto> ApplyToJobAsync(int jobId, CreateApplicationDto createApplicationDto, int applicantId)
    {
        // Check if job exists and is approved
        var job = await _context.Jobs.FindAsync(jobId);
        if (job == null)
        {
            throw new KeyNotFoundException("Job not found");
        }

        if (job.Status != "Approved")
        {
            throw new InvalidOperationException("Cannot apply to this job");
        }

        // Check if already applied
        var existingApplication = await _context.Applications
            .FirstOrDefaultAsync(a => a.JobId == jobId && a.ApplicantId == applicantId);

        if (existingApplication != null)
        {
            throw new InvalidOperationException("You have already applied to this job");
        }

        // Create application
        var application = new Models.Application
        {
            JobId = jobId,
            ApplicantId = applicantId,
            CoverLetter = createApplicationDto.CoverLetter,
            Status = "Pending",
            AppliedDate = DateTime.UtcNow
        };

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();

        // Reload with includes
        await _context.Entry(application).Reference(a => a.Job).LoadAsync();
        await _context.Entry(application).Reference(a => a.Applicant).LoadAsync();

        return _mapper.Map<ApplicationDto>(application);
    }

    public async Task<IEnumerable<ApplicationDto>> GetMyApplicationsAsync(int applicantId)
    {
        var applications = await _context.Applications
            .Include(a => a.Job)
            .Include(a => a.Applicant)
            .Where(a => a.ApplicantId == applicantId)
            .OrderByDescending(a => a.AppliedDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ApplicationDto>>(applications);
    }

    public async Task<IEnumerable<ApplicationDto>> GetApplicationsByJobAsync(int jobId, int employerId)
    {
        // Verify job belongs to employer
        var job = await _context.Jobs.FindAsync(jobId);
        if (job == null)
        {
            throw new KeyNotFoundException("Job not found");
        }

        if (job.EmployerId != employerId)
        {
            throw new UnauthorizedAccessException("You are not authorized to view these applications");
        }

        var applications = await _context.Applications
            .Include(a => a.Job)
            .Include(a => a.Applicant)
            .Where(a => a.JobId == jobId)
            .OrderByDescending(a => a.AppliedDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ApplicationDto>>(applications);
    }

    public async Task<ApplicationDto> UpdateApplicationStatusAsync(int applicationId, string status, int employerId)
    {
        var application = await _context.Applications
            .Include(a => a.Job)
            .Include(a => a.Applicant)
            .FirstOrDefaultAsync(a => a.Id == applicationId);

        if (application == null)
        {
            throw new KeyNotFoundException("Application not found");
        }

        // Verify job belongs to employer
        if (application.Job.EmployerId != employerId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this application");
        }

        // Validate status
        if (status != "Accepted" && status != "Rejected")
        {
            throw new InvalidOperationException("Invalid status. Must be 'Accepted' or 'Rejected'");
        }

        application.Status = status;
        application.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<ApplicationDto>(application);
    }
}
