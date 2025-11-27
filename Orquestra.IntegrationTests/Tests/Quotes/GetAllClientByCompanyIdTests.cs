using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Quotes.GetAllByCompanyId;
using Orquestra.Application.UseCases.Quotes.Shared;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Quotes;

public sealed class GetAllQuoteByCompanyIdTests
{
    [Fact]
    public async Task Execute_ShouldReturnQuotes_WhenFiltersMatch()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        // Cria algumas quotes manualmente;
        Quote quote1 = new()
        {
            QuoteId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            Company = company,
            ClientId = client.ClientId,
            Client = client,
            Title = "Teste 1",
            Observation = "Observação 1",
            Status = true,
            CreatedBy = user.UserId,
            CreatedDate = GetDate().AddDays(-2),
            LastModificationDate = GetDate().AddDays(-1),
            Items = []
        };

        Quote quote2 = new()
        {
            QuoteId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            Company = company,
            ClientId = client.ClientId,
            Client = client,
            Title = "Teste 2",
            Observation = "Observação 2",
            Status = true,
            CreatedBy = user.UserId,
            CreatedDate = GetDate().AddDays(-3),
            LastModificationDate = GetDate(),
            Items = []
        };

        await context.Quotes.AddRangeAsync(quote1, quote2);
        await context.SaveChangesAsync();

        GetAllQuoteByCompanyId sut = CreateSut(context, user);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };
        QuoteInput filter = new() { Title = "Teste", Observation = "Observação" };

        // Act;
        (IEnumerable<QuoteOutput> output, int count) = await sut.Execute(pagination, filter, user.UserId, company.CompanyId);

        // Assert;
        Assert.Equal(2, count);
        Assert.Contains(output, q => q.Title == "Teste 1");
        Assert.Contains(output, q => q.Title == "Teste 2");
    }

    [Fact]
    public async Task Execute_ShouldReturnEmpty_WhenNoMatch()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Guid companyId = Guid.NewGuid();

        GetAllQuoteByCompanyId sut = CreateSut(context, user);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };
        QuoteInput filter = new() { Title = "Não existe", Observation = "Nada" };

        // Act;
        (IEnumerable<QuoteOutput> output, int count) = await sut.Execute(pagination, filter, user.UserId, companyId);

        // Assert;
        Assert.Empty(output);
        Assert.Equal(0, count);
    }

    #region helpers
    private static GetAllQuoteByCompanyId CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        return new GetAllQuoteByCompanyId(context, checkIfUserIsLinkedCompanyUser);
    }
    #endregion
}