using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Orquestra.Application.UseCases.Companies.CalculatePrice;
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
using static Orquestra.Utils.Fixtures.Get;

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

        ModuleEnum[] modules = [ModuleEnum.Sales, ModuleEnum.Scheduling];

        // Act;
        CompanyInvoice? invoice = await sut.Execute(adminUser.UserId, company.CompanyId, modules);

        // Assert;
        Assert.NotNull(invoice);

        List<string> modulesList = [.. modules.Select(x => GetEnumDesc(x))];

        foreach (var module in modules)
        {
            Assert.Contains(GetEnumDesc(module), invoice.Description);
        }

        Assert.Equal(company.CompanyId, invoice.CompanyId);
        Assert.Equal(CompanyInvoiceSituationEnum.Pending, invoice.CompanyInvoiceSituation);

        decimal expected = ModuleHelper.GetPrice(ModuleEnum.Sales) + ModuleHelper.GetPrice(ModuleEnum.Scheduling);
        Assert.True(invoice?.Amount <= expected, $"Esperado que {invoice.Amount} seja menor ou igual a {expected} (por descontos e valor proporcional).");

        CompanyInvoice? dbInvoice = await context.CompanyInvoices.FirstOrDefaultAsync(x => x.CompanyInvoiceId == invoice.CompanyInvoiceId);
        Assert.NotNull(dbInvoice);
    }

    [Fact]
    public async Task Execute_ShouldCreateInvoice_WithSingleModule_AndProperDescription()
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

        ModuleEnum[] modules = [ModuleEnum.Sales];

        // Act;
        CompanyInvoice? invoice = await sut.Execute(adminUser.UserId, company.CompanyId, modules);

        List<string> modulesList = [.. modules.Select(x => GetEnumDesc(x))];

        // Assert;
        foreach (var module in modules)
        {
            Assert.Contains(GetEnumDesc(module), invoice?.Description);
        }

        decimal expected = ModuleHelper.GetPrice(ModuleEnum.Sales);
        Assert.True(invoice?.Amount <= expected, $"Esperado que {invoice.Amount} seja menor ou igual a {expected} (por descontos e valor proporcional).");
    }

    [Fact]
    public async Task Execute_ShouldReturnNull_WhenModulesAreEmpty()
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

        ModuleEnum[] modules = [];

        // Act;
        CompanyInvoice? invoice = await sut.Execute(adminUser.UserId, company.CompanyId, modules);

        // Act & Assert;
        Assert.Null(invoice);
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
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(memberUser.UserId, company.CompanyId, [ModuleEnum.Sales]));
    }

    [Fact]
    public async Task Execute_ShouldCreateInvoice_WhenIsCreateCompanyTrue_EvenIfModulesAlreadyExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        company.Modules = [ModuleEnum.Sales];
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

        ModuleEnum[] modules = [ModuleEnum.Sales, ModuleEnum.Scheduling];

        CreateCompanyInvoice sut = CreateSut(context, adminUser);

        // Act;
        CompanyInvoice? invoice = await sut.Execute(adminUser.UserId, company.CompanyId, modules, isCreateCompany: true);

        // Assert;
        Assert.NotNull(invoice);
        Assert.Equal(company.CompanyId, invoice.CompanyId);
        Assert.Contains(GetEnumDesc(ModuleEnum.Sales), invoice.Description);
        Assert.Contains(GetEnumDesc(ModuleEnum.Sales), invoice.Description);
        Assert.Equal(CompanyInvoiceSituationEnum.Pending, invoice.CompanyInvoiceSituation);
    }

    [Fact]
    public async Task Execute_ShouldReturnNull_WhenAllModulesAlreadyExist_AndIsCreateCompanyFalse()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        company.Modules = [ModuleEnum.Sales, ModuleEnum.Scheduling];
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

        ModuleEnum[] modules = [ModuleEnum.Sales, ModuleEnum.Scheduling];

        CreateCompanyInvoice sut = CreateSut(context, adminUser);

        // Act;
        CompanyInvoice? invoice = await sut.Execute(adminUser.UserId, company.CompanyId, modules, isCreateCompany: false);

        // Assert;
        Assert.Null(invoice);
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
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(adminUser.UserId, Guid.NewGuid(), modules));
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

        // Act
        CompanyInvoice? invoice = await sut.Execute(adminUser.UserId, company.CompanyId, modules, isCreateCompany: true);

        // Assert
        Assert.NotNull(invoice);
        Assert.NotNull(capturedValues);
        Assert.Equal(company.Name, capturedValues!["[CompanyName]"]);
        Assert.Equal(invoice.Amount.ToString(), capturedValues!["[Price]"]);
        Assert.Contains(GetEnumDesc(ModuleEnum.Scheduling), capturedValues!["[ModuleDescription]"]);

        emailServiceMock.Verify(
            x => x.SendEmail(
                It.IsAny<string>(),        // to;
                It.IsAny<string>(),        // subject;
                It.IsAny<string>(),        // body;
                It.IsAny<bool>(),          // cc;
                It.IsAny<IEnumerable<string>>()  // ccList;
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
        CalculatePriceModuleCompany calculatePriceModuleCompany = new(context, checkIfUserIsLinkedCompanyUser);

        emailServiceMock ??= Fixture.CreateEmailService();

        CreateCompanyInvoice createCompanyInvoice = new(context, checkIfUserIsLinkedCompanyUser, calculatePriceModuleCompany, envService, emailServiceMock.Object);


        return createCompanyInvoice;
    }
    #endregion
}