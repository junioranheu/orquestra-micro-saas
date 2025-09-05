using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.Companies.GetModule;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Companies;

public sealed class GetModuleCompanyTests
{
    [Fact]
    public async Task Execute_ShouldReturnCompanyModules_WhenUserIsLinked()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        company.Modules = [ModuleEnum.Sales, ModuleEnum.Scheduling];
        await Fixture.Save(context, company);

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        GetModuleCompany sut = CreateSut(context, user);

        // Act;
        var result = await sut.Execute(user.UserId, company.CompanyId);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(company.CompanyId, result.Company.CompanyId);
        Assert.Equal(company.Modules.Length, result.Modules?.Length);
        Assert.Equal(company.Modules.Select(m => GetEnumDesc(m)), result.ModulesStr);
    }

    [Fact]
    public async Task Execute_ShouldThrowKeyNotFound_WhenCompanyDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User user = UserMock.Create();
        await Fixture.Save(context, user);

        GetModuleCompany sut = CreateSut(context, user);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, Guid.NewGuid()));
    }

    [Fact]
    public async Task Execute_ShouldThrowInvalidOperation_WhenCompanyInactive()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        company.Modules = [ModuleEnum.Sales, ModuleEnum.Scheduling];
        await Fixture.Save(context, company);

        company.Status = false;
        context.Update(company);
        await context.SaveChangesAsync();

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        GetModuleCompany sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() =>  sut.Execute(user.UserId, company.CompanyId));
    }

    #region helpers
    private static GetModuleCompany CreateSut(Context context, User user)
    {
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new (context);
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        GetModuleCompany sut = new(context, checkIfUserIsLinkedCompanyUser);

        return sut;
    }
    #endregion
}