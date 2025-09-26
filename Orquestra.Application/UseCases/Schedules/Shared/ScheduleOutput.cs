using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Schedules.Shared;

public sealed class ScheduleOutput
{
    public Guid ScheduleId { get; set; }

    public DateTime DateStart { get; set; }

    public string TimeStart => DateStart.ToString("HH:mm");

    public DateTime DateEnd { get; set; }

    public string TimeEnd => DateEnd.ToString("HH:mm");

    public PaymentTypeEnum PaymentType { get; set; }

    public ScheduleStatusEnum ScheduleStatus { get; set; }

    public Guid ClientId { get; set; }
    public ClientOutput? Client { get; set; }

    public Guid CompanyId { get; set; }
    public CompanyOutput? Company { get; set; }

    public Guid[]? UsersIds { get; set; } = [];

    public bool IsRestrictForSpecificUsers { get; set; }

    public string? CustomTitle { get; set; }

    public string? CustomUrl { get; set; }

    public string? Observation { get; set; }

    public decimal? AmountReceived { get; set; } = 0;

    // Extras;
    public List<string>? Observations { get; set; }

    public UserOutput[]? UsersOutput { get; set; } = [];
}