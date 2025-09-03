using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Auth.Models;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Email;
using System.Security.Claims;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Fixtures;

public static class Fixture
{
    public static Context CreateContext()
    {
        DbContextOptions<Context> mockContext = new DbContextOptionsBuilder<Context>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        HttpContextAccessor mockHttpContextAccessor = new();

        Context? context = new(mockContext, mockHttpContextAccessor);

        return context;
    }

    public static IHttpContextAccessor CreateIHttpContextAccessor(User user)
    {
        DefaultHttpContext context = new();

        List<Claim> claimList =
        [
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        ];

        ClaimsIdentity identity = new(claimList, "mock");
        context.User = new ClaimsPrincipal(identity);

        Mock<IHttpContextAccessor> mockAccessor = new();
        mockAccessor.Setup(x => x.HttpContext).Returns(context);

        return mockAccessor.Object;
    }

    public static async Task Save<T>(Context context, T obj) where T : class
    {
        DbSet<T> dbSet = context.Set<T>();

        await dbSet.AddAsync(obj);
        await context.SaveChangesAsync();
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

        ClaimsIdentity identity = new(claims, "TestAuthScheme");
        ClaimsPrincipal principal = new(identity);

        DefaultHttpContext httpContext = new()
        {
            User = principal
        };

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    public static IWebHostEnvironment CreateDevelopmentEnvironment()
    {
        Mock<IWebHostEnvironment> envMock = new();
        envMock.SetupGet(x => x.EnvironmentName).Returns("Development");
        envMock.SetupGet(x => x.ApplicationName).Returns(SystemConsts.NameApp);
        envMock.SetupGet(x => x.ContentRootPath).Returns(AppContext.BaseDirectory);
        envMock.SetupGet(x => x.WebRootPath).Returns(AppContext.BaseDirectory);

        return envMock.Object;
    }

    public static IConfiguration CreateConfiguration()
    {
        Dictionary<string, string> urls = new()
        {
            { "Urls:Development:Backend", "http://localhost:5035/api" },
            { "Urls:Development:Frontend", "http://localhost:5173" },
            { "JwtSettings:Secret", GenerateJwtSecret() }
        };

        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(urls!).Build();

        return configuration;
    }

    public static Mock<IEmailService> CreateEmailService()
    {
        Mock<IEmailService> emailServiceMock = new();

        emailServiceMock.Setup(x => x.RenderTemplate(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).Returns("<html>fake</html>");
        emailServiceMock.Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), true, null)).Returns(Task.CompletedTask);

        return emailServiceMock;
    }

    public static Mock<IEmailService> CreateEmailService(Action<Dictionary<string, string>>? capture = null)
    {
        Mock<IEmailService> emailServiceMock = new();

        emailServiceMock.Setup(x => x.RenderTemplate(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).
            Callback<string, Dictionary<string, string>>((tpl, vals) =>
            {
                capture?.Invoke(vals);
            }).Returns("<html>fake</html>");

        emailServiceMock.Setup(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), true, null)).Returns(Task.CompletedTask);

        return emailServiceMock;
    }

    public static IOptions<JwtSettings> CreateJwtOptions()
    {
        JwtSettings jwtSettings = new()
        {
            TokenExpiryMinutes = 10,
            RefreshTokenExpiryMinutes = 30,
            Issuer = $"{SystemConsts.NameApp}.Test",
            Audience = $"{SystemConsts.NameApp}.TestAudience",
        };

        IOptions<JwtSettings> jwtOptions = Options.Create(jwtSettings);

        return jwtOptions;
    }
}