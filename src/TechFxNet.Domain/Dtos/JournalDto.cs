namespace TechFxNet.Domain.Dtos;

public record JournalDto
{
    public string Text { get; init; } = null!;

    public long Id { get; init; }

    public long EventId { get; init; }

    public DateTime CreatedAt { get; init; }
}