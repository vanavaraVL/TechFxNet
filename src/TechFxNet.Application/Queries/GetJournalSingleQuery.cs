using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TechFxNet.Domain.Dtos;
using TechFxNet.Infrastructure.Repositories;

namespace TechFxNet.Application.Queries;

public record GetJournalSingleQuery(long EventId) : IRequest<JournalDto?>;

public class GetJournalSingleQueryHandler : IRequestHandler<GetJournalSingleQuery, JournalDto?>
{
    private readonly IMapper _mapper;
    private readonly IJournalRepository _journalRepository;
    private readonly ILogger<GetJournalSingleQueryHandler> _logger;

    public GetJournalSingleQueryHandler(IMapper mapper, IJournalRepository journalRepository, ILogger<GetJournalSingleQueryHandler> logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _journalRepository = journalRepository ?? throw new ArgumentNullException(nameof(journalRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<JournalDto?> Handle(GetJournalSingleQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Getting journal single entity: EventId={request.EventId}'");

        var response = await _journalRepository.GetSingleByEventId(request.EventId, cancellationToken);

        return response is not null ? _mapper.Map<JournalDto>(response) : null;
    }
}