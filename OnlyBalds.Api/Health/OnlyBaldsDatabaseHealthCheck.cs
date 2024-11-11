using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OnlyBalds.Api.Data;

namespace OnlyBalds.Api.Health;

internal sealed class OnlyBaldsDatabaseHealthCheck(
    OnlyBaldsDataContext _dbContext, 
    ILogger<OnlyBaldsDatabaseHealthCheck> _logger) : IHealthCheck
{
    /// <summary>
    /// The database connection to check.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try 
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(cancellationToken);

            _logger.LogDebug("Determing if the database connection is a PostgreSQL connection.");
            var isNpgsql = _dbContext.Database.IsNpgsql();

            _logger.LogDebug("Opening the database connection.");
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);
            
            var anyThreads = await _dbContext.ThreadItems.AnyAsync();
            _logger.LogDebug($"Any Threads: {anyThreads}");
            var anyPosts = await _dbContext.PostItems.AnyAsync();
            _logger.LogDebug($"Any Posts: {anyPosts}");
            var anyComments = await _dbContext.CommentItems.AnyAsync();
            _logger.LogDebug($"Any Comments: {anyComments}");

            if (anyThreads is false && anyPosts is false && anyComments is false)
            {
                _logger.LogWarning("No data is available in the database.");
                return HealthCheckResult.Degraded();
            }
            
            if (isNpgsql && canConnect)
            {
                _logger.LogInformation("The database connection is healthy.");
                return HealthCheckResult.Healthy();
            }

            _logger.LogWarning("The database connection is not healthy.");
            return HealthCheckResult.Unhealthy();
        }
        catch
        {
            _logger.LogError("An error occurred while checking the database connection health.");
            return HealthCheckResult.Unhealthy();
        }
    }
}
