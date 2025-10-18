using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.CompanyUsers;

public sealed class GetAllByCompanyIdCompanyUserTests
{
    [Fact]
    public async Task Execute_Simple_ShouldReturnUsers_WhenCompanyIdIsValid()
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
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true
        };

        await Fixture.Save(context, companyUser);

        GetCompanyUserByCompanyId sut = CreateSut(context);

        // Act;
        List<CompanyUserOutput>? result = await sut.Execute(company.CompanyId);

        // Assert;
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(user.UserId, result.First().UserId);
        Assert.True(result.First().IsOwner);
    }

    [Fact]
    public async Task Execute_Simple_ShouldReturnEmptyList_WhenNoResults()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        GetCompanyUserByCompanyId sut = CreateSut(context);

        // Act;
        List<CompanyUserOutput>? result = await sut.Execute(Guid.NewGuid());

        // Assert;
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Execute_Simple_ShouldWork_WhenCompanyIdIsEmptyAndUserIdIsNull()
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
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Status = true
        };

        await Fixture.Save(context, companyUser);

        GetCompanyUserByCompanyId sut = CreateSut(context);

        // Act;
        var result = await sut.Execute(Guid.Empty, null);

        // Assert;
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task Execute_WithPagination_ShouldReturnPagedResults_WhenUserHasPermission()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User admin = UserMock.Create();
        await Fixture.Save(context, admin);

        CompanyUser adminCU = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = admin.UserId,
            Status = true
        };

        await Fixture.Save(context, adminCU);

        // Cria alguns usuários;
        for (int i = 0; i < 3; i++)
        {
            User user = UserMock.Create();
            await Fixture.Save(context, user);

            CompanyUser cu = new()
            {
                CompanyUserId = Guid.NewGuid(),
                CompanyId = company.CompanyId,
                UserId = user.UserId,
                CompanyUserRole = i == 0 ? CompanyUserRoleEnum.Member : CompanyUserRoleEnum.Administrator,
                UserModules = [ModuleEnum.Scheduling],
                Status = true
            };

            await Fixture.Save(context, cu);
        }

        PaginationInput pagination = new() { Index = 0, Limit = 10 };
        CompanyUserFilterInput input = new();

        GetCompanyUserByCompanyId sut = CreateSut(context);

        // Act;
        (IEnumerable<CompanyUserOutput> output, int count) = await sut.Execute(pagination, input, admin.UserId, company.CompanyId);

        // Assert;
        Assert.NotEmpty(output);
        Assert.Equal(4, count); // 3 + admin;
    }

    [Fact]
    public async Task Execute_WithPagination_ShouldThrow_WhenUserHasNoPermission()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };
        CompanyUserFilterInput input = new();

        GetCompanyUserByCompanyId sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(pagination, input, user.UserId, company.CompanyId));
    }

    [Fact]
    public async Task Execute_WithPagination_ShouldFilterByRole()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User admin = UserMock.Create();
        await Fixture.Save(context, admin);

        CompanyUser adminCU = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = admin.UserId,
            Status = true
        };

        await Fixture.Save(context, adminCU);

        // Adiciona um membro e um admin;
        User memberUser = UserMock.Create();

        await Fixture.Save(context, memberUser);
        await Fixture.Save(context, new CompanyUser
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = memberUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Status = true
        });

        User otherAdmin = UserMock.Create();

        await Fixture.Save(context, otherAdmin);

        await Fixture.Save(context, new CompanyUser
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = otherAdmin.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true
        });

        PaginationInput pagination = new() { Index = 0, Limit = 10 };
        CompanyUserFilterInput input = new() { CompanyUserRole = "Administrator" };

        GetCompanyUserByCompanyId sut = CreateSut(context);

        // Act;
        (IEnumerable<CompanyUserOutput> output, int count) =
            await sut.Execute(pagination, input, admin.UserId, company.CompanyId);

        // Assert;
        Assert.NotEmpty(output);
        Assert.All(output, x => Assert.Equal(CompanyUserRoleEnum.Administrator, x.CompanyUserRole));
    }

    [Fact]
    public async Task Execute_WithPagination_ShouldFilterByFullNameAndEmail()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User admin = UserMock.Create(fullName: "Admin User", email: "admin@test.com", role: UserRoleEnum.Common);
        await Fixture.Save(context, admin);

        CompanyUser adminCU = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = admin.UserId,
            Status = true
        };

        await Fixture.Save(context, adminCU);

        User targetUser = UserMock.Create(fullName: "Junior Anheu", email: "junior@teste.com", role: UserRoleEnum.Common);
        await Fixture.Save(context, targetUser);

        await Fixture.Save(context, new CompanyUser
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = targetUser.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Status = true
        });

        PaginationInput pagination = new() { Index = 0, Limit = 10 };

        CompanyUserFilterInput input = new()
        {
            FullName = "junior",
            Email = "teste"
        };

        GetCompanyUserByCompanyId sut = CreateSut(context);

        // Act;
        (IEnumerable<CompanyUserOutput> output, int count) = await sut.Execute(pagination, input, admin.UserId, company.CompanyId);

        // Assert;
        Assert.Single(output);
        Assert.Contains("junior", output.First().User!.FullName.ToLower());
    }

    #region helper
    private static GetCompanyUserByCompanyId CreateSut(Context context)
    {
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);

        return getCompanyUserByCompanyId;
    }
    #endregion
}