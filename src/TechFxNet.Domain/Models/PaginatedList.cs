namespace TechFxNet.Domain.Models;

public class PaginatedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int Skip { get; }
    public long Count { get; }

    public PaginatedList(IReadOnlyCollection<T> items, long count, int skip)
    {
        Skip = skip;
        Count = count;
        Items = items;
    }
}