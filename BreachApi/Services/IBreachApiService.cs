using BreachApi.Models;

namespace BreachApi.Services;

/// <summary>
/// Interface for breach API service with resilience patterns
/// </summary>
public interface IBreachApiService
{
    /// <summary>
    /// Gets all breaches from the external API with retry and circuit breaker policies
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of breaches</returns>
    Task<List<Breach>> GetAllBreachesAsync(CancellationToken cancellationToken = default);
}