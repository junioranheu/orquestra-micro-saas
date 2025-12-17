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

public sealed class CreateServiceOrderTests
{
    [Fact]
    public async Task Execute_ShouldThrow_WhenInputIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CreateServiceOrder sut = CreateSut(context, user);

        ServiceOrderInput input = new()
        {
            Title = "A", // Inválido;
            CompanyId = Guid.NewGuid(),
            Observation = "desc"
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldCreateServiceOrder_WhenInputIsValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CreateServiceOrder sut = CreateSut(context, user);

        Guid companyId = Guid.NewGuid();

        ServiceOrderInput input = new()
        {
            Title = "Ordem de Serviço Teste",
            Observation = "Descrição válida",
            CompanyId = companyId
        };

        // Act;
        await sut.Execute(user.UserId, input);

        // Assert;
        ServiceOrder? serviceOrder = context.ServiceOrders .FirstOrDefault(x => x.Title == "Ordem de Serviço Teste");

        Assert.NotNull(serviceOrder);
        Assert.Equal(companyId, serviceOrder!.CompanyId);
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

        // Cria um outro usuário vinculado à empresa;
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
            Observation = "desc",
            CompanyId = company.CompanyId
        };

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldPersistOnlyOneServiceOrder()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CreateServiceOrder sut = CreateSut(context, user);

        ServiceOrderInput input = new()
        {
            Title = "OS Única",
            Observation = "desc",
            CompanyId = Guid.NewGuid()
        };

        // Act;
        await sut.Execute(user.UserId, input);

        // Assert;
        int count = context.ServiceOrders.Count(x => x.Title == "OS Única");
        Assert.Equal(1, count);
    }

    #region helpers
    private static CreateServiceOrder CreateSut(Context context, User user)
    {
        IHttpContextAccessor accessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getAll = new(context);
        CheckIfUserIsLinkedCompanyUser check = new(getAll, accessor);

        CreateServiceOrder createServiceOrder = new (context, check);

        return createServiceOrder;
    }
    #endregion
}