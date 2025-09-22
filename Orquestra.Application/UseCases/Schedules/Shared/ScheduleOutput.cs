using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Schedules.Shared;

public sealed class ScheduleOutput
{
    public Guid ScheduleId { get; set; }

    public DateTime Date { get; set; }

    public int DurationMinutes { get; set; }

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

    // Extras;
    public DateTime DateEnd { get; set; }

    public List<string>? Observations { get; set; }

    public UserOutput[]? UsersOutput { get; set; } = [];
}