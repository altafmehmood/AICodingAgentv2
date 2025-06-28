namespace BreachApi.Models;

/// <summary>
/// Represents a data breach from HaveIBeenPwned API
/// </summary>
public class Breach
{
    /// <summary>
    /// The name of the breach
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The title of the breach
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// The domain of the breached service
    /// </summary>
    public string Domain { get; set; } = string.Empty;
    
    /// <summary>
    /// The date when the breach was added to the database
    /// </summary>
    public DateTime AddedDate { get; set; }
    
    /// <summary>
    /// The date when the breach occurred
    /// </summary>
    public DateTime? BreachDate { get; set; }
    
    /// <summary>
    /// The date when the breach was modified
    /// </summary>
    public DateTime? ModifiedDate { get; set; }
    
    /// <summary>
    /// The number of accounts affected by the breach
    /// </summary>
    public int PwnCount { get; set; }
    
    /// <summary>
    /// A description of the breach
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// The data classes that were compromised
    /// </summary>
    public List<string> DataClasses { get; set; } = new();
    
    /// <summary>
    /// Whether the breach is verified
    /// </summary>
    public bool IsVerified { get; set; }
    
    /// <summary>
    /// Whether the breach is fabricated
    /// </summary>
    public bool IsFabricated { get; set; }
    
    /// <summary>
    /// Whether the breach is sensitive
    /// </summary>
    public bool IsSensitive { get; set; }
    
    /// <summary>
    /// Whether the breach is retired
    /// </summary>
    public bool IsRetired { get; set; }
    
    /// <summary>
    /// Whether the breach is spam list
    /// </summary>
    public bool IsSpamList { get; set; }
} 