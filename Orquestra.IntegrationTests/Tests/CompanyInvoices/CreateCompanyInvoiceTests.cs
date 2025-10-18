using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.CompanyInvoices;

public sealed class CreateCompanyInvoiceTests
{
    [Fact]
    public async Task Execute_ShouldCreateInvoice_WhenUserIsAdminAndModulesValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User adminUser = UserMock.Create();
        await Fixture.Save(context, adminUser);

        CompanyUser adminCompanyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = adminUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true
        };

        await Fixture.Save(context, adminCompanyUser);

        CreateCompanyInvoice sut = CreateSut(context, adminUser);

        // Act;
        CompanyInvoice? invoice = await sut.Execute(adminUser.UserId, company.CompanyId, PlanTypeEnum.Basic);

        // Assert;
        Assert.NotNull(invoice);

        Assert.Equal(company.CompanyId, invoice.CompanyId);
        Assert.Equal(CompanyInvoiceSituationEnum.Pending, invoice.CompanyInvoiceSituation);

        CompanyInvoice? dbInvoice = await context.CompanyInvoices.FirstOrDefaultAsync(x => x.CompanyInvoiceId == invoice.CompanyInvoiceId);
        Assert.NotNull(dbInvoice);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserIsNotAdmin()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User memberUser = UserMock.Create();
        await Fixture.Save(context, memberUser);

        CompanyUser member = new()
        {
            CompanyId = company.CompanyId,
            UserId = memberUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Status = true
        };

        await Fixture.Save(context, member);

        CreateCompanyInvoice sut = CreateSut(context, memberUser);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(memberUser.UserId, company.CompanyId, PlanTypeEnum.Basic));
    }

    [Fact]
    public async Task Execute_ShouldCreateInvoice_WhenIsCreateCompanyTrue_EvenIfModulesAlreadyExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        company.PlanType = PlanTypeEnum.Basic;
        await Fixture.Save(context, company);

        User adminUser = UserMock.Create();
        await Fixture.Save(context, adminUser);

        CompanyUser adminCompanyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = adminUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true
        };

        await Fixture.Save(context, adminCompanyUser);

        CreateCompanyInvoice sut = CreateSut(context, adminUser);

        // Act;
        CompanyInvoice? invoice = await sut.Execute(adminUser.UserId, company.CompanyId, PlanTypeEnum.Basic, isCreateCompany: true);

        // Assert;
        Assert.NotNull(invoice);
        Assert.Equal(company.CompanyId, invoice.CompanyId);
        Assert.Equal(CompanyInvoiceSituationEnum.Pending, invoice.CompanyInvoiceSituation);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenCompanyDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User adminUser = UserMock.Create();
        await Fixture.Save(context, adminUser);

        ModuleEnum[] modules = [ModuleEnum.Sales];

        CreateCompanyInvoice sut = CreateSut(context, adminUser);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(adminUser.UserId, Guid.NewGuid(), PlanTypeEnum.Basic));
    }

    [Fact]
    public async Task Execute_ShouldSendEmailAfterInvoiceCreation()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User adminUser = UserMock.Create();
        await Fixture.Save(context, adminUser);

        CompanyUser adminCompanyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = adminUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true
        };

        await Fixture.Save(context, adminCompanyUser);

        ModuleEnum[] modules = [ModuleEnum.Scheduling];

        Dictionary<string, string>? capturedValues = null;
        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService(vals => capturedValues = new Dictionary<string, string>(vals));

        CreateCompanyInvoice sut = CreateSut(context, adminUser, emailServiceMock);

        // Act;
        CompanyInvoice? invoice = await sut.Execute(adminUser.UserId, company.CompanyId, PlanTypeEnum.Basic, isCreateCompany: true);

        // Assert;
        Assert.NotNull(invoice);
        Assert.NotNull(capturedValues);
        Assert.Equal(company.Name, capturedValues!["[CompanyName]"]);
        Assert.Equal(invoice.Amount.ToString(), capturedValues!["[Price]"]);

        emailServiceMock.Verify(
            x => x.SendEmail(
                It.IsAny<string>(),        // to;
                It.IsAny<string>(),        // subject;
                It.IsAny<string>(),        // body;
                It.IsAny<bool>(),          // cc;
                It.IsAny<List<string>>()   // ccList;
            ),
            Times.Once
        );
    }

    #region helpers
    private static CreateCompanyInvoice CreateSut(Context context, User user, Mock<IEmailService>? emailServiceMock = null)
    {
        IConfiguration config = Fixture.CreateConfiguration();
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        IWebHostEnvironment env = Fixture.CreateDevelopmentEnvironment();
        EnvService envService = new(env, config);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        emailServiceMock ??= Fixture.CreateEmailService();

        CreateCompanyInvoice createCompanyInvoice = new(context, checkIfUserIsLinkedCompanyUser, envService, emailServiceMock.Object);

        return createCompanyInvoice;
    }
    #endregion
}