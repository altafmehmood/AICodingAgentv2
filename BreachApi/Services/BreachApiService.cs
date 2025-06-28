using BreachApi.Models;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System.Text.Json;

namespace BreachApi.Services;

/// <summary>
/// Service for calling external breach API with resilience patterns
/// </summary>
public class BreachApiService : IBreachApiService
{
    private const string HaveIBeenPwnedApiUrl = "https://haveibeenpwned.com/api/v3/breaches";
    private readonly HttpClient _httpClient;
    private readonly ILogger<BreachApiService> _logger;
    private readonly ResiliencePipeline<HttpResponseMessage> _resiliencePipeline;

    public BreachApiService(HttpClient httpClient, ILogger<BreachApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "BreachApi-1.0");
        
        // Configure resilience pipeline
        _resiliencePipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new Polly.Retry.RetryStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>().Handle<HttpRequestException>()
                    .Handle<TaskCanceledException>()
                    .HandleResult(response => !response.IsSuccessStatusCode),
                Delay = TimeSpan.FromSeconds(1),
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                OnRetry = args =>
                {
                    _logger.LogWarning("Retry attempt {AttemptNumber} for external API call. Delay: {Delay}ms", 
                        args.AttemptNumber, args.RetryDelay.TotalMilliseconds);
                    return ValueTask.CompletedTask;
                }
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(response => !response.IsSuccessStatusCode),
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(10),
                MinimumThroughput = 3,
                BreakDuration = TimeSpan.FromSeconds(30),
                OnOpened = args =>
                {
                    _logger.LogError("Circuit breaker opened for external API calls. Break duration: {BreakDuration}s", 
                        args.BreakDuration.TotalSeconds);
                    return ValueTask.CompletedTask;
                },
                OnClosed = args =>
                {
                    _logger.LogInformation("Circuit breaker closed. External API calls resumed");
                    return ValueTask.CompletedTask;
                },
                OnHalfOpened = args =>
                {
                    _logger.LogInformation("Circuit breaker half-opened. Testing external API");
                    return ValueTask.CompletedTask;
                }
            })
            .AddTimeout(TimeSpan.FromSeconds(30))
            .Build();
    }

    /// <summary>
    /// Gets all breaches from the external API with retry and circuit breaker policies
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of breaches</returns>
    public async Task<List<Breach>> GetAllBreachesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Calling HaveIBeenPwned API at {ApiUrl}", HaveIBeenPwnedApiUrl);

        try
        {
            var response = await _resiliencePipeline.ExecuteAsync(async (ct) =>
            {
                return await _httpClient.GetAsync(HaveIBeenPwnedApiUrl, ct);
            }, cancellationToken);

            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var breaches = JsonSerializer.Deserialize<List<Breach>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }) ?? new List<Breach>();

            _logger.LogInformation("Successfully retrieved {BreachCount} breaches from HaveIBeenPwned API", 
                breaches.Count);

            return breaches;
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError(ex, "Circuit breaker is open. External API is currently unavailable");
            throw new InvalidOperationException("External breach API is currently unavailable. Please try again later.", ex);
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError(ex, "Timeout occurred while calling external API");
            throw new InvalidOperationException("Request to external breach API timed out. Please try again later.", ex);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while calling HaveIBeenPwned API");
            throw new InvalidOperationException($"Failed to retrieve breaches from external API: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing API response");
            throw new InvalidOperationException("Invalid response format from external API", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while retrieving breaches");
            throw;
        }
    }
}