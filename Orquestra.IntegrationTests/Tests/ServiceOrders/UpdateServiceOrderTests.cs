using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.ServiceOrders.Shared;
using Orquestra.Application.UseCases.ServiceOrders.Update;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.ServiceOrders;

public sealed class UpdateServiceOrderTests
{
    [Fact]
    public async Task Execute_ShouldThrow_WhenServiceOrderNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        UpdateServiceOrder sut = CreateSut(context, user);

        ServiceOrderInput input = new()
        {
            ServiceOrderId = Guid.NewGuid(),
            Title = "Teste",
            CompanyId = Guid.NewGuid()
        };

        // Act & Assert;
        KeyNotFoundException ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, input));

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

        // Outro user vinculado;
        User user2 = UserMock.Create();
        await Fixture.Save(context, user2);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user2.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        ServiceOrder order = ServiceOrderMock.Create(company.CompanyId);
        await Fixture.Save(context, order);

        UpdateServiceOrder sut = CreateSut(context, user);

        ServiceOrderInput input = new()
        {
            ServiceOrderId = order.ServiceOrderId,
            CompanyId = company.CompanyId,
            Title = "Atualizado"
        };

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenInputIsInvalid()
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
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        ServiceOrder order = ServiceOrderMock.Create(company.CompanyId);
        await Fixture.Save(context, order);

        UpdateServiceOrder sut = CreateSut(context, user);

        ServiceOrderInput input = new()
        {
            ServiceOrderId = order.ServiceOrderId,
            CompanyId = company.CompanyId,
            Title = "A" // Inválido;
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldUpdateServiceOrder_WhenInputIsValid()
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
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        ServiceOrder order = ServiceOrderMock.Create(company.CompanyId);
        await Fixture.Save(context, order);

        UpdateServiceOrder sut = CreateSut(context, user);

        Guid clientId = Guid.NewGuid();

        ServiceOrderInput input = new()
        {
            ServiceOrderId = order.ServiceOrderId,
            CompanyId = company.CompanyId,
            ClientId = clientId,
            Title = "OS Atualizada",
            Observation = "Nova observação",
            ExecutionDate = DateTime.Today,
            ServiceOrderStatus = ServiceOrderStatusEnum.InProgress
        };

        // Act;
        await sut.Execute(user.UserId, input);

        // Assert;
        ServiceOrder? updated = await context.ServiceOrders.FirstOrDefaultAsync(x => x.ServiceOrderId == order.ServiceOrderId);

        Assert.NotNull(updated);
        Assert.Equal("OS Atualizada", updated!.Title);
        Assert.Equal("Nova observação", updated.Observation);
        Assert.Equal(clientId, updated.ClientId);
        Assert.Equal(ServiceOrderStatusEnum.InProgress, updated.ServiceOrderStatus);
    }

    #region helpers
    private static UpdateServiceOrder CreateSut(Context context, User user)
    {
        IHttpContextAccessor accessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getAll = new(context);
        CheckIfUserIsLinkedCompanyUser check = new(getAll, accessor);

        UpdateServiceOrder updateServiceOrder = new(context, check);

        return updateServiceOrder;
    }
    #endregion
}