using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.ServiceOrders.Shared;

public sealed class ServiceOrderOutput 
{
    public Guid ServiceOrderId { get; set; }

    public Guid CompanyId { get; set; }
    public Company? Company { get; set; }

    public Guid QuoteId { get; set; }
    public Quote? Quote { get; set; }

    public Guid ClientId { get; set; }
    public Client? Client { get; set; }

    public string? Title { get; set; }

    public string? Observation { get; set; }

    public DateTime? ExecutionDate { get; set; }

    public ServiceOrderStatusEnum ServiceOrderStatus { get; set; } 
}