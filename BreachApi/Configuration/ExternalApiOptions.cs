namespace BreachApi.Configuration;

/// <summary>
/// Configuration options for external API settings
/// </summary>
public class ExternalApiOptions
{
    public const string SectionName = "ExternalApi";
    
    public HaveIBeenPwnedOptions HaveIBeenPwned { get; set; } = new();
}

/// <summary>
/// Configuration options for HaveIBeenPwned API
/// </summary>
public class HaveIBeenPwnedOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public string UserAgent { get; set; } = string.Empty;
}