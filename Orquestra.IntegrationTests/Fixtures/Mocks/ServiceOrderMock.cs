using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Fixtures.Mocks;

public static class ServiceOrderMock
{
    public static ServiceOrder Create(Guid companyId, string? title = "")
    {
        if (string.IsNullOrEmpty(title))
        {
            title = GetRandomString(charLength: GetRandomNumber(5, 30));
        }

        Quote quote = new()
        {
            Title = "Orçamento Teste",
            Observation = "Alguma observação válida",
            CompanyId = companyId,
            Items =
            [
               new QuoteItem
               {
                  Title = "Item 1",
                  Quantity = 2,
                  UnitPrice = 50
               },
               new QuoteItem
               {
                  Title = "Item 2",
                  Quantity = 1,
                  UnitPrice = 100
               }
            ]
        };

        Client client = ClientMock.Create();

        ServiceOrder input = new()
        {
            CompanyId = companyId,
            QuoteId = quote.QuoteId,
            Quote = quote,
            ClientId = client.ClientId,
            Client = client,
            Title = title,
            Observation = GetRandomString(charLength: GetRandomNumber(30, 50)),
            ExecutionDate = GetDate().AddDays(22),
            ServiceOrderStatus = ServiceOrderStatusEnum.Pending
        };

        return input;
    }
}