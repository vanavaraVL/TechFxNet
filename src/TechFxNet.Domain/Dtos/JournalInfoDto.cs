namespace TechFxNet.Domain.Dtos;

public record JournalInfoDto
{
    public long Id { get; init; } 
    public long EventId { get; init; }
    public DateTime CreatedAt { get; init; }
}