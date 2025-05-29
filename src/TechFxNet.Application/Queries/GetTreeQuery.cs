using MediatR;
using AutoMapper;
using TechFxNet.Domain.Dtos;
using TechFxNet.Infrastructure.Repositories;
using TechFxNet.Domain.Entities;

namespace TechFxNet.Application.Queries;

public record GetTreeQuery(string Name) : IRequest<TreeNodeDto>;

public class GetTreeQueryHandler : IRequestHandler<GetTreeQuery, TreeNodeDto>
{
    private readonly IMapper _mapper;
    private readonly ITreeNodeRepository _treeNodeRepository;

    public GetTreeQueryHandler(IMapper mapper, ITreeNodeRepository treeNodeRepository)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _treeNodeRepository = treeNodeRepository ?? throw new ArgumentNullException(nameof(treeNodeRepository));
    }

    public async Task<TreeNodeDto> Handle(GetTreeQuery request, CancellationToken cancellationToken)
    {
        var treeEntity = await _treeNodeRepository.GetTreeByName(request.Name, cancellationToken) ?? await _treeNodeRepository.CreateTree(request.Name, cancellationToken);
        var treeDto = _mapper.Map<TreeNodeDto>(treeEntity);

        var flatNodes = await _treeNodeRepository.GetTreeNodes(treeEntity.Id, cancellationToken);

        if (!flatNodes.Any(n => n.ParentNodeId is null))
        {
            return treeDto;
        }

        var nodeLookup = flatNodes.ToDictionary(k => k.Id, v => v);

        foreach (var node in flatNodes)
        {
            if (node.ParentNodeId.HasValue)
            {
                if (nodeLookup.TryGetValue(node.ParentNodeId.Value, out var parentNode))
                {
                    parentNode.ChildNodes.Add(node);
                }
            }
        }


        var childNodes = new List<TreeNodeDto>();
        var rootNodesInTree = flatNodes.Where(n => n.ParentNodeId is null).ToList();

        foreach (var rootEntity in rootNodesInTree)
        {
            childNodes.Add(MapNodeToDto(rootEntity));
        }

        return treeDto with { Children = childNodes };
    }

    private TreeNodeDto MapNodeToDto(NodeEntity entityNode)
    {
        var dto = new TreeNodeDto
        {
            Id = entityNode.Id,
            Name = entityNode.NodeName
        };

        if (entityNode.ChildNodes.Any())
        {
            foreach (var childEntity in entityNode.ChildNodes.OrderBy(c => c.Id))
            {
                dto.Children.Add(MapNodeToDto(childEntity));
            }
        }

        return dto;
    }
}