namespace Orquestra.Domain.Entities;

public abstract class Audit
{
    public DateTime? CreatedDate { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? LastModificationDate { get; set; }

    public Guid? LastModificationBy { get; set; }

    public bool Status { get; set; } 
}