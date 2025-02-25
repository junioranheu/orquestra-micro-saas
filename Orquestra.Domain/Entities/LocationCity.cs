using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orquestra.Domain.Entities;

public sealed class LocationCity
{
    [Key]
    public int LocationCityId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int LocationStateId { get; set; }
    [ForeignKey(nameof(LocationStateId))]
    public LocationState? LocationStates { get; set; }

    public bool Status { get; set; } = true;
}