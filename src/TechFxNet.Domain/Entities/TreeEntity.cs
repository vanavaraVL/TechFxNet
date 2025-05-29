using System.ComponentModel.DataAnnotations;

namespace TechFxNet.Domain.Entities;

public class TreeEntity
{
    public TreeEntity()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public long Id { get; set; }

    [Required] 
    [MaxLength(255)] 
    public string TreeName { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<NodeEntity> Nodes { get; set; } = new List<NodeEntity>();
}