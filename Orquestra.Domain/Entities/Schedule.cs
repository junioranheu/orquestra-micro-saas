using Microsoft.EntityFrameworkCore;
using Orquestra.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orquestra.Domain.Entities;

[Index(nameof(ClientId), nameof(CompanyId))]
public sealed class Schedule : Audit
{
    [Key]
    public Guid ScheduleId { get; set; }

    public DateTime Date { get; set; }

    public PaymentTypeEnum PaymentType { get; set; }

    public ScheduleStatusEnum ScheduleStatus { get; set; }

    public Guid ClientId { get; set; }
    [ForeignKey(nameof(ClientId))]
    public Client? Client { get; set; }

    public Guid CompanyId { get; set; }
    [ForeignKey(nameof(CompanyId))]
    public Company? Company { get; set; }

    public Guid[]? UsersIds { get; set; } = []; // Não altere a ordem entre UsersIds e IsRestrictForSpecificUsers;

    private bool _isRestrictForSpecificUsers = false;
    public bool IsRestrictForSpecificUsers
    {
        get => _isRestrictForSpecificUsers;
        set
        {
            if (value && (UsersIds is null || UsersIds.Length == 0))
            {
                throw new InvalidOperationException("Não é possível restringir o agendamento se não houver usuários definidos.");
            }

            _isRestrictForSpecificUsers = value;
        }
    }
}