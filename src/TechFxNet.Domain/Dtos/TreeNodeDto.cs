namespace TechFxNet.Domain.Dtos;

public record TreeNodeDto
{
    public long Id { get; init; }

    public string Name { get; init; } = null!;

    public List<TreeNodeDto> Children { get; init; } = new List<TreeNodeDto>();
}