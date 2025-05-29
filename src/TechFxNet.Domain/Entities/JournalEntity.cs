namespace TechFxNet.Domain.Entities;

public class JournalEntity
{
    public JournalEntity()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public long Id { get; init; }

    public string Text { get; set; } = null!;

    public long EventId { get; init; }

    public DateTime CreatedAt { get; init; }
}