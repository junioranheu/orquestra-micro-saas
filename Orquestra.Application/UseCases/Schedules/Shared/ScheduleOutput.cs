using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Schedules.Shared;

public sealed class ScheduleOutput
{
    public Guid ScheduleId { get; set; }

    public DateTime Date { get; set; }

    public PaymentTypeEnum PaymentType { get; set; }

    public ScheduleStatusEnum ScheduleStatus { get; set; }

    public Guid ClientId { get; set; }
    public ClientOutput? Client { get; set; }

    public Guid CompanyId { get; set; }
    public CompanyOutput? Company { get; set; }

    public Guid[]? Users { get; set; } = [];

    // Extras;
    public List<string>? Observations { get; set; }

    public UserOutput[]? UsersOutput { get; set; } = [];
}