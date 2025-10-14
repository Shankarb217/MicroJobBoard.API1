using MicroJobBoard.API.DTOs.Application;

namespace MicroJobBoard.API.Services.Interfaces;

public interface IApplicationService
{
    Task<ApplicationDto> ApplyToJobAsync(int jobId, CreateApplicationDto createApplicationDto, int applicantId);
    Task<IEnumerable<ApplicationDto>> GetMyApplicationsAsync(int applicantId);
    Task<IEnumerable<ApplicationDto>> GetApplicationsByJobAsync(int jobId, int employerId);
    Task<ApplicationDto> UpdateApplicationStatusAsync(int applicationId, string status, int employerId);
}
