using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechFxNet.Domain.Entities;

namespace TechFxNet.Infrastructure.Configurations;

internal class JournalEntityConfiguration : IEntityTypeConfiguration<JournalEntity>
{
    public void Configure(EntityTypeBuilder<JournalEntity> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.Property(t => t.Text);

        builder.Property(t => t.CreatedAt)
            .IsRequired();
    }
}