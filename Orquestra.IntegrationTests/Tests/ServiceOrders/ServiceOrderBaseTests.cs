using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.ServiceOrders.Create;
using Orquestra.Application.UseCases.ServiceOrders.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.ServiceOrders;

public sealed class ServiceOrderBaseTests
{
    [Fact]
    public async Task Validate_ShouldThrow_WhenUserIsNotLinkedToCompany()
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

        CreateServiceOrder sut = CreateSut(context, user);

        ServiceOrderInput input = new()
        {
            Title = "OS Teste",
            Observation = "obs",
            CompanyId = company.CompanyId
        };

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenTitleIsInvalid()
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

        CreateServiceOrder sut = CreateSut(context, user);

        ServiceOrderInput input = new()
        {
            Title = "A", // inválido
            Observation = "obs",
            CompanyId = company.CompanyId
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenObservationIsInvalid()
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

        CreateServiceOrder sut = CreateSut(context, user);

        ServiceOrderInput input = new()
        {
            Title = "OS válida",
            Observation = new string('x', 2000), // Inválida pelo CommonForBases;
            CompanyId = company.CompanyId
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Validate_ShouldNormalizeExecutionDate_WhenInvalidDefaultDate()
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

        CreateServiceOrder sut = CreateSut(context, user);

        ServiceOrderInput input = new()
        {
            Title = "OS válida",
            Observation = "obs",
            CompanyId = company.CompanyId,
            ExecutionDate = DateTime.MinValue
        };

        // Act;
        await sut.Execute(user.UserId, input);

        // Assert;
        ServiceOrder? order = context.ServiceOrders.FirstOrDefault(x => x.Title == "OS válida");

        Assert.NotNull(order);
        Assert.Equal(default, order!.ExecutionDate);
    }

    #region helpers
    private static CreateServiceOrder CreateSut(Context context, User user)
    {
        IHttpContextAccessor accessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getAll = new(context);
        CheckIfUserIsLinkedCompanyUser check = new(getAll, accessor);

        CreateServiceOrder createServiceOrder = new(context, check);

        return createServiceOrder;
    }
    #endregion
}