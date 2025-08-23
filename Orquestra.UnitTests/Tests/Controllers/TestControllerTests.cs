using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Orquestra.API.Controllers;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Services.Email;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.UnitTests.Tests.Controllers;

public sealed class TestControllerTests
{
    [Fact]
    public void GetAnonymous_ShouldReturn_OkWithAscii()
    {
        // Arrange;
        TestController controller = new();

        // Act;
        ActionResult result = controller.GetAnonymous();

        // Assert;
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        string content = Assert.IsType<string>(okResult.Value);
        string day = GetDate().Day.ToString();

        Assert.Contains(day, content);
    }

    [Fact]
    public void GetAuth_ShouldReturn_OkWithUserId()
    {
        // Arrange;
        Guid userId = Guid.NewGuid();

        // Mock do HttpContext.User;
        ClaimsPrincipal claims = new(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        ], "mock"));

        TestController controller = new()
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claims } // Usuário logado;
            }
        };

        // Act;
        ActionResult result = controller.GetAuth();

        // Assert;
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        object? content = okResult.Value;

        // Reflection
        PropertyInfo? idProp = content?.GetType().GetProperty("Id");
        object? idValue = idProp?.GetValue(content);
        Assert.Equal(userId, (Guid)idValue!);
    }

    [Fact]
    public void GetAuth_ShouldThrow_WhenUserNotAuthenticated()
    {
        // Arrange;
        TestController controller = new()
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // Sem usuário;
            }
        };

        // Act & Assert;
        Exception ex = Assert.Throws<Exception>(() => controller.GetAuth());
        Assert.Equal(SystemConsts.Warn_Simple_UserNotAuth, ex.Message);
    }
}