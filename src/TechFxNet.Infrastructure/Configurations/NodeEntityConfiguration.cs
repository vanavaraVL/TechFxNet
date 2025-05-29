using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFxNet.Domain.Entities;

namespace TechFxNet.Infrastructure.Configurations;

internal class NodeEntityConfiguration: IEntityTypeConfiguration<NodeEntity>
{
    public void Configure(EntityTypeBuilder<NodeEntity> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).ValueGeneratedOnAdd();

        builder.HasIndex(n => new { n.ParentNodeId, n.NodeName })
            .IsUnique();

        builder.Property(n => n.NodeName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.HasOne(n => n.ParentNode)   
            .WithMany(n => n.ChildNodes) 
            .HasForeignKey(n => n.ParentNodeId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
    }
}