using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.GetModule;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.CompanyUsers;

public sealed class GetModuleCompanyUserTests
{
    [Fact]
    public async Task Execute_ShouldReturnUserModules_WhenUserIsLinked()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Modules = [ModuleEnum.Sales, ModuleEnum.Scheduling],
            Status = true
        };

        await Fixture.Save(context, companyUser);

        GetModuleCompanyUser sut = CreateSut(context, user);

        // Act;
        var (modules, modulesStr) = await sut.Execute(user.UserId, company.CompanyId);

        // Assert;
        Assert.NotNull(modules);
        Assert.NotNull(modulesStr);
        Assert.Equal(companyUser.Modules.Length, modules.Length);
        Assert.Equal(companyUser.Modules.Select(x => GetEnumDesc(x)), modulesStr);
    }

    [Fact]
    public async Task Execute_ShouldThrowKeyNotFound_WhenUserNotLinked()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        GetModuleCompanyUser sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, company.CompanyId));
    }

    [Fact]
    public async Task Execute_ShouldThrowInvalidOperation_WhenUserInactive()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User user = UserMock.Create();
        await Fixture.Save(context, user);

        user.Status = false;
        context.Update(user);
        await context.SaveChangesAsync();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Status = false
        };

        await Fixture.Save(context, companyUser);

        GetModuleCompanyUser sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(user.UserId, company.CompanyId));
    }

    #region helpers
    private static GetModuleCompanyUser CreateSut(Context context, User user)
    {
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new (context);
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new (getCompanyUserByCompanyId, httpContextAccessor);

        GetModuleCompanyUser getModuleCompanyUser = new (context, checkIfUserIsLinkedCompanyUser);

        return getModuleCompanyUser;
    }
    #endregion
}