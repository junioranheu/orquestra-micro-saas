using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Schedules.Shared;

public sealed class ScheduleOutput
{
    public Guid ScheduleId { get; set; }

    public DateTime Date { get; set; }

    public PaymentTypeEnum PaymentType { get; set; }

    public ScheduleStatusEnum ScheduleStatus { get; set; }

    public Guid ClientId { get; set; }
    public ClientOutput? Clients { get; set; }

    public Guid CompanyId { get; set; }
    public CompanyOutput? Company { get; set; }

    // Extras;
    public List<string>? Observations { get; set; }
}