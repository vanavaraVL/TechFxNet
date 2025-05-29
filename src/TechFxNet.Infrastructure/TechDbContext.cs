using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TechFxNet.Domain.Entities;

namespace TechFxNet.Infrastructure;

public class TechDbContext: DbContext
{
    public TechDbContext(DbContextOptions<TechDbContext> options) : base(options)
    {
    }

    public DbSet<TreeEntity> Trees { get; set; }

    public DbSet<NodeEntity> Nodes { get; set; }

    public DbSet<JournalEntity> Journals { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}