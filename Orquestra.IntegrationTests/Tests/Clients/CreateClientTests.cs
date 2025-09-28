using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Clients.Create;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Clients;

public sealed class CreateClientTests
{
    [Fact]
    public async Task Execute_ShouldCreateClient_WhenInputIsValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CreateClient sut = CreateSut(context, user);

        Client client = ClientMock.Create();
        ClientInput input = client.Adapt<ClientInput>();

        // Act;
        await sut.Execute(user.UserId, input);

        // Assert;
        Client? createdClient = await context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.ClientId == input.ClientId);
        Assert.NotNull(createdClient);
        Assert.Equal(input.FullName, createdClient.FullName);
        Assert.Equal(input.Email, createdClient.Email);
        Assert.Equal(input.Phone, createdClient.Phone);
        Assert.Equal(input.CompanyId, createdClient.CompanyId);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenNameIsEmpty()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CreateClient sut = CreateSut(context, user);

        Client client = ClientMock.Create();
        ClientInput input = client.Adapt<ClientInput>();
        input.FullName = string.Empty;

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenEmailIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CreateClient sut = CreateSut(context, user);

        Client client = ClientMock.Create();
        ClientInput input = client.Adapt<ClientInput>();
        input.Email = "invalid-email";

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserNotLinkedToCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CreateClient sut = CreateSut(context, user);

        Client client = ClientMock.Create();
        ClientInput input = client.Adapt<ClientInput>();

        // Cria um outro usuário vinculado à empresa, mas não o user original;
        User user2 = UserMock.Create();
        await Fixture.Save(context, user2);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = client.CompanyId,
            UserId = user2.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, input));
    }

    #region helper
    private static CreateClient CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        CreateClient createClient = new(context, checkIfUserIsLinkedCompanyUser);

        return createClient;
    }
    #endregion
}