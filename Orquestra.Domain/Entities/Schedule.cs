using Orquestra.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orquestra.Domain.Entities;

public sealed class Schedule : Audit
{
    [Key]
    public Guid ScheduleId { get; set; }

    public DateTime Date { get; set; }

    public PaymentTypeEnum PaymentType { get; set; }

    public ScheduleStatusEnum ScheduleStatus { get; set; }

    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    public Guid CompanyId { get; set; }
    [ForeignKey(nameof(CompanyId))]
    public Company? Company { get; set; }
}