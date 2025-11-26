using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Quotes.Delete;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Quotes;

public sealed class DeleteQuoteTests
{
    [Fact]
    public async Task Execute_ShouldDeactivateQuoteAndItems_WhenQuoteExists()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Guid companyId = Guid.NewGuid();

        Quote quote = QuoteMock.Create(user.UserId);
        await Fixture.Save(context, quote);

        DeleteQuote sut = CreateSut(context, user);

        // Act;
        await sut.Execute(user.UserId, quote.QuoteId);

        // Assert;
        Quote? deleted = await context.Quotes.FindAsync(quote.QuoteId);
        Assert.NotNull(deleted);
        Assert.False(deleted.Status);

        List<QuoteItem> items = [.. context.QuoteItems.Where(x => x.QuoteId == quote.QuoteId)];
        Assert.All(items, x => Assert.False(x.Status));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenQuoteDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        DeleteQuote sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, Guid.NewGuid()));
    }

    #region helper
    private static DeleteQuote CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        DeleteQuote deleteQuote = new(context, checkIfUserIsLinkedCompanyUser);

        return deleteQuote;
    }
    #endregion
}