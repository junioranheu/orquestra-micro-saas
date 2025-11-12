using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using Orquestra.API.Controllers;
using Orquestra.API.Filters;
using Orquestra.Application.UseCases.Locations.States.Get;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using System.Security.Claims;

namespace Orquestra.UnitTests.Tests.Controllers;

public sealed class TestControllerTests
{
    [Fact]
    public void GetAnonymous_ShouldReturn_OkWithAscii()
    {
        // Arrange;
        Mock<IGetState> mockGetState = new();

        List<LocationState> expected =
        [
            new() { LocationStateId = 1, Name = "SP" },
            new() { LocationStateId = 2, Name = "RJ" }
        ];

        mockGetState.Setup(x => x.Execute()).ReturnsAsync(expected);

        TestController controller = new(mockGetState.Object);

        // Act;
        ActionResult result = controller.GetAnonymous();

        // Assert;
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        string content = Assert.IsType<string>(okResult.Value);

        Assert.NotNull(content);
    }

    [Theory]
    [InlineData(UserRoleEnum.Administrator, UserRoleEnum.Administrator, true)]
    [InlineData(UserRoleEnum.Administrator, UserRoleEnum.Common, false)]
    public void AuthorizeFilter_ShouldPass_WhenUserHasRequiredRole(UserRoleEnum userRole, UserRoleEnum requiredRole, bool mustWork)
    {
        // Arrange;
        ClaimsIdentity identity = new(
        [
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, requiredRole.ToString()) // A role que o filtro vai exigir;
        ], "TestAuth");

        DefaultHttpContext httpContext = new()
        {
            User = new ClaimsPrincipal(identity)
        };

        ActionContext actionContext = new(httpContext, new RouteData(), new ControllerActionDescriptor());

        AuthorizationFilterContext filterContext = new(actionContext, []);

        AuthorizeFilter filter = new(userRole); // A Role do usuário;

        // Act;
        filter.OnAuthorization(filterContext);

        // Assert;
        if (mustWork)
        {
            // Deve passar;
            Assert.Null(filterContext.Result);
        }
        else
        {
            // Deve bloquear;
            Assert.NotNull(filterContext.Result);

            if (filterContext.Result is ObjectResult objResult)
            {
                Assert.Equal(StatusCodes.Status403Forbidden, objResult.StatusCode);
            }
            else if (filterContext.Result is UnauthorizedResult result)
            {
                Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            }
        }
    }

    [Fact]
    public void AuthorizeFilter_ShouldReturn401_WhenUserNotAuthenticated()
    {
        // Arrange;
        DefaultHttpContext httpContext = new()
        {
            User = new ClaimsPrincipal(new ClaimsIdentity()) // Sem autenticação;
        };

        ActionContext actionContext = new(httpContext, new RouteData(), new ControllerActionDescriptor());

        AuthorizationFilterContext filterContext = new(actionContext, []);
        AuthorizeFilter filter = new(); // Sem roles;

        // Act;
        filter.OnAuthorization(filterContext);

        // Assert;
        Assert.IsType<UnauthorizedResult>(filterContext.Result);
    }
}