using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TechFxNet.Domain.Dtos;
using TechFxNet.Domain.Models;
using TechFxNet.Infrastructure.Repositories;

namespace TechFxNet.Application.Queries;

public record GetJournalRangeQuery(int Skip, int Take, JournalFilterDto? Filter) : IRequest<PaginatedList<JournalInfoDto>>;

public class GetJournalRangeQueryHandler : IRequestHandler<GetJournalRangeQuery, PaginatedList<JournalInfoDto>>
{
    private readonly IMapper _mapper;
    private readonly IJournalRepository _journalRepository;
    private readonly ILogger<GetJournalRangeQueryHandler> _logger;

    public GetJournalRangeQueryHandler(IMapper mapper, IJournalRepository journalRepository, ILogger<GetJournalRangeQueryHandler> logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _journalRepository = journalRepository ?? throw new ArgumentNullException(nameof(journalRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PaginatedList<JournalInfoDto>> Handle(GetJournalRangeQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Getting journal range: Skip={request.Skip}, Take={request.Take}, Search='{request?.Filter?.Search}'");

        var response = await _journalRepository.GetPagedResultAsync(request!.Take, request.Skip, request.Filter, cancellationToken);

        var (count, items) = (response.Count, response.Items);
        var modelList = _mapper.Map<List<JournalInfoDto>>(items);

        return new PaginatedList<JournalInfoDto>(modelList, count, request.Skip);
    }
}