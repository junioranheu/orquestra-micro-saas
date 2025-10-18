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
    [Fact]
    public async Task Execute_ShouldReturnCurrentMainCompany_WhenExists()
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
        (CompanyOutput? currentMainCompany, bool isUserAdm) = await sut.Execute(user.UserId);

        // Assert;
        Assert.NotNull(currentMainCompany);
        Assert.Equal(company.CompanyId, currentMainCompany!.CompanyId);
        Assert.Equal(company.Name, currentMainCompany.Name);
        Assert.True(isUserAdm);
        Assert.NotNull(currentMainCompany.UserModules);
        Assert.Contains(ModuleEnum.Scheduling, currentMainCompany.UserModules);
    }

    [Fact]
    public async Task Execute_ShouldReturnFalse_WhenUserIsMember()
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
        (CompanyOutput? currentMainCompany, bool isUserAdm) = await sut.Execute(user.UserId);

        // Assert;
        Assert.NotNull(currentMainCompany);
        Assert.False(isUserAdm);
    }

    [Fact]
    public async Task Execute_ShouldReturnNull_WhenNoCurrentMainCompanyUserExists()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        // Não marca nenhum como principal;
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
        (CompanyOutput? currentMainCompany, bool isUserAdm) = await sut.Execute(user.UserId);

        // Assert;
        Assert.Null(currentMainCompany);
        Assert.False(isUserAdm);
    }

    #region helper
    private static GetCurrentMainCompanyUser CreateSut(Context context)
    {
        GetCurrentMainCompanyUser getCurrentMainCompanyUser = new (context);

        return getCurrentMainCompanyUser;
    }
    #endregion
}