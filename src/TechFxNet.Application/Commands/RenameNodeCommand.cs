using MediatR;
using Microsoft.Extensions.Logging;
using TechFxNet.Domain.Exceptions;
using TechFxNet.Infrastructure.Repositories;

namespace TechFxNet.Application.Commands;

public record RenameNodeCommand(string TreeName, long NodeId, string NewNodeName) : IRequest<bool>;

public class RenameNodeCommandHandler : IRequestHandler<RenameNodeCommand, bool>
{
    private readonly ILogger<RenameNodeCommandHandler> _logger;
    private readonly ITreeNodeRepository _treeNodeRepository;

    public RenameNodeCommandHandler(ILogger<RenameNodeCommandHandler> logger,
        ITreeNodeRepository treeNodeRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _treeNodeRepository = treeNodeRepository ?? throw new ArgumentNullException(nameof(treeNodeRepository));
    }

    public async Task<bool> Handle(RenameNodeCommand request, CancellationToken cancellationToken)
    {
        var treeEntity = await _treeNodeRepository.GetTreeByName(request.TreeName, cancellationToken);

        if (treeEntity is null)
        {
            throw new NotFoundEntityException($"Tree {request.TreeName} not found");
        }

        var nodeToRename = await _treeNodeRepository.GetNodeById(treeEntity.Id, request.NodeId, cancellationToken);

        if (nodeToRename is null)
        {
            throw new NotFoundEntityException($"Node {request.NodeId} not found");
        }

        await _treeNodeRepository.RenameNode(nodeToRename.Id, request.NewNodeName, cancellationToken);

        return true;
    }
}