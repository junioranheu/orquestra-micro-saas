using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.AutoMapper;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using System.Security.Claims;

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

    public static async Task Save<T>(Context context, T obj) where T : class
    {
        DbSet<T> dbSet = context.Set<T>();

        await dbSet.AddAsync(obj);
        await context.SaveChangesAsync();
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

    public static void Auth(ControllerBase controller)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, "Mock"),
            new(ClaimTypes.Email, "Mock@test.com") ,
            new(ClaimTypes.Role, UserRoleEnum.Common.ToString())
        ];

        var identity = new ClaimsIdentity(claims, "TestAuthScheme");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }
}