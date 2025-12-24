using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Quotes.Shared;

public sealed class QuoteOutput
{
    public Guid QuoteId { get; set; }

    public Guid CompanyId { get; set; }
    public CompanyOutput? Company { get; set; }

    public Guid ClientId { get; set; }
    public ClientOutput? Client { get; set; }

    public string? Title { get; set; }
    public string? Observation { get; set; }
    public DateTime? ValidUntil { get; set; }
    public QuoteStatusEnum QuoteStatus { get; set; }
    public List<QuoteItem> Items { get; set; } = [];

    public DateTime? CreatedDate { get; set; }
}