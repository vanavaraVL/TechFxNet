namespace TechFxNet.Domain.Models;

public record PaginatedResult<T>(IReadOnlyCollection<T> Items, long Count);