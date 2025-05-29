using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TechFxNet.Application.Commands;

namespace TechFxNet.Web.Controllers;

/// <inheritdoc />
[ApiController]
[Route("api.user.tree.node")] // Base route segment
[Produces("application/json")]
public class TreeNodeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TreeNodeController> _logger;

    /// <inheritdoc />
    public TreeNodeController(ILogger<TreeNodeController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new node in your tree. You must specify a parent node ID that belongs to your tree.
    /// A new node name must be unique across all siblings.
    /// </summary>
    /// <param name="treeName">The name of the tree.</param>
    /// <param name="parentNodeId">The ID of the parent node.</param>
    /// <param name="nodeName">The name of the new node.</param>
    /// <returns>A successful response if the node is created.</returns>
    [HttpPost("create")]
    [SwaggerOperation(Tags = new[] { "user.tree.node" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromQuery] string treeName, [FromQuery] long parentNodeId, [FromQuery] string nodeName, CancellationToken ct)
    {
        await _mediator.Send(new CreateNodeCommand(nodeName, treeName, parentNodeId), ct);

        return Ok();
    }

    /// <summary>
    /// Delete an existing node in your tree. You must specify a node ID that belongs your tree.
    /// </summary>
    /// <param name="treeName">The name of the tree.</param>
    /// <param name="nodeId">The ID of the node to delete.</param>
    /// <returns>A successful response if the node is deleted.</returns>
    [HttpPost("delete")]
    [SwaggerOperation(Tags = new[] { "user.tree.node" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromQuery] string treeName, [FromQuery] long nodeId, CancellationToken ct)
    {
        await _mediator.Send(new DeleteNodeCommand(treeName, nodeId), ct);

        return Ok();
    }

    /// <summary>
    /// Rename an existing node in your tree. You must specify a node ID that belongs your tree.
    /// A new name of the node must be unique across all siblings.
    /// </summary>
    /// <param name="treeName">The name of the tree.</param>
    /// <param name="nodeId">The ID of the node to rename.</param>
    /// <param name="newNodeName">The new name for the node.</param>
    /// <returns>A successful response if the node is renamed.</returns>
    [HttpPost("rename")]
    [SwaggerOperation(Tags = new[] { "user.tree.node" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Rename([FromQuery] string treeName, [FromQuery] long nodeId, [FromQuery] string newNodeName, CancellationToken ct)
    {
        await _mediator.Send(new RenameNodeCommand(treeName, nodeId, newNodeName), ct);

        return Ok();
    }
}