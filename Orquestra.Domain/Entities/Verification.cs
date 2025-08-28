using Orquestra.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Orquestra.Domain.Entities;

public sealed class Verification : Audit
{
    [Key]
    public Guid VerificationId { get; set; }

    [MaxLength(44)] // Máx byte[32];
    public required string Token { get; set; } 

    [MaxLength(20)]
    public required string EntityType { get; set; }

    public required Guid EntityId { get; set; }
    public required VerificationTypeEnum VerificationType { get; set; }

    [Display(Name = "Informação adicional", Description = "Propriedade curinga para guardar e-mail, telefone ou outro identificador útil.")]
    [MaxLength(100)]
    public string? Reference { get; set; }

    public bool Used { get; set; } = false;
}