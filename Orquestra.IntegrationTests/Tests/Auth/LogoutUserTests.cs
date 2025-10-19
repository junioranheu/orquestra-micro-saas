using Microsoft.AspNetCore.Http;
using Moq;
using Orquestra.Application.UseCases.Auth.CreateRefreshTokenJWT;
using Orquestra.Application.UseCases.Auth.Logout;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Auth.Token;

namespace Orquestra.IntegrationTests.Tests.Auth;

public sealed class LogoutUserTests
{
    [Fact]
    public async Task Execute_ShouldDeleteAuthCookie_AndUpdateRefreshToken()
    {
        // Arrange;
        Guid userId = Guid.NewGuid();

        Mock<IJwtTokenGenerator> mockJwtTokenGenerator = new();
        Mock<ICreateRefreshToken> mockCreateRefreshToken = new();
        Mock<IHttpContextAccessor> mockHttpContextAccessor = new();

        // Mock cookie options;
        CookieOptions cookieOptions = new() { HttpOnly = true, Secure = true };
        mockJwtTokenGenerator.Setup(x => x.GetCookieOptions()).Returns(cookieOptions);

        // mock IResponseCookies (para verificar Delete);
        Mock<IResponseCookies> mockResponseCookies = new();
        mockResponseCookies.Setup(c => c.Delete(It.IsAny<string>(), It.IsAny<CookieOptions>())).Verifiable();

        // Mock HttpResponse e HttpContext para expor Response.Cookies;
        Mock<HttpResponse> mockHttpResponse = new();
        mockHttpResponse.Setup(r => r.Cookies).Returns(mockResponseCookies.Object);

        Mock<HttpContext> mockHttpContext = new();
        mockHttpContext.Setup(c => c.Response).Returns(mockHttpResponse.Object);

        mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(mockHttpContext.Object);

        // Mock Update do refresh token para não lançar;
        mockCreateRefreshToken.Setup(x => x.Update(It.IsAny<Guid>(), It.IsAny<bool>())).Returns(Task.CompletedTask).Verifiable();

        // SUT;
        LogoutUser sut = new(mockHttpContextAccessor.Object, mockJwtTokenGenerator.Object, mockCreateRefreshToken.Object);

        // Act;
        await sut.Execute(userId);

        // Assert
        mockResponseCookies.Verify(x => x.Delete(SystemConsts.Cookies.Auth, It.IsAny<CookieOptions>()), Times.Once);
        mockCreateRefreshToken.Verify(x => x.Update(userId, true), Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldNotThrow_WhenHttpContextIsNull()
    {
        // Arrange;
        Guid userId = Guid.NewGuid();

        var mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        var mockCreateRefreshToken = new Mock<ICreateRefreshToken>();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        // Nenhum HttpContext configurado;
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        mockJwtTokenGenerator.Setup(x => x.GetCookieOptions()).Returns(new CookieOptions());

        LogoutUser sut = new(mockHttpContextAccessor.Object, mockJwtTokenGenerator.Object, mockCreateRefreshToken.Object);

        // Act & Assert;
        await sut.Execute(userId);

        // Mesmo sem HttpContext, deve ainda chamar Update;
        mockCreateRefreshToken.Verify(x => x.Update(userId, true), Times.Once);
    }
}