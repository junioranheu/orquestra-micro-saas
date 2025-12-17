using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.ServiceOrders.Delete;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.ServiceOrders;

public sealed class DeleteServiceOrderTests
{
    [Fact]
    public async Task Execute_ShouldThrow_WhenServiceOrderNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        DeleteServiceOrder sut = CreateSut(context, user);

        Guid invalidId = Guid.NewGuid();

        // Act & Assert;
        KeyNotFoundException ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, invalidId));

        Assert.Equal(SystemConsts.Warnings.NotFoundData, ex.Message);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserIsNotLinkedToCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        // Outro user admin;
        User admin = UserMock.Create();
        await Fixture.Save(context, admin);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = admin.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator
        };

        await Fixture.Save(context, companyUser);

        ServiceOrder order = ServiceOrderMock.Create(company.CompanyId);
        await Fixture.Save(context, order);

        DeleteServiceOrder sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, order.ServiceOrderId));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserIsNotAdmin()
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
            CompanyUserRole = CompanyUserRoleEnum.Member // Não admin;
        };

        await Fixture.Save(context, companyUser);

        ServiceOrder order = ServiceOrderMock.Create(company.CompanyId);
        await Fixture.Save(context, order);

        DeleteServiceOrder sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, order.ServiceOrderId));
    }

    [Fact]
    public async Task Execute_ShouldSoftDeleteServiceOrder_WhenUserIsAdmin()
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
            CompanyUserRole = CompanyUserRoleEnum.Administrator
        };

        await Fixture.Save(context, companyUser);

        ServiceOrder order = ServiceOrderMock.Create(company.CompanyId);
        await Fixture.Save(context, order);

        DeleteServiceOrder sut = CreateSut(context, user);

        // Act;
        await sut.Execute(user.UserId, order.ServiceOrderId);

        // Assert;
        ServiceOrder? deleted = await context.ServiceOrders.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.QuoteId == order.QuoteId);

        Assert.NotNull(deleted);
        Assert.False(deleted!.Status);
    }

    #region helpers
    private static DeleteServiceOrder CreateSut(Context context, User user)
    {
        IHttpContextAccessor accessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getAll = new(context);
        CheckIfUserIsLinkedCompanyUser check = new(getAll, accessor);

        DeleteServiceOrder deleteServiceOrder = new(context, check);

        return deleteServiceOrder;
    }
    #endregion
}