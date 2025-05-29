using System.ComponentModel.DataAnnotations;

namespace TechFxNet.Domain.Entities;

public class NodeEntity
{
    public NodeEntity()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public long Id { get; set; }

    [Required] 
    [MaxLength(255)] 
    public string NodeName { get; set; } = null!;

    public long TreeId { get; set; }

    public virtual TreeEntity Tree { get; set; } = null!;

    public long? ParentNodeId { get; set; }

    public virtual NodeEntity? ParentNode { get; set; }

    public virtual ICollection<NodeEntity> ChildNodes { get; set; } = new List<NodeEntity>();

    [Required]
    public DateTime CreatedAt { get; set; }
}