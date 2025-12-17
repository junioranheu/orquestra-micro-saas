using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Quotes.GeneratePDF;
using Orquestra.Application.UseCases.Quotes.GetAllByCompanyId;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.PDF;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using QuestPDF.Infrastructure;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Quotes;

public sealed class GeneratePDFQuoteTests
{
    [Fact]
    public async Task Execute_ShouldGeneratePdf_WhenQuoteExistsAndUserIsLinked()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        Quote quote = new()
        {
            QuoteId = Guid.NewGuid(),
            Title = "Orçamento PDF Teste",
            CompanyId = company.CompanyId,
            Company = company,
            ClientId = client.ClientId,
            Client = client,
            ValidUntil = GetDate().AddDays(22),
            Status = true,
            Items =
            [
                new() { Title = "Item 1", Quantity = 2, UnitPrice = 50 },
                new() { Title = "Item 2", Quantity = 1, UnitPrice = 100 }
            ]
        };

        await Fixture.Save(context, quote);

        GeneratePDFQuote sut = CreateSut(context, user);

        // Act;
        (byte[] pdf, string _) = await sut.Execute(user.UserId, quote.QuoteId);

        // Assert;
        Assert.NotNull(pdf);
        Assert.True(pdf.Length > 0);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenQuoteDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        GeneratePDFQuote sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, Guid.NewGuid()));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserIsNotLinkedToCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        // Outro user vinculado, mas não o user autenticado;
        User user2 = UserMock.Create();
        await Fixture.Save(context, user2);

        Guid companyId = Guid.NewGuid();

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = companyId,
            UserId = user2.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        Quote quote = new()
        {
            QuoteId = Guid.NewGuid(),
            Title = "Orçamento PDF Teste",
            CompanyId = companyId,
            Status = true,
            Client = new Client { FullName = "Cliente Teste" },
            Items =
            [
                new() { Title = "Item 1", Quantity = 2, UnitPrice = 50 }
            ]
        };

        await Fixture.Save(context, quote);

        GeneratePDFQuote sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, quote.QuoteId));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenDateValidUntilIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        Quote quote = new()
        {
            QuoteId = Guid.NewGuid(),
            Title = "Orçamento PDF Teste",
            CompanyId = company.CompanyId,
            Company = company,
            ClientId = client.ClientId,
            Client = client,
            ValidUntil = GetDate().AddDays(-22),
            Status = true,
            Items =
            [
                new() { Title = "Item 1", Quantity = 2, UnitPrice = 50 },
                new() { Title = "Item 2", Quantity = 1, UnitPrice = 100 }
            ]
        };

        await Fixture.Save(context, quote);

        GeneratePDFQuote sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, quote.QuoteId));
    }

    #region helpers
    private static GeneratePDFQuote CreateSut(Context context, User user)
    {
        IHttpContextAccessor accessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getAll = new(context);
        CheckIfUserIsLinkedCompanyUser check = new(getAll, accessor);
        GetAllQuoteByCompanyId getAllQuote = new(context, check);

        QuestPDF.Settings.License = LicenseType.Community;
        IPDFService pdfService = new PDFService();

        GeneratePDFQuote generatePDFQuote = new(context, getAllQuote, check, pdfService);

        return generatePDFQuote;
    }
    #endregion
}