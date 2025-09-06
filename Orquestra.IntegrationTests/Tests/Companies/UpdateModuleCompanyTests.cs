using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.Companies.UpdateModule;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Companies;

public sealed class UpdateModuleCompanyTests
{
    [Theory]
    [InlineData(UserRoleEnum.Administrator)]
    [InlineData(UserRoleEnum.Maintainer)]
    [InlineData(UserRoleEnum.Common)]
    public async Task Execute_ShouldUpdateCompanyModules_BasedOnUserRole(UserRoleEnum role)
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        // Usuário que vai executar a ação;
        User user = UserMock.Create();
        user.Role = role;
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        company.Modules = [ModuleEnum.Sales, ModuleEnum.Scheduling];
        await Fixture.Save(context, company);

        // Usuários membros da empresa (CompanyUser);
        User memberUser1 = UserMock.Create();
        await Fixture.Save(context, memberUser1);

        CompanyUser member1 = new()
        {
            CompanyId = company.CompanyId,
            UserId = memberUser1.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Modules = [ModuleEnum.Sales, ModuleEnum.Scheduling],
            Status = true
        };

        User memberUser2 = UserMock.Create();
        await Fixture.Save(context, memberUser2);

        CompanyUser member2 = new()
        {
            CompanyId = company.CompanyId,
            UserId = memberUser2.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Modules = [ModuleEnum.Sales],
            Status = true
        };

        await Fixture.Save(context, member1);
        await Fixture.Save(context, member2);

        UpdateModuleCompany sut = CreateSut(context, user);

        CompanyUpdateModuleInput input = new()
        {
            CompanyId = company.CompanyId,
            Modules = [ModuleEnum.Sales] // Remove Scheduling;
        };

        if (role != UserRoleEnum.Common)
        {
            // Act;
            await sut.Execute(user.UserId, input);

            // Assert;
            Company? updatedCompany = await context.Companies.FindAsync(company.CompanyId);
            Assert.NotNull(updatedCompany);
            Assert.Equal(input.Modules, updatedCompany!.Modules);

            List<CompanyUser> updatedMembers = await context.CompanyUsers.Where(x => x.CompanyId == company.CompanyId).ToListAsync();

            // Member1 perdeu Scheduling;
            CompanyUser member1Updated = updatedMembers.First(x => x.UserId == memberUser1.UserId);
            Assert.Contains(ModuleEnum.Sales, member1Updated.Modules ?? []);
            Assert.DoesNotContain(ModuleEnum.Scheduling, member1Updated.Modules ?? []);

            // Member2 manteve Sales;
            CompanyUser member2Updated = updatedMembers.First(x => x.UserId == memberUser2.UserId);
            Assert.Contains(ModuleEnum.Sales, member2Updated.Modules ?? []);
        }
        else
        {
            // Usuário não admin → deve lançar UnauthorizedAccessException;
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, input));
        }
    }

    [Fact]
    public async Task Execute_ShouldNotFail_WhenCompanyHasNoUsers()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User adminUser = UserMock.Create();
        await Fixture.Save(context, adminUser);

        Company company = CompanyMock.Create();
        company.Modules = [ModuleEnum.Sales, ModuleEnum.Scheduling];
        await Fixture.Save(context, company);

        UpdateModuleCompany sut = CreateSut(context, adminUser);

        CompanyUpdateModuleInput input = new()
        {
            CompanyId = company.CompanyId,
            Modules = [ModuleEnum.Sales] // Remove Scheduling;
        };

        // Act
        var exception = await Record.ExceptionAsync(() => sut.Execute(adminUser.UserId, input));

        // Assert
        Assert.Null(exception);

        Company? updatedCompany = await context.Companies.FindAsync(company.CompanyId);
        Assert.Equal(input.Modules, updatedCompany!.Modules);
    }

    [Fact]
    public async Task Execute_ShouldRemoveAllUserModules_WhenNewModulesEmpty()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User adminUser = UserMock.Create();
        await Fixture.Save(context, adminUser);

        Company company = CompanyMock.Create();
        company.Modules = [ModuleEnum.Sales, ModuleEnum.Scheduling];
        await Fixture.Save(context, company);

        CompanyUser member = new()
        {
            CompanyId = company.CompanyId,
            UserId = adminUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Modules = [ModuleEnum.Sales, ModuleEnum.Scheduling],
            Status = true
        };

        await Fixture.Save(context, member);

        UpdateModuleCompany sut = CreateSut(context, adminUser);

        CompanyUpdateModuleInput input = new()
        {
            CompanyId = company.CompanyId,
            Modules = [] // Remove todos;
        };

        // Act;
        await sut.Execute(adminUser.UserId, input);

        // Assert;
        CompanyUser? updatedMember = await context.CompanyUsers.FindAsync(member.CompanyUserId);
        Assert.Empty(updatedMember!.Modules ?? []);

        Company? updatedCompany = await context.Companies.FindAsync(company.CompanyId);
        Assert.Empty(updatedCompany!.Modules ?? []);
    }

    [Fact]
    public async Task Execute_ShouldThrowKeyNotFound_WhenCompanyDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        UpdateModuleCompany sut = CreateSut(context, user);

        CompanyUpdateModuleInput input = new()
        {
            CompanyId = Guid.NewGuid(),
            Modules = [ModuleEnum.Sales]
        };

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrowInvalidOperation_WhenCompanyInactive()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User adminUser = UserMock.Create();
        await Fixture.Save(context, adminUser);

        Company company = CompanyMock.Create();
        company.Status = false;
        await Fixture.Save(context, company);

        company.Status = false;
        context.Update(company);
        await context.SaveChangesAsync();

        UpdateModuleCompany sut = CreateSut(context, adminUser);

        CompanyUpdateModuleInput input = new()
        {
            CompanyId = company.CompanyId,
            Modules = [ModuleEnum.Sales]
        };

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(adminUser.UserId, input));
    }

    #region helpers
    private static UpdateModuleCompany CreateSut(Context context, User user)
    {
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        CreateCompanyInvoice createCompanyInvoice = new(context, checkIfUserIsLinkedCompanyUser);

        UpdateModuleCompany updateModuleCompany = new(context, checkIfUserIsLinkedCompanyUser, createCompanyInvoice);

        return updateModuleCompany;
    }
    #endregion
}