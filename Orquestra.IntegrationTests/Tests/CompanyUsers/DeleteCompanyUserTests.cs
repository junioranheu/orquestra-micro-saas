using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.Delete;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.CompanyUsers;

public sealed class DeleteCompanyUserTests
{
    [Theory]
    [InlineData(UserRoleEnum.Administrator)]
    [InlineData(UserRoleEnum.Maintainer)]
    public async Task Execute_ShouldDeleteCompanyUser_WhenAuthorized(UserRoleEnum role)
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        // Usuário autenticado;
        User authUser = UserMock.Create();
        authUser.Role = role;
        await Fixture.Save(context, authUser);

        // Usuário alvo;
        User targetUser = UserMock.Create();
        await Fixture.Save(context, targetUser);

        // Empresa;
        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        // Vínculo autenticado;
        CompanyUser companyUserAuth = new()
        {
            CompanyId = company.CompanyId,
            UserId = authUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true
        };

        await Fixture.Save(context, companyUserAuth);

        // Vínculo alvo;
        CompanyUser companyUserTarget = new()
        {
            CompanyId = company.CompanyId,
            UserId = targetUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Status = true
        };

        await Fixture.Save(context, companyUserTarget);

        DeleteCompanyUser sut = CreateSut(context, authUser);

        // Act;
        await sut.Execute(authUser.UserId, company.CompanyId, targetUser.UserId);

        // Assert;
        CompanyUser? deleted = await context.CompanyUsers.FindAsync(companyUserTarget.CompanyUserId);
        Assert.NotNull(deleted);
        Assert.False(deleted!.Status);
    }

    [Fact]
    public async Task Execute_ShouldThrowUnauthorized_WhenUserHasNoPermission()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Common;
        await Fixture.Save(context, authUser);

        User targetUser = UserMock.Create();
        await Fixture.Save(context, targetUser);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        // Vínculo autenticado (não administrador);
        CompanyUser companyUserAuth = new()
        {
            CompanyId = company.CompanyId,
            UserId = authUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Status = true
        };

        await Fixture.Save(context, companyUserAuth);

        // Vínculo alvo;
        CompanyUser companyUserTarget = new()
        {
            CompanyId = company.CompanyId,
            UserId = targetUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Status = true
        };

        await Fixture.Save(context, companyUserTarget);

        DeleteCompanyUser sut = CreateSut(context, authUser);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(authUser.UserId, company.CompanyId, targetUser.UserId));
    }

    [Fact]
    public async Task Execute_ShouldThrowUnauthorized_WhenOwnerTriesToDeleteHimself()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, authUser);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser owner = new()
        {
            CompanyId = company.CompanyId,
            UserId = authUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            InviterUserId = null, // Indica proprietário;
            Status = true
        };

        await Fixture.Save(context, owner);

        DeleteCompanyUser sut = CreateSut(context, authUser);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(authUser.UserId, company.CompanyId, authUser.UserId));
    }

    [Fact]
    public async Task Execute_ShouldThrowKeyNotFound_WhenTargetUserNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User authUser = UserMock.Create();
        authUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, authUser);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser companyUserAuth = new()
        {
            CompanyId = company.CompanyId,
            UserId = authUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true
        };

        await Fixture.Save(context, companyUserAuth);

        DeleteCompanyUser sut = CreateSut(context, authUser);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(authUser.UserId, company.CompanyId, Guid.NewGuid()));
    }

    #region helpers
    private static DeleteCompanyUser CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        DeleteCompanyUser deleteCompanyUser = new(context, checkIfUserIsLinkedCompanyUser);

        return deleteCompanyUser;
    }
    #endregion
}