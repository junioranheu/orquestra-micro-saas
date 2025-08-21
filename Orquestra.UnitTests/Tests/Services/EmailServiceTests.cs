using Microsoft.Extensions.DependencyInjection;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Email.Models;

namespace Orquestra.UnitTests.Tests.Services;

public sealed class EmailServiceTests
{
    private readonly IEmailService _emailService;

    public EmailServiceTests()
    {
        // Configura DI real;
        ServiceCollection services = new();

        EmailSettings emailSettingsMock = new() { Password = "xxx" };
        services.AddSingleton(emailSettingsMock);
        services.AddTransient<IEmailService, EmailService>();

        ServiceProvider provider = services.BuildServiceProvider();
        _emailService = provider.GetRequiredService<IEmailService>();
    }

    [Fact]
    public void RenderTemplate_ShouldReplacePlaceholders_Correctly()
    {
        // Arrange
        string templateName = "EmailVerifyCompany.html";
        const string userName = "Junior Souza";

        Dictionary<string, string> values = new()
        {
            { "[NameApp]", SystemConsts.NameApp },
            { "[UserName]", userName },
            { "[CompanyName]", "Anheu" },
            { "[ConfirmLink]", $"{SystemConsts.EmailVerifyCompany}?id=aea" }
        };

        // Act
        string result = _emailService.RenderTemplate(templateName, values);

        // Assert
        Assert.Contains(userName, result);
    }

    [Fact]
    public void RenderTemplate_ShouldThrow_FileNotFoundException_WhenTemplateMissing()
    {
        // Arrange
        string templateName = "AEA.html";
        Dictionary<string, string> values = [];

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => _emailService.RenderTemplate(templateName, values));
    }
}