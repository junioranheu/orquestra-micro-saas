using Orquestra.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Domain.Entities;

public sealed class CompanyInvoice : Audit
{
    public CompanyInvoice()
    {
        InvoiceNumber = GuidToNumericId(CompanyInvoiceId);
    }

    [Key]
    public Guid CompanyInvoiceId { get; set; }

    public long InvoiceNumber { get; private set; }

    public Guid CompanyId { get; set; }
    [JsonIgnore]
    [ForeignKey(nameof(CompanyId))]
    public Company? Company { get; set; }

    public required ModuleEnum[] Modules { get; set; } = [];

    public decimal Amount { get; set; } = 0;

    [MaxLength(500)]
    public string? Description { get; set; } = string.Empty;

    public CompanyInvoiceSituationEnum CompanyInvoiceSituation { get; set; } = CompanyInvoiceSituationEnum.Pending;
}