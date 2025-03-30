using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.AutoMapper;
using Orquestra.Infrastructure.Data;

namespace Orquestra.UnitTests.Fixtures;

public static class Fixture
{
    public static Context CreateContext()
    {
        DbContextOptions<Context> mockContext = new DbContextOptionsBuilder<Context>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        HttpContextAccessor mockHttpContextAccessor = new();

        Context? context = new(mockContext, mockHttpContextAccessor);

        return context;
    }

    public static IMapper CreateMapper()
    {
        MapperConfiguration mockMapper = new(x =>
        {
            x.AddProfile(new AutoMapperConfig());
        });

        IMapper map = mockMapper.CreateMapper();

        return map;
    }
}