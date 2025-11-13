using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.ClientsFollowUps.Base;
using Orquestra.Application.UseCases.ClientsFollowUps.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.ClientsFollowUps;

public sealed class ClientFollowUpBaseTests
{
    [Fact]
    public async Task Validate_ShouldPass_WhenInputIsValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Client client = ClientMock.Create();
        client.CompanyId = company.CompanyId;
        await Fixture.Save(context, client);

        ClientFollowUpBase sut = CreateSut(context, user);

        ClientFollowUpInput input = new()
        {
            ClientId = client.ClientId,
            Observation = "Cliente retornará em breve.",
            ImagesFormFile = []
        };

        // Act & Assert;
        await sut.Validate(input, user.UserId);
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenClientNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        ClientFollowUpBase sut = CreateSut(context, user);

        ClientFollowUpInput input = new()
        {
            ClientId = Guid.NewGuid()
        };

        // Act & Assert;
        ArgumentException ex = await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId));
        Assert.Equal(SystemConsts.Warnings.NotFoundClient, ex.Message);
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenImageIsLargerThan3MB()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Client client = ClientMock.Create();
        client.CompanyId = company.CompanyId;
        await Fixture.Save(context, client);

        ClientFollowUpBase sut = CreateSut(context, user);

        byte[] largeImage = new byte[4 * 1024 * 1024];
        using MemoryStream stream = new(largeImage);

        FormFile bigFile = new(stream, 0, largeImage.Length, "file", "large.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };

        ClientFollowUpInput input = new()
        {
            ClientId = client.ClientId,
            ImagesFormFile = [bigFile]
        };

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Validate(input, user.UserId));
    }

    #region helper
    private static ClientFollowUpBase CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        ClientFollowUpBase clientFollowUpBase = new(context, checkIfUserIsLinkedCompanyUser);

        return clientFollowUpBase;
    }
    #endregion
}
