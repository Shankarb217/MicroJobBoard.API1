using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MicroJobBoard.API.Data;
using MicroJobBoard.API.DTOs.Job;
using MicroJobBoard.API.Models;
using MicroJobBoard.API.Services.Interfaces;

namespace MicroJobBoard.API.Services.Implementations;

public class JobService : IJobService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public JobService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<JobDto>> GetAllJobsAsync(string? keyword, string? category, string? location)
    {
        var query = _context.Jobs
            .Include(j => j.Employer)
            .Include(j => j.Applications)
            .Where(j => j.Status == "Approved")
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(j => j.Title.Contains(keyword) || j.Description.Contains(keyword));
        }

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(j => j.Category == category);
        }

        if (!string.IsNullOrEmpty(location))
        {
            query = query.Where(j => j.Location.Contains(location));
        }

        var jobs = await query.OrderByDescending(j => j.PostedDate).ToListAsync();
        return _mapper.Map<IEnumerable<JobDto>>(jobs);
    }

    public async Task<JobDto> GetJobByIdAsync(int id)
    {
        var job = await _context.Jobs
            .Include(j => j.Employer)
            .Include(j => j.Applications)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
        {
            throw new KeyNotFoundException("Job not found");
        }

        return _mapper.Map<JobDto>(job);
    }

    public async Task<JobDto> CreateJobAsync(CreateJobDto createJobDto, int employerId)
    {
        var job = _mapper.Map<Job>(createJobDto);
        job.EmployerId = employerId;
        job.Status = "Pending"; // Requires admin approval
        job.PostedDate = DateTime.UtcNow;

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        // Reload with includes
        await _context.Entry(job).Reference(j => j.Employer).LoadAsync();
        await _context.Entry(job).Collection(j => j.Applications).LoadAsync();

        return _mapper.Map<JobDto>(job);
    }

    public async Task<JobDto> UpdateJobAsync(int id, CreateJobDto updateJobDto, int employerId)
    {
        var job = await _context.Jobs.FindAsync(id);

        if (job == null)
        {
            throw new KeyNotFoundException("Job not found");
        }

        if (job.EmployerId != employerId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this job");
        }

        job.Title = updateJobDto.Title;
        job.Company = updateJobDto.Company;
        job.Location = updateJobDto.Location;
        job.JobType = updateJobDto.JobType;
        job.Category = updateJobDto.Category;
        job.Salary = updateJobDto.Salary;
        job.Description = updateJobDto.Description;
        job.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Reload with includes
        await _context.Entry(job).Reference(j => j.Employer).LoadAsync();
        await _context.Entry(job).Collection(j => j.Applications).LoadAsync();

        return _mapper.Map<JobDto>(job);
    }

    public async Task DeleteJobAsync(int id, int employerId)
    {
        var job = await _context.Jobs.FindAsync(id);

        if (job == null)
        {
            throw new KeyNotFoundException("Job not found");
        }

        if (job.EmployerId != employerId)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this job");
        }

        _context.Jobs.Remove(job);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<JobDto>> GetMyJobsAsync(int employerId)
    {
        var jobs = await _context.Jobs
            .Include(j => j.Employer)
            .Include(j => j.Applications)
            .Where(j => j.EmployerId == employerId)
            .OrderByDescending(j => j.PostedDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<JobDto>>(jobs);
    }

    public async Task<IEnumerable<JobDto>> GetPendingJobsAsync()
    {
        var jobs = await _context.Jobs
            .Include(j => j.Employer)
            .Include(j => j.Applications)
            .Where(j => j.Status == "Pending")
            .OrderByDescending(j => j.PostedDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<JobDto>>(jobs);
    }

    public async Task ApproveJobAsync(int id)
    {
        var job = await _context.Jobs.FindAsync(id);

        if (job == null)
        {
            throw new KeyNotFoundException("Job not found");
        }

        job.Status = "Approved";
        job.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}
