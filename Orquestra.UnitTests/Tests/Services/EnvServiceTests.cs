using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.Infrastructure.Services.Env.Models;

namespace Orquestra.UnitTests.Tests.Services;

public sealed class EnvServiceTests
{
    [Fact]
    public void IsDevelopment_ShouldReturnTrue_WhenEnvironmentIsDevelopment()
    {
        // Arrange;
        Mock<IWebHostEnvironment> mockEnv = new();
        mockEnv.Setup(e => e.EnvironmentName).Returns(Environments.Development);

        Mock<IConfiguration> mockConfig = new();

        EnvService service = new(mockEnv.Object, mockConfig.Object);

        // Act;
        bool result = service.IsDevelopment();

        // Assert;
        Assert.True(result);
    }

    [Fact]
    public void IsDevelopment_ShouldReturnFalse_WhenEnvironmentIsProduction()
    {
        // Arrange;
        Mock<IWebHostEnvironment> mockEnv = new();
        mockEnv.Setup(e => e.EnvironmentName).Returns(Environments.Production);

        Mock<IConfiguration> mockConfig = new();

        EnvService service = new(mockEnv.Object, mockConfig.Object);

        // Act;
        bool result = service.IsDevelopment();

        // Assert;
        Assert.False(result);
    }

    [Fact]
    public void GetUrls_ShouldReturnUrls_WhenInDevelopment()
    {
        // Arrange;
        Mock<IWebHostEnvironment> mockEnv = new();
        mockEnv.Setup(e => e.EnvironmentName).Returns(Environments.Development);

        Mock<IConfigurationSection> urlsSectionMock = new();
        urlsSectionMock.Setup(s => s["Backend"]).Returns("http://localhost:5000");
        urlsSectionMock.Setup(s => s["Frontend"]).Returns("http://localhost:3000");

        Mock<IConfiguration> mockConfig = new();
        mockConfig.Setup(c => c.GetSection("Urls:Development")).Returns(urlsSectionMock.Object);

        EnvService service = new(mockEnv.Object, mockConfig.Object);

        // Act;
        EnvOutput result = service.GetUrls();

        // Assert;
        Assert.Equal("http://localhost:5000", result.UrlBackend);
        Assert.Equal("http://localhost:3000", result.UrlFrontend);
    }

    [Fact]
    public void GetUrls_ShouldReturnUrls_WhenInProduction()
    {
        // Arrange;
        Mock<IWebHostEnvironment> mockEnv = new();
        mockEnv.Setup(e => e.EnvironmentName).Returns(Environments.Production);

        Mock<IConfigurationSection> urlsSectionMock = new();
        urlsSectionMock.Setup(s => s["Backend"]).Returns("https://api.meusite.com");
        urlsSectionMock.Setup(s => s["Frontend"]).Returns("https://meusite.com");

        Mock<IConfiguration> mockConfig = new();
        mockConfig.Setup(c => c.GetSection("Urls:Production")).Returns(urlsSectionMock.Object);

        EnvService service = new(mockEnv.Object, mockConfig.Object);

        // Act;
        EnvOutput result = service.GetUrls();

        // Assert;
        Assert.Equal("https://api.meusite.com", result.UrlBackend);
        Assert.Equal("https://meusite.com", result.UrlFrontend);
    }

    [Fact]
    public void GetUrls_ShouldThrowException_WhenBackendUrlMissing()
    {
        // Arrange;
        Mock<IWebHostEnvironment> mockEnv = new();
        mockEnv.Setup(e => e.EnvironmentName).Returns(Environments.Development);

        Mock<IConfigurationSection> urlsSectionMock = new();
        urlsSectionMock.Setup(s => s["Backend"]).Returns((string?)null);
        urlsSectionMock.Setup(s => s["Frontend"]).Returns("http://localhost:3000");

        Mock<IConfiguration> mockConfig = new();
        mockConfig.Setup(c => c.GetSection("Urls:Development")).Returns(urlsSectionMock.Object);

        EnvService service = new(mockEnv.Object, mockConfig.Object);

        // Act & Assert;
        Exception ex = Assert.Throws<InvalidOperationException>(() => service.GetUrls());
        Assert.Equal("Erro crítico interno: URLs back-end não definidas.", ex.Message);
    }

    [Fact]
    public void GetUrls_ShouldThrowException_WhenFrontendUrlMissing()
    {
        // Arrange;
        Mock<IWebHostEnvironment> mockEnv = new();
        mockEnv.Setup(e => e.EnvironmentName).Returns(Environments.Development);

        Mock<IConfigurationSection> urlsSectionMock = new();
        urlsSectionMock.Setup(s => s["Backend"]).Returns("http://localhost:5000");
        urlsSectionMock.Setup(s => s["Frontend"]).Returns((string?)null);

        Mock<IConfiguration> mockConfig = new();
        mockConfig.Setup(c => c.GetSection("Urls:Development")).Returns(urlsSectionMock.Object);

        EnvService service = new(mockEnv.Object, mockConfig.Object);

        // Act & Assert;
        Exception ex = Assert.Throws<InvalidOperationException>(() => service.GetUrls());
        Assert.Equal("Erro crítico interno: URLs front-end não definidas.", ex.Message);
    }
}