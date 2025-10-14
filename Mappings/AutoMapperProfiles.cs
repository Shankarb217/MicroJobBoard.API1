using AutoMapper;
using MicroJobBoard.API.Models;
using MicroJobBoard.API.DTOs.Auth;
using MicroJobBoard.API.DTOs.Job;
using MicroJobBoard.API.DTOs.Application;

namespace MicroJobBoard.API.Mappings;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.JoinedDate, opt => opt.MapFrom(src => src.CreatedAt));

        // Job mappings
        CreateMap<CreateJobDto, Job>();
        CreateMap<Job, JobDto>()
            .ForMember(dest => dest.EmployerName, opt => opt.MapFrom(src => src.Employer.FullName))
            .ForMember(dest => dest.ApplicationsCount, opt => opt.MapFrom(src => src.Applications.Count));

        // Application mappings
        CreateMap<CreateApplicationDto, Application>();
        CreateMap<Application, ApplicationDto>()
            .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title))
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Job.Company))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Job.Location))
            .ForMember(dest => dest.ApplicantName, opt => opt.MapFrom(src => src.Applicant.FullName))
            .ForMember(dest => dest.ApplicantEmail, opt => opt.MapFrom(src => src.Applicant.Email));
    }
}
