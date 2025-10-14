using MicroJobBoard.API.DTOs.Job;

namespace MicroJobBoard.API.Services.Interfaces;

public interface IJobService
{
    Task<IEnumerable<JobDto>> GetAllJobsAsync(string? keyword, string? category, string? location);
    Task<JobDto> GetJobByIdAsync(int id);
    Task<JobDto> CreateJobAsync(CreateJobDto createJobDto, int employerId);
    Task<JobDto> UpdateJobAsync(int id, CreateJobDto updateJobDto, int employerId);
    Task DeleteJobAsync(int id, int employerId);
    Task<IEnumerable<JobDto>> GetMyJobsAsync(int employerId);
    Task<IEnumerable<JobDto>> GetPendingJobsAsync();
    Task ApproveJobAsync(int id);
}
