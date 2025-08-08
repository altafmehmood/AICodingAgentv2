namespace BreachApi.Configuration;

/// <summary>
/// Configuration options for validation settings
/// </summary>
public class ValidationOptions
{
    public const string SectionName = "Validation";
    
    public int MinAllowedYear { get; set; } = 2007;
    public int MaxDateRangeDays { get; set; } = 3650;
}