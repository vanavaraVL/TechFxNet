using MediatR;
using Microsoft.Extensions.Logging;
using TechFxNet.Infrastructure.Repositories;
using TechFxNet.Domain.Exceptions;

namespace TechFxNet.Application.Commands;

public record CreateNodeCommand(string NodeName, string TreeName, long ParentNodeId) : IRequest<bool>;

public class CreateNodeCommandHandler : IRequestHandler<CreateNodeCommand, bool>
{
    private readonly ILogger<CreateNodeCommandHandler> _logger;
    private readonly ITreeNodeRepository _treeNodeRepository;

    public CreateNodeCommandHandler(ILogger<CreateNodeCommandHandler> logger,
        ITreeNodeRepository treeNodeRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _treeNodeRepository = treeNodeRepository ?? throw new ArgumentNullException(nameof(treeNodeRepository));
    }

    public async Task<bool> Handle(CreateNodeCommand request, CancellationToken cancellationToken)
    {
        var treeEntity = await _treeNodeRepository.GetTreeByName(request.TreeName, cancellationToken);

        if (treeEntity is null)
        {
            throw new NotFoundEntityException($"Tree {request.TreeName} not found");
        }

        var parentNode = await _treeNodeRepository.GetParentNode(treeEntity.Id, request.ParentNodeId, cancellationToken);

        if (parentNode is null)
        {
            throw new NotFoundEntityException($"Parent node {request.ParentNodeId} not found");
        }

        await _treeNodeRepository.CreateNode(treeEntity.Id, request.ParentNodeId, request.NodeName, cancellationToken);

        return true;
    }
}