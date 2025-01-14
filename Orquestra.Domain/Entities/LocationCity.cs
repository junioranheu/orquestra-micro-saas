using System.ComponentModel.DataAnnotations;

namespace Orquestra.Domain.Entities;

public sealed class LocationCity
{
    [Key]
    public int LocationCityId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int LocationStateId { get; set; }
    public LocationState? LocationStates { get; set; }

    public bool Status { get; set; } = true;
}