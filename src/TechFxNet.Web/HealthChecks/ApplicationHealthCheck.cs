using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TechFxNet.Infrastructure;

namespace TechFxNet.Web.HealthChecks;

/// <inheritdoc />
public class ApplicationHealthCheck: IHealthCheck
{
    private readonly ILogger<ApplicationHealthCheck> _logger;
    private readonly TechDbContext _context;

    /// <summary>
    /// Application health check
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="context"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ApplicationHealthCheck(ILogger<ApplicationHealthCheck> logger, TechDbContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        _logger.LogInformation("Attempting to connect to application db");

        try
        {
            _logger.LogInformation("Get tree record");
            var _ = await _context.Trees.AsNoTracking().Take(1).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Get trees records: {ex.Message}", ex);
        }

        return HealthCheckResult.Healthy("Application database online");
    }
}