using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.CompanyUsers;

public sealed class CheckIfUserIsLinkedCompanyUserIntegrationTests
{
    [Fact]
    public async Task Execute_ShouldReturnTrue_WhenUserIsLinkedToCompany()
    {
        (Context context, User user, Company company) = await ArrangeCompanyWithUserAsync();
        CheckIfUserIsLinkedCompanyUser sut = CreateSut(context, user);

        bool result = await sut.Execute(company.CompanyId, user.UserId, needCompanyAdmin: false);

        Assert.True(result);
    }

    [Fact]
    public async Task Execute_ShouldReturnFalse_WhenUserNotLinkedAndThrowErrorFalse()
    {
        (Context context, User user, Company company) = await ArrangeCompanyWithOtherUserAsync();
        CheckIfUserIsLinkedCompanyUser sut = CreateSut(context, user);

        bool result = await sut.Execute(company.CompanyId, user.UserId, needCompanyAdmin: false, throwError: false);

        Assert.False(result);
    }

    [Fact]
    public async Task Execute_ShouldReturnTrue_WhenCompanyIsEmptyAndUserNotLinked()
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CheckIfUserIsLinkedCompanyUser sut = CreateSut(context, user);

        bool result = await sut.Execute(company.CompanyId, user.UserId, needCompanyAdmin: false);

        Assert.True(result);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserLinkedButNotAdminAndNeedAdmin()
    {
        (Context context, User user, Company company) = await ArrangeCompanyWithUserAsync();
        CheckIfUserIsLinkedCompanyUser sut = CreateSut(context, user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(company.CompanyId, user.UserId, needCompanyAdmin: true));
    }

    [Fact]
    public async Task Execute_ShouldReturnTrue_WhenUserIsAdmin()
    {
        (Context context, User user, Company company) = await ArrangeCompanyWithUserAsync(CompanyUserRoleEnum.Administrator);
        CheckIfUserIsLinkedCompanyUser sut = CreateSut(context, user);

        bool result = await sut.Execute(company.CompanyId, user.UserId, needCompanyAdmin: true);

        Assert.True(result);
    }

    [Fact]
    public async Task Execute_ShouldReturnTrue_WhenUserIsSystemAdmin()
    {
        Context context = Fixture.CreateContext();

        User adminUser = UserMock.Create();
        adminUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, adminUser);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CheckIfUserIsLinkedCompanyUser sut = CreateSut(context, user: adminUser);

        bool result = await sut.Execute(company.CompanyId, adminUser.UserId, needCompanyAdmin: true);

        Assert.True(result);
    }

    [Fact]
    public async Task Execute_ShouldUseCache_WhenCalledMultipleTimesWithSameParameters()
    {
        // Arrange;
        (Context context, User user, Company company) = await ArrangeCompanyWithUserAsync();
        CheckIfUserIsLinkedCompanyUser sut = CreateSut(context, user);

        // Act - 1ª vez (vai chamar InternalCheck e setar no cache);
        bool firstCall = await sut.Execute(company.CompanyId, user.UserId, needCompanyAdmin: false);

        // Act - 2ª vez (mesmo parâmetro, deve vir do cache);
        bool secondCall = await sut.Execute(company.CompanyId, user.UserId, needCompanyAdmin: false);

        // Assert;
        Assert.True(firstCall);
        Assert.True(secondCall);
    }

    #region helpers
    private static async Task<(Context context, User user, Company company)> ArrangeCompanyWithUserAsync(CompanyUserRoleEnum role = CompanyUserRoleEnum.Member)
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = role
        };

        await Fixture.Save(context, companyUser);

        return (context, user, company);
    }

    private static async Task<(Context context, User user, Company company)> ArrangeCompanyWithOtherUserAsync()
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        // Outro usuário já vinculado;
        CompanyUser anotherUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = Guid.NewGuid(),
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, anotherUser);

        return (context, user, company);
    }

    private static CheckIfUserIsLinkedCompanyUser CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        return checkIfUserIsLinkedCompanyUser;
    }
    #endregion
}