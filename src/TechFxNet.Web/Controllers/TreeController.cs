using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TechFxNet.Application.Queries;
using TechFxNet.Domain.Dtos;

namespace TechFxNet.Web.Controllers;

/// <inheritdoc />
[ApiController]
[Route("api.user.tree")]
[Produces("application/json")]
public class TreeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TreeController> _logger;

    /// <inheritdoc />
    public TreeController(ILogger<TreeController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Returns your entire tree. If your tree doesn't exist it will be created automatically.
    /// </summary>
    /// <param name="treeName">The name of the tree.</param>
    /// <returns>The root node of the tree with its children, or a response indicating creation.</returns>
    [HttpPost("get")]
    [SwaggerOperation(Tags = new[] { "user.tree" })]
    [ProducesResponseType(typeof(TreeNodeDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] string treeName, CancellationToken ct)
    {
        var treeDto = await _mediator.Send(new GetTreeQuery(treeName), ct);

        return Ok(treeDto);
    }
}