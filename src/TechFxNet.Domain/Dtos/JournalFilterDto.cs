namespace TechFxNet.Domain.Dtos;

public record JournalFilterDto
{
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
    public string Search { get; init; } = string.Empty;
}