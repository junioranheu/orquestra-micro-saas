using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.Companies.Get;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Companies;

public sealed class GetCompanyTests
{
    [Fact]
    public async Task Execute_ById_ShouldReturnCompany_WhenCompanyExistsAndIsActive()
    {
        // Arrange;
        (Context context, User user, Company company) = await ArrangeCompanyWithUserAsync(status: true);

        GetCompany sut = CreateSut(context, user);

        // Act;
        CompanyOutput result = await sut.Execute(user.UserId, company.CompanyId);

        // Assert;
        Assert.Equal(company.CompanyId, result.CompanyId);
        Assert.Equal(company.Name, result.Name);
    }

    [Fact]
    public async Task Execute_ById_ShouldThrow_WhenCompanyIsInactiveAndThrowIfStatusFalseIsTrue()
    {
        // Arrange;
        (Context context, User user, Company company) = await ArrangeCompanyWithUserAsync(status: false);

        GetCompany sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(user.UserId, company.CompanyId, throwIfStatusFalse: true));
    }

    [Fact]
    public async Task Execute_ById_ShouldReturnInactiveCompany_WhenThrowIfStatusFalseIsFalse()
    {
        // Arrange;
        (Context context, User user, Company company) = await ArrangeCompanyWithUserAsync(status: false);

        GetCompany sut = CreateSut(context, user);

        // Act;
        CompanyOutput result = await sut.Execute(user.UserId, company.CompanyId, throwIfStatusFalse: false);

        // Assert;
        Assert.Equal(company.CompanyId, result.CompanyId);
    }

    [Fact]
    public async Task Execute_NoParams_ShouldReturnAllActiveCompanies()
    {
        // Arrange;
        (Context context1, User user1, Company companyActive) = await ArrangeCompanyWithUserAsync(status: true);

        (Context _, User _, Company _) = await ArrangeCompanyWithUserAsync(status: false);

        GetCompany sut = CreateSut(context1, user1);

        // Act;
        List<CompanyOutput>? result = await sut.Execute();

        // Assert;
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(companyActive.CompanyId, result[0].CompanyId);
    }

    [Fact]
    public async Task Execute_ByUser_ShouldReturnCompaniesLinkedToUser()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        // Cria user;
        User user = UserMock.Create();
        await Fixture.Save(context, user);

        // Cria primeira company e vincula ao usuário;
        Company company1 = CompanyMock.Create();
        company1.Status = true;
        await Fixture.Save(context, company1);

        CompanyUser companyUser1 = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company1.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser1);

        // Cria segunda company (no MESMO context) e vincula ao mesmo usuário;
        Company company2 = CompanyMock.Create();
        company2.Status = true;
        await Fixture.Save(context, company2);

        CompanyUser companyUser2 = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company2.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser2);

        // Cria SUT usando o mesmo user;
        GetCompany sut = CreateSut(context, user);

        // Act;
        List<CompanyOutput>? result = await sut.Execute(user.UserId);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.CompanyId == company1.CompanyId);
        Assert.Contains(result, c => c.CompanyId == company2.CompanyId);
    }

    [Fact]
    public async Task Execute_ByUser_ShouldReturnEmpty_WhenUserHasNoCompanies()
    {
        // Arrange
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        GetCompany sut = CreateSut(context, user);

        // Act
        List<CompanyOutput>? result = await sut.Execute(user.UserId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #region helpers
    private static async Task<(Context context, User user, Company company)> ArrangeCompanyWithUserAsync(CompanyUserRoleEnum role = CompanyUserRoleEnum.Member, bool status = true)
    {
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        user.Status = status;
        context.Update(user);
        await context.SaveChangesAsync();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        company.Status = status;
        context.Update(user);
        await context.SaveChangesAsync();

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = role
        };

        await Fixture.Save(context, companyUser);

        return (context, user, company);
    }

    private static GetCompany CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        GetCompany getCompany = new(context, checkIfUserIsLinkedCompanyUser);

        return getCompany;
    }
    #endregion
}