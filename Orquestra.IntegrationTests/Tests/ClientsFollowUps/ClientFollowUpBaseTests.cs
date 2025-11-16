using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.ClientsFollowUps.Base;
using Orquestra.Application.UseCases.ClientsFollowUps.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
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
            ImagesFormFile = [],
            ClientFollowUpStatus = ClientFollowUpStatusEnum.InProgress
        };

        // Act & Assert;
        Guid companyId = await sut.Validate(input, user.UserId, isCreate: true);
        Assert.True(companyId != Guid.Empty);
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
            ClientId = Guid.NewGuid(),
            ClientFollowUpStatus = ClientFollowUpStatusEnum.InProgress
        };

        // Act & Assert;
        ArgumentException ex = await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true));
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
            ImagesFormFile = [bigFile],
            ClientFollowUpStatus = ClientFollowUpStatusEnum.InProgress
        };

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenMoreThanMaxFilesUploaded()
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

        // cria 4 arquivos simulando mais do que o permitido;
        List<IFormFile> files = [.. Enumerable.Range(0, 4).Select(i =>
        {
            byte[] data = new byte[1024];
            MemoryStream ms = new(data);
            return new FormFile(ms, 0, data.Length, $"file{i}", $"file{i}.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        })];

        ClientFollowUpInput input = new()
        {
            ClientId = client.ClientId,
            ImagesFormFile = files,
            ClientFollowUpStatus = ClientFollowUpStatusEnum.InProgress
        };

        // Act & Assert;
        ArgumentException ex = await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true));
        Assert.Contains("Você pode subir no máximo", ex.Message);
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenStatusIsInvalidOnCreate()
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
            ClientFollowUpStatus = ClientFollowUpStatusEnum.Completed
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenStatusIsInvalidOnEdit()
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
            Observation = "editando...",
            ClientFollowUpStatus = ClientFollowUpStatusEnum.Completed
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: false));
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