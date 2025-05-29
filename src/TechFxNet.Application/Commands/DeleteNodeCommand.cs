using MediatR;
using Microsoft.Extensions.Logging;
using TechFxNet.Domain.Exceptions;
using TechFxNet.Infrastructure.Repositories;

namespace TechFxNet.Application.Commands;

public record DeleteNodeCommand(string TreeName, long NodeId) : IRequest<bool>;

public class DeleteNodeCommandHandler : IRequestHandler<DeleteNodeCommand, bool>
{
    private readonly ILogger<DeleteNodeCommandHandler> _logger;
    private readonly ITreeNodeRepository _treeNodeRepository;

    public DeleteNodeCommandHandler(ILogger<DeleteNodeCommandHandler> logger,
        ITreeNodeRepository treeNodeRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _treeNodeRepository = treeNodeRepository ?? throw new ArgumentNullException(nameof(treeNodeRepository));
    }

    public async Task<bool> Handle(DeleteNodeCommand request, CancellationToken cancellationToken)
    {
        var treeEntity = await _treeNodeRepository.GetTreeByName(request.TreeName, cancellationToken);

        if (treeEntity is null)
        {
            throw new NotFoundEntityException($"Tree {request.TreeName} not found");
        }

        var nodeToDelete = await _treeNodeRepository.GetNodeById(treeEntity.Id, request.NodeId, cancellationToken);

        if (nodeToDelete is null)
        {
            throw new NotFoundEntityException($"Node {request.NodeId} not found");
        }

        await _treeNodeRepository.DeleteNode(nodeToDelete, cancellationToken);

        return true;
    }
}