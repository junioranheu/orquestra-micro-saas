using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
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
        CompanyInvoice invoice = await sut.Execute(adminUser.UserId, company.CompanyId, modules);

        // Assert;
        Assert.NotNull(invoice);

        List<string> modulesList = [.. modules.Select(x => GetEnumDesc(x))];

        foreach (var module in modules)
        {
            Assert.Contains(GetEnumDesc(module), invoice.Description);
        }

        Assert.Equal(company.CompanyId, invoice.CompanyId);
        Assert.Equal(ModuleHelper.GetPrice(ModuleEnum.Sales) + ModuleHelper.GetPrice(ModuleEnum.Scheduling), invoice.Amount);
        Assert.Equal(CompanyInvoiceSituationEnum.Pending, invoice.CompanyInvoiceSituation);

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
        CompanyInvoice invoice = await sut.Execute(adminUser.UserId, company.CompanyId, modules);

        List<string> modulesList = [.. modules.Select(x => GetEnumDesc(x))];

        // Assert;
        foreach (var module in modules)
        {
            Assert.Contains(GetEnumDesc(module), invoice.Description);
        }

        Assert.Equal(ModuleHelper.GetPrice(ModuleEnum.Sales), invoice.Amount);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenModulesAreEmpty()
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

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(adminUser.UserId, company.CompanyId, []));
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

    #region helpers
    private static CreateCompanyInvoice CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        return new CreateCompanyInvoice(context, checkIfUserIsLinkedCompanyUser);
    }
    #endregion
}