using Microsoft.Extensions.DependencyInjection;
using Orquestra.Application.UseCases.Quotes.Create;
using Orquestra.Application.UseCases.Quotes.Delete;
using Orquestra.Application.UseCases.Quotes.GetAllByCompanyId;
using Orquestra.Application.UseCases.Quotes.Update;

namespace Orquestra.Application.UseCases.Quotes;

public static class DependencyInjection
{
    public static IServiceCollection AddQuotesApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetAllQuoteByCompanyId, GetAllQuoteByCompanyId>();
        services.AddScoped<ICreateQuote, CreateQuote>();
        services.AddScoped<IUpdateQuote, UpdateQuote>();
        services.AddScoped<IDeleteQuote, DeleteQuote>();

        return services;
    }
}