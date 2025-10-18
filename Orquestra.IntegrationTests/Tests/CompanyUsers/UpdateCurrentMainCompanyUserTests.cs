using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMain;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.CompanyUsers;

public sealed class UpdateCurrentMainCompanyUserTests
{
    [Fact]
    public async Task Execute_ShouldSetCurrentMainCompanyUser_AndUnsetOthers()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company companyA = CompanyMock.Create();
        await Fixture.Save(context, companyA);

        Company companyB = CompanyMock.Create();
        await Fixture.Save(context, companyB);

        CompanyUser companyUserA = new()
        {
            CompanyId = companyA.CompanyId,
            UserId = user.UserId,
            Status = true,
            IsCurrentMainCompanyUser = false
        };

        await Fixture.Save(context, companyUserA);

        CompanyUser companyUserB = new()
        {
            CompanyId = companyB.CompanyId,
            UserId = user.UserId,
            Status = true,
            IsCurrentMainCompanyUser = true
        };

        await Fixture.Save(context, companyUserB);

        UpdateCurrentMainCompanyUser sut = CreateSut(context, user);

        // Act;
        await sut.Execute(user.UserId, companyA.CompanyId);

        // Assert;
        CompanyUser? updatedA = await context.CompanyUsers.FindAsync(companyUserA.CompanyUserId);
        CompanyUser? updatedB = await context.CompanyUsers.FindAsync(companyUserB.CompanyUserId);

        Assert.True(updatedA!.IsCurrentMainCompanyUser);
        Assert.False(updatedB!.IsCurrentMainCompanyUser);
    }

    [Fact]
    public async Task Execute_ShouldThrowKeyNotFound_WhenUserHasNoCompanyUsers()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        UpdateCurrentMainCompanyUser sut = CreateSut(context, user);

        // Act & Assert;
        KeyNotFoundException ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, company.CompanyId));

        Assert.Equal(SystemConsts.Warnings.InvalidLinkedCompanyUser, ex.Message);
    }

    [Fact]
    public async Task Execute_ShouldThrowKeyNotFound_WhenLinkedCompanyUserIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company companyA = CompanyMock.Create();
        await Fixture.Save(context, companyA);

        Company companyB = CompanyMock.Create();
        await Fixture.Save(context, companyB);

        CompanyUser companyUserB = new()
        {
            CompanyId = companyB.CompanyId,
            UserId = user.UserId,
            Status = true
        };

        await Fixture.Save(context, companyUserB);

        UpdateCurrentMainCompanyUser sut = CreateSut(context, user);

        // Act & Assert;
        KeyNotFoundException ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, companyA.CompanyId));

        Assert.Equal(SystemConsts.Warnings.InvalidLinkedCompanyUser, ex.Message);
    }

    [Fact]
    public async Task Execute_ShouldNotChange_WhenAlreadyCurrentMain()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            Status = true,
            IsCurrentMainCompanyUser = true
        };

        await Fixture.Save(context, companyUser);

        UpdateCurrentMainCompanyUser sut = CreateSut(context, user);

        // Act;
        await sut.Execute(user.UserId, company.CompanyId);

        // Assert;
        CompanyUser? updated = await context.CompanyUsers.FindAsync(companyUser.CompanyUserId);
        Assert.True(updated!.IsCurrentMainCompanyUser);
    }

    #region helpers
    private static UpdateCurrentMainCompanyUser CreateSut(Context context, User user)
    {
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        UpdateCurrentMainCompanyUser updateCurrentMainCompanyUser = new(context, checkIfUserIsLinkedCompanyUser);

        return updateCurrentMainCompanyUser;
    }
    #endregion
}