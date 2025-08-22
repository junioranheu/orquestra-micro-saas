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
        // Arrange
        using Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser sut = new(getCompanyUserByCompanyId, httpContextAccessor);

        // Act
        bool result = await sut.Execute(company.CompanyId, user.UserId, needCompanyAdmin: false);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Execute_ShouldReturnFalse_WhenUserNotLinkedAndThrowErrorFalse()
    {
        using Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        // Adiciona outro usuário na empresa (não o testado);
        CompanyUser anotherUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = Guid.NewGuid(),
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, anotherUser);

        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser sut = new(getCompanyUserByCompanyId, httpContextAccessor);

        bool result = await sut.Execute(company.CompanyId, user.UserId, needCompanyAdmin: false, throwError: false);

        Assert.False(result);
    }

    [Fact]
    public async Task Execute_ShouldReturnTrue_WhenCompanyIsEmptyAndUserNotLinked()
    {
        using Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        // Nenhum usuário na empresa ainda;

        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser sut = new(getCompanyUserByCompanyId, httpContextAccessor);

        bool result = await sut.Execute(company.CompanyId, user.UserId, needCompanyAdmin: false);

        Assert.True(result); // Empresa nova sempre permite;
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserLinkedButNotAdminAndNeedAdmin()
    {
        using Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser sut = new(getCompanyUserByCompanyId, httpContextAccessor);

        await Assert.ThrowsAsync<Exception>(() => sut.Execute(company.CompanyId, user.UserId, needCompanyAdmin: true));
    }

    [Fact]
    public async Task Execute_ShouldReturnTrue_WhenUserIsOwner()
    {
        using Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Owner
        };

        await Fixture.Save(context, companyUser);

        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser sut = new(getCompanyUserByCompanyId, httpContextAccessor);

        bool result = await sut.Execute(company.CompanyId, user.UserId, needCompanyAdmin: true);

        Assert.True(result);
    }

    [Fact]
    public async Task Execute_ShouldReturnTrue_WhenUserIsSystemAdmin()
    {
        using Context context = Fixture.CreateContext();

        User adminUser = UserMock.Create();
        adminUser.Role = UserRoleEnum.Admin;
        await Fixture.Save(context, adminUser);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(adminUser);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser sut = new(getCompanyUserByCompanyId, httpContextAccessor);

        bool result = await sut.Execute(company.CompanyId, adminUser.UserId, needCompanyAdmin: true);

        Assert.True(result);
    }
}