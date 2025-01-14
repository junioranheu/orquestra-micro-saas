using System.ComponentModel.DataAnnotations;

namespace Orquestra.Domain.Entities;

public sealed class LocationState
{
    [Key]
    public int LocationStateId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Abbreviation { get; set; } = string.Empty;

    public bool Status { get; set; } = true;
}