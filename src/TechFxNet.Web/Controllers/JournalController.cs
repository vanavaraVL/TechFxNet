using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TechFxNet.Application.Queries;
using TechFxNet.Domain.Dtos;
using TechFxNet.Domain.Models;

namespace TechFxNet.Web.Controllers;

/// <inheritdoc />
[ApiController]
[Route("api.user.journal")]
[Produces("application/json")]
public class JournalController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <inheritdoc />
    public JournalController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Provides the pagination API for journal entries.
    /// Skip means the number of items should be skipped by server.
    /// Take means the maximum number items should be returned by server.
    /// All fields of the filter are optional.
    /// </summary>
    /// <param name="skip">The number of items to skip.</param>
    /// <param name="take">The maximum number of items to return.</param>
    /// <param name="filter">Optional filter criteria for journal entries.</param>
    /// <returns>A paginated list of journal info.</returns>
    [HttpPost("getRange")]
    [SwaggerOperation(Tags = new[] { "user.journal" })]
    [ProducesResponseType(typeof(PaginatedList<JournalInfoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRange([FromQuery] int skip, [FromQuery] int take, [FromBody] JournalFilterDto? filter, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetJournalRangeQuery(skip, take, filter), ct);

        return Ok(result);
    }

    /// <summary>
    /// Returns the information about a particular event by ID.
    /// </summary>
    /// <param name="id">The ID of the journal event.</param>
    /// <returns>Information about the journal event.</returns>
    [HttpPost("getSingle")]
    [SwaggerOperation(Tags = new[] { "user.journal" })]
    [ProducesResponseType(typeof(JournalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSingle([FromQuery] long id, CancellationToken ct)
    {
        var journal = await _mediator.Send(new GetJournalSingleQuery(id), ct);

        if (journal is null)
        {
            return NotFound($"Journal with ID {id} not found.");
        }

        return Ok(journal);
    }
}