using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using TechFxNet.Domain.Entities;
using TechFxNet.Infrastructure.Repositories;

namespace TechFxNet.Application.Commands;

public record AddJournalExceptionCommand(HttpContext Context, Exception Exception) : IRequest<long>;

public class AddJournalExceptionCommandHandler : IRequestHandler<AddJournalExceptionCommand, long>
{
    private readonly ILogger<AddJournalExceptionCommandHandler> _logger;
    private readonly Random _random = new(Environment.TickCount);
    private readonly IJournalRepository _journalRepository;

    public AddJournalExceptionCommandHandler(ILogger<AddJournalExceptionCommandHandler> logger, IJournalRepository journalRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _journalRepository = journalRepository ?? throw new ArgumentNullException(nameof(journalRepository)); ;
    }
    public async Task<long> Handle(AddJournalExceptionCommand request, CancellationToken cancellationToken)
    {
        var context = request.Context;
        var ex = request.Exception;

        var requestBody = await ReadRequestBodyAsync(context.Request);
        var queryParameters = context.Request.Query;
        var headers = context.Request.Headers.ToDictionary(header => header.Key, header => header.Value.ToString());

        var requestData = new
        {
            context.Request.Method,
            QueryString = context.Request.QueryString.Value,
            QueryParameters = queryParameters.ToDictionary(q => q.Key, q => q.Value.ToString()),
            Headers = headers,
            RequestBody = requestBody,
            ExceptionMessage = ex.Message,
            InnerExceptionMessage = ex.InnerException?.Message
        };

        var requestObj = JsonSerializer.Serialize(requestData, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        var journalEntry = new JournalEntity()
        {
            EventId = _random.NextInt64(),
            Text = requestObj
        };

        await _journalRepository.SaveEntity(journalEntry, cancellationToken);

        return journalEntry.EventId;
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        using var reader = new StreamReader(request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        return body;
    }
}