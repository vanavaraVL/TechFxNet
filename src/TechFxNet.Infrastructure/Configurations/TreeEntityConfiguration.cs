using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFxNet.Domain.Entities;

namespace TechFxNet.Infrastructure.Configurations;

internal class TreeEntityConfiguration: IEntityTypeConfiguration<TreeEntity>
{
    public void Configure(EntityTypeBuilder<TreeEntity> builder)
    {
        builder.HasKey(t => t.Id); 
        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.Property(t => t.TreeName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.HasMany(t => t.Nodes)
            .WithOne(n => n.Tree)
            .HasForeignKey(n => n.TreeId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
}