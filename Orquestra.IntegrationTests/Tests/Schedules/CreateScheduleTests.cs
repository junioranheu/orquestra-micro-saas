using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Schedules;

public sealed class CheckIfUserIsLinkedCompanyUserIntegrationTests
{
    [Fact]
    public async Task Execute_ShouldReturnTrue_WhenUserIsLinkedToCompany()
    {
        // Arrange: contexto com usuário vinculado à empresa
        (Context context, User user, Company company) = await ArrangeCompanyWithUserAsync();
        CheckIfUserIsLinkedCompanyUser sut = CreateSut(context, user);

        // Act: verificar se usuário está vinculado
        bool result = await sut.Execute(company.CompanyId, user.UserId, needCompanyAdmin: false);

        // Assert: deve retornar true
        Assert.True(result);
    }

    [Fact]
    public async Task Execute_ShouldReturnFalse_WhenUserNotLinkedAndThrowErrorFalse()
    {
        // Arrange: contexto com outro usuário vinculado à empresa
        (Context context, User user, Company company) = await ArrangeCompanyWithOtherUserAsync();
        CheckIfUserIsLinkedCompanyUser sut = CreateSut(context, user);

        // Act: tentar verificar vínculo sem lançar exceção
        bool result = await sut.Execute(company.CompanyId, user.UserId, needCompanyAdmin: false, throwError: false);

        // Assert: deve retornar false
        Assert.False(result);
    }

    [Fact]
    public async Task Execute_ShouldReturnTrue_WhenCompanyIsEmptyAndUserNotLinked()
    {
        // Arrange: criar contexto, usuário e empresa vazia
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        var sut = CreateSut(context, user);

        // Act: verificar vínculo do usuário
        bool result = await sut.Execute(company.CompanyId, user.UserId, needCompanyAdmin: false);

        // Assert: deve retornar true, pois empresa está vazia
        Assert.True(result);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserLinkedButNotAdminAndNeedAdmin()
    {
        // Arrange: contexto com usuário vinculado como membro
        (Context context, User user, Company company) = await ArrangeCompanyWithUserAsync();
        CheckIfUserIsLinkedCompanyUser sut = CreateSut(context, user);

        // Act & Assert: deve lançar exceção pois precisa ser admin
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(company.CompanyId, user.UserId, needCompanyAdmin: true));
    }

    [Fact]
    public async Task Execute_ShouldReturnTrue_WhenUserIsAdmin()
    {
        // Arrange: contexto com usuário vinculado como administrador
        (Context context, User user, Company company) = await ArrangeCompanyWithUserAsync(CompanyUserRoleEnum.Administrator);
        CheckIfUserIsLinkedCompanyUser sut = CreateSut(context, user);

        // Act: verificar vínculo com necessidade de admin
        bool result = await sut.Execute(company.CompanyId, user.UserId, needCompanyAdmin: true);

        // Assert: deve retornar true
        Assert.True(result);
    }

    [Fact]
    public async Task Execute_ShouldReturnTrue_WhenUserIsSystemAdmin()
    {
        // Arrange: contexto com usuário system admin
        Context context = Fixture.CreateContext();

        User adminUser = UserMock.Create();
        adminUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, adminUser);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CheckIfUserIsLinkedCompanyUser sut = CreateSut(context, adminUser);

        // Act: verificar vínculo com necessidade de admin
        bool result = await sut.Execute(company.CompanyId, adminUser.UserId, needCompanyAdmin: true);

        // Assert: deve retornar true
        Assert.True(result);
    }

    #region Helpers
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