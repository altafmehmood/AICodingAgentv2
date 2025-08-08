namespace BreachApi.Configuration;

/// <summary>
/// Configuration options for resilience patterns
/// </summary>
public class ResilienceOptions
{
    public const string SectionName = "Resilience";
    
    public CircuitBreakerOptions CircuitBreaker { get; set; } = new();
    public RetryOptions Retry { get; set; } = new();
}

/// <summary>
/// Configuration options for circuit breaker
/// </summary>
public class CircuitBreakerOptions
{
    public int HandledEventsAllowedBeforeBreaking { get; set; } = 3;
    public int BreakDurationSeconds { get; set; } = 30;
}

/// <summary>
/// Configuration options for retry policy
/// </summary>
public class RetryOptions
{
    public int MaxRetryAttempts { get; set; } = 3;
    public int DelaySeconds { get; set; } = 1;
}