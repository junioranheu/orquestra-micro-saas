using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.Companies.GetModule;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Application.UseCases.CompanyUsers.UpdateModule;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.CompanyUsers;

public sealed class UpdateModuleCompanyUserTests
{
    [Theory]
    [InlineData(UserRoleEnum.Administrator)]
    [InlineData(UserRoleEnum.Maintainer)]
    public async Task Execute_ShouldUpdateUserModules_WhenUserIsAdminOrMaintainer(UserRoleEnum role)
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        // Usuário que executa a ação;
        User adminUser = UserMock.Create();
        adminUser.Role = role;
        await Fixture.Save(context, adminUser);

        // Usuário que terá módulos atualizados;
        User targetUser = UserMock.Create();
        await Fixture.Save(context, targetUser);

        // Empresa;
        Company company = CompanyMock.Create();
        company.CompanyModules = [ModuleEnum.Sales, ModuleEnum.Scheduling];
        await Fixture.Save(context, company);

        // Vínculo do usuário alvo;
        CompanyUser targetCompanyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = targetUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            UserModules = [ModuleEnum.Sales, ModuleEnum.Scheduling],
            Status = true
        };

        await Fixture.Save(context, targetCompanyUser);

        UpdateModuleCompanyUser sut = CreateSut(context, adminUser);

        CompanyUserUpdateModuleInput input = new()
        {
            CompanyId = company.CompanyId,
            UserId = targetUser.UserId,
            UserModules = [ModuleEnum.Sales] // Remove Scheduling;
        };

        // Act;
        await sut.Execute(adminUser.UserId, input);

        // Assert;
        CompanyUser? updated = await context.CompanyUsers.FindAsync(targetCompanyUser.CompanyUserId);
        Assert.NotNull(updated);
        Assert.Contains(ModuleEnum.Sales, updated!.UserModules ?? []);
        Assert.DoesNotContain(ModuleEnum.Scheduling, updated.UserModules ?? []);
    }

    [Fact]
    public async Task Execute_ShouldThrowUnauthorized_WhenUserIsMember()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User commonUser = UserMock.Create();
        await Fixture.Save(context, commonUser);

        User targetUser = UserMock.Create();
        await Fixture.Save(context, targetUser);

        Company company = CompanyMock.Create();
        company.CompanyModules = [ModuleEnum.Sales];
        await Fixture.Save(context, company);

        CompanyUser targetCompanyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = targetUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            UserModules = [ModuleEnum.Sales],
            Status = true
        };

        await Fixture.Save(context, targetCompanyUser);

        UpdateModuleCompanyUser sut = CreateSut(context, commonUser);

        CompanyUserUpdateModuleInput input = new()
        {
            CompanyId = company.CompanyId,
            UserId = targetUser.UserId,
            UserModules = [ModuleEnum.Sales]
        };

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(targetUser.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrowInvalidOperation_WhenModulesNotInCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User adminUser = UserMock.Create();
        adminUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, adminUser);

        User targetUser = UserMock.Create();
        await Fixture.Save(context, targetUser);

        Company company = CompanyMock.Create();
        company.CompanyModules = [ModuleEnum.Sales]; // Somente Sales;
        await Fixture.Save(context, company);

        CompanyUser targetCompanyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = targetUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            UserModules = [ModuleEnum.Sales],
            Status = true
        };

        await Fixture.Save(context, targetCompanyUser);

        UpdateModuleCompanyUser sut = CreateSut(context, adminUser);

        CompanyUserUpdateModuleInput input = new()
        {
            CompanyId = company.CompanyId,
            UserId = targetUser.UserId,
            UserModules = [ModuleEnum.Scheduling] // Módulo inválido;
        };

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(adminUser.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrowKeyNotFound_WhenUserNotLinked()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User adminUser = UserMock.Create();
        adminUser.Role = UserRoleEnum.Administrator;
        await Fixture.Save(context, adminUser);

        User targetUser = UserMock.Create();
        await Fixture.Save(context, targetUser);

        Company company = CompanyMock.Create();
        company.CompanyModules = [ModuleEnum.Sales];
        await Fixture.Save(context, company);

        UpdateModuleCompanyUser sut = CreateSut(context, adminUser);

        CompanyUserUpdateModuleInput input = new()
        {
            CompanyId = company.CompanyId,
            UserId = targetUser.UserId,
            UserModules = [ModuleEnum.Sales]
        };

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(adminUser.UserId, input));
    }

    #region helpers
    private static UpdateModuleCompanyUser CreateSut(Context context, User user)
    {
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetModuleCompany getModuleCompany = new(context, checkIfUserIsLinkedCompanyUser);

        UpdateModuleCompanyUser updateModuleCompanyUser = new(context, checkIfUserIsLinkedCompanyUser, getModuleCompany);

        return updateModuleCompanyUser;
    }
    #endregion
}