using AutoMapper;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Application.UseCases.Schedules.Shared;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;

namespace Orquestra.Application.AutoMapper;

public sealed class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<UserInput, User>();
        CreateMap<User, UserOutput>();

        CreateMap<CompanyInput, Company>();
        CreateMap<Company, CompanyOutput>();

        CreateMap<CompanyUser, CompanyUserOutput>();

        CreateMap<ScheduleInput, Schedule>();
        CreateMap<Schedule, ScheduleOutput>(); 
    }
}