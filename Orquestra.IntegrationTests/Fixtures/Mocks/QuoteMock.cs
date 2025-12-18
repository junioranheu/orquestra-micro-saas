using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Fixtures.Mocks;

public static class QuoteMock
{
    public static Quote Create(Guid createdBy)
    {
        var quoteId = Guid.NewGuid();

        return new Quote
        {
            QuoteId = quoteId,
            CompanyId = Guid.NewGuid(),
            Title = "Quote Teste",
            Observation = "Observação de teste",
            ValidUntil = GetDate().AddDays(10),
            QuoteStatus = QuoteStatusEnum.Draft,

            CreatedBy = createdBy,
            CreatedDate = GetDate(),
            LastModificationBy = null,
            LastModificationDate = null,

            Items =
            [
                new QuoteItem
                {
                    QuoteId = quoteId,
                    Title = "Item 1",
                    Quantity = 2,
                    UnitPrice = 50,

                    CreatedBy = createdBy,
                    CreatedDate = GetDate(),
                    LastModificationBy = null,
                    LastModificationDate = null
                },
                new QuoteItem
                {
                    QuoteId = quoteId,
                    Title = "Item 2",
                    Quantity = 1,
                    UnitPrice = 100,

                    CreatedBy = createdBy,
                    CreatedDate = GetDate(),
                    LastModificationBy = null,
                    LastModificationDate = null
                }
            ]
        };
    }
}