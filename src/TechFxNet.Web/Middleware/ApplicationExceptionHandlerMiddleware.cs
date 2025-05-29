using System.Text.Json;
using MediatR;
using TechFxNet.Application.Commands;
using TechFxNet.Domain.Exceptions;

namespace TechFxNet.Web.Middleware;

/// <summary>
/// Journal middleware
/// </summary>
public class ApplicationExceptionHandlerMiddleware
{
    private readonly ILogger<ApplicationExceptionHandlerMiddleware> _logger;
    private readonly RequestDelegate _nextDelegate;
    private readonly IServiceScopeFactory _scopeFactory;

    /// <summary>
    /// Journal middleware
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="nextDelegate"></param>
    /// <param name="scopeFactory"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ApplicationExceptionHandlerMiddleware(ILogger<ApplicationExceptionHandlerMiddleware> logger,
        RequestDelegate nextDelegate, IServiceScopeFactory scopeFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _nextDelegate = nextDelegate ?? throw new ArgumentNullException(nameof(nextDelegate));
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    }

    /// <summary>
    /// Invoke middleware
    /// </summary>
    /// <param name="context"></param>
    public virtual async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.Value != null && context.Request.Path.Value.Contains("swagger"))
        {
            await _nextDelegate.Invoke(context);
            return;
        }

        try
        {
            await _nextDelegate.Invoke(context);
        }
        catch (Exception ex) when (ex is NotFoundEntityException commandException)
        {
            var eventId = await SaveJournal(context, commandException);

            var errorInfo = JsonSerializer.Serialize(new { Type = $"{ExceptionTypeEnum.NotFound}", Id = eventId, Data = commandException.Message }, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            context.Response.StatusCode = StatusCodes.Status404NotFound;

            await context.Response.WriteAsync(errorInfo);
        }
        catch (Exception ex) when (ex is SecureException secureException)
        {
            var eventId = await SaveJournal(context, secureException);

            var errorInfo = JsonSerializer.Serialize(new { Type = $"{ExceptionTypeEnum.Secure}", Id = eventId, Data = secureException.Message }, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsync(errorInfo);
        }
        catch (Exception ex)
        {
            var eventId = await SaveJournal(context, ex);

            var errorInfo = JsonSerializer.Serialize(new { Type = $"{ExceptionTypeEnum.Exception}", Id = eventId, Data = $"Internal server error ID = {eventId}" }, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsync(errorInfo);
        }
    }

    private async Task<long> SaveJournal(HttpContext context, Exception ex)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            context.Request.EnableBuffering();

            return await mediator.Send(new AddJournalExceptionCommand(context, ex));
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Fatal exception: {e.Message}");

            throw;
        }
    }
}