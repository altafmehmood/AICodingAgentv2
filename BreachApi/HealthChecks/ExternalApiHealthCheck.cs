using Microsoft.Extensions.Diagnostics.HealthChecks;
using BreachApi.Services;

namespace BreachApi.HealthChecks;

/// <summary>
/// Health check for external API connectivity
/// </summary>
public class ExternalApiHealthCheck : IHealthCheck
{
    private readonly IBreachApiService _breachApiService;
    private readonly ILogger<ExternalApiHealthCheck> _logger;

    public ExternalApiHealthCheck(IBreachApiService breachApiService, ILogger<ExternalApiHealthCheck> logger)
    {
        _breachApiService = breachApiService;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Performing health check for external API");
            
            // Try to call the external API with a timeout
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(10));
            
            var breaches = await _breachApiService.GetAllBreachesAsync(timeoutCts.Token);
            
            if (breaches?.Any() == true)
            {
                _logger.LogDebug("External API health check successful. Retrieved {BreachCount} breaches", breaches.Count);
                return HealthCheckResult.Healthy($"External API is responding. Retrieved {breaches.Count} breaches.");
            }
            
            _logger.LogWarning("External API returned no data");
            return HealthCheckResult.Degraded("External API is responding but returned no data.");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("External API health check was cancelled");
            return HealthCheckResult.Unhealthy("Health check was cancelled.");
        }
        catch (OperationCanceledException)
        {
            _logger.LogError("External API health check timed out");
            return HealthCheckResult.Unhealthy("External API health check timed out.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "External API health check failed");
            return HealthCheckResult.Unhealthy($"External API is not responding: {ex.Message}");
        }
    }
}