using AutoMapper;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;

namespace Orquestra.Application.AutoMapper;

public sealed class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<User, UserOutput>();
    }
}