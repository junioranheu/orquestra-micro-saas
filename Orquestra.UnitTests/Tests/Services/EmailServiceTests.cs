using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
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

        Mock<IWebHostEnvironment> envMock = new();
        envMock.SetupGet(x => x.ContentRootPath).Returns(AppContext.BaseDirectory);
        services.AddSingleton(envMock.Object);

        ServiceProvider provider = services.BuildServiceProvider();
        _emailService = provider.GetRequiredService<IEmailService>();
    }

    [Fact]
    public void RenderTemplate_ShouldReplacePlaceholders_Correctly()
    {
        // Arrange;
        string templateName = SystemConsts.Templates.EmailVerifyCompany;
        const string userName = "Junior Souza";

        Dictionary<string, string> values = new()
        {
            { "[NameApp]", SystemConsts.App.NameApp },
            { "[UserName]", userName },
            { "[CompanyName]", "Anheu" },
            { "[ConfirmLink]", "https://anheu.vercel.app/" }
        };

        // Act;
        string result = _emailService.RenderTemplate(templateName, values);

        // Assert;
        Assert.Contains(userName, result);
    }

    [Fact]
    public void RenderTemplate_ShouldThrow_FileNotFoundException_WhenTemplateMissing()
    {
        // Arrange;
        string templateName = "AEA.html";
        Dictionary<string, string> values = [];

        // Act & Assert;
        Assert.Throws<FileNotFoundException>(() => _emailService.RenderTemplate(templateName, values));
    }

    [Theory]
    [InlineData(SystemConsts.Templates.EmailSchedule)]
    [InlineData(SystemConsts.Templates.EmailVerifyCompany)]
    [InlineData(SystemConsts.Templates.EmailCreateInvoice)]
    [InlineData(SystemConsts.Templates.EmailVerifyUser)]
    [InlineData(SystemConsts.Templates.EmailVerifyCompanyUser)]
    public void EmailTemplateFile_ShouldExist_ForAllConstants(string templateFileName)
    {
        // Arrange;
        string basePath = Path.Combine(AppContext.BaseDirectory, "Services", "Email", "Templates");
        string templatePath = Path.Combine(basePath, templateFileName);

        // Act;
        bool exists = File.Exists(templatePath);

        // Assert;
        Assert.True(exists, $"O template '{templateFileName}' não foi encontrado em '{templatePath}'.");
    }
}