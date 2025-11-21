using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.CompanyUsers;

public sealed class GetCurrentMainCompanyUserTests
{
    #region GetCurrentMainCompany

    [Fact]
    public async Task GetCurrentMainCompany_ShouldReturnCurrentMainCompany_WhenExists()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            IsCurrentMainCompanyUser = true,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            UserModules = [ModuleEnum.Scheduling, ModuleEnum.Sales],
            Status = true
        };

        await Fixture.Save(context, companyUser);

        GetCurrentMainCompanyUser sut = CreateSut(context);

        // Act;
        (CompanyOutput? currentMainCompany, bool isUserAdm) = await sut.GetCurrentMainCompany(user.UserId);

        // Assert;
        Assert.NotNull(currentMainCompany);
        Assert.Equal(company.CompanyId, currentMainCompany!.CompanyId);
        Assert.Equal(company.Name, currentMainCompany.Name);
        Assert.True(isUserAdm);
        Assert.NotNull(currentMainCompany.UserModules);
        Assert.Contains(ModuleEnum.Scheduling, currentMainCompany.UserModules);
    }

    [Fact]
    public async Task GetCurrentMainCompany_ShouldReturnFalse_WhenUserIsMember()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            IsCurrentMainCompanyUser = true,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Status = true
        };

        await Fixture.Save(context, companyUser);

        GetCurrentMainCompanyUser sut = CreateSut(context);

        // Act;
        (CompanyOutput? currentMainCompany, bool isUserAdm) = await sut.GetCurrentMainCompany(user.UserId);

        // Assert;
        Assert.NotNull(currentMainCompany);
        Assert.False(isUserAdm);
    }

    [Fact]
    public async Task GetCurrentMainCompany_ShouldReturnNull_WhenNoCurrentMainCompanyUserExists()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            IsCurrentMainCompanyUser = false,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true
        };

        await Fixture.Save(context, companyUser);

        GetCurrentMainCompanyUser sut = CreateSut(context);

        // Act;
        (CompanyOutput? currentMainCompany, bool isUserAdm) = await sut.GetCurrentMainCompany(user.UserId);

        // Assert;
        Assert.Null(currentMainCompany);
        Assert.False(isUserAdm);
    }

    #endregion

    #region GetCurrentModules

    [Fact]
    public async Task GetCurrentModules_ShouldReturnModules_WhenUserIsAdmin()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            IsCurrentMainCompanyUser = true,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true
        };

        await Fixture.Save(context, companyUser);

        GetCurrentMainCompanyUser sut = CreateSut(context);

        // Act;
        (ModuleEnum[] modules, List<string> modulesStr) = await sut.GetCurrentModules(user.UserId);

        // Assert;
        Assert.NotEmpty(modules);
        Assert.NotEmpty(modulesStr);
        Assert.Contains(ModuleEnum.Scheduling, modules);
    }

    [Fact]
    public async Task GetCurrentModules_ShouldReturnModules_WhenUserIsMember()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            IsCurrentMainCompanyUser = true,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            UserModules = [ModuleEnum.Scheduling, ModuleEnum.Sales],
            Status = true
        };

        await Fixture.Save(context, companyUser);

        GetCurrentMainCompanyUser sut = CreateSut(context);

        // Act;
        (ModuleEnum[] modules, List<string> modulesStr) = await sut.GetCurrentModules(user.UserId);

        // Assert;
        Assert.Equal(2, modules.Length);
        Assert.Equal(2, modulesStr.Count);
        Assert.Contains(ModuleEnum.Sales, modules);
    }

    [Fact]
    public async Task GetCurrentModules_ShouldReturnEmpty_WhenNoCurrentMainCompanyUserExists()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            IsCurrentMainCompanyUser = false,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true
        };

        await Fixture.Save(context, companyUser);

        GetCurrentMainCompanyUser sut = CreateSut(context);

        // Act;
        (ModuleEnum[] modules, List<string> modulesStr) = await sut.GetCurrentModules(user.UserId);

        // Assert;
        Assert.Empty(modules);
        Assert.Empty(modulesStr);
    }

    #endregion

    #region helper
    private static GetCurrentMainCompanyUser CreateSut(Context context)
    {
        return new GetCurrentMainCompanyUser(context);
    }
    #endregion
}