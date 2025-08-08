namespace BreachApi.Configuration;

/// <summary>
/// Configuration options for caching settings
/// </summary>
public class CachingOptions
{
    public const string SectionName = "Caching";
    
    public int DefaultExpirationMinutes { get; set; } = 30;
    public int SlidingExpirationMinutes { get; set; } = 10;
}