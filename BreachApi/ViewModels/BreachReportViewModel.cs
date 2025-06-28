using BreachApi.Models;

namespace BreachApi.ViewModels;

/// <summary>
/// View model for breach report PDF generation
/// </summary>
public class BreachReportViewModel
{
    /// <summary>
    /// List of breaches to include in the report
    /// </summary>
    public List<Breach> Breaches { get; set; } = new();
    
    /// <summary>
    /// Optional start date filter
    /// </summary>
    public DateTime? FromDate { get; set; }
    
    /// <summary>
    /// Optional end date filter
    /// </summary>
    public DateTime? ToDate { get; set; }
    
    /// <summary>
    /// Date when the report was generated
    /// </summary>
    public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Total number of breaches in the report
    /// </summary>
    public int TotalBreaches => Breaches.Count;
    
    /// <summary>
    /// Total number of affected users across all breaches
    /// </summary>
    public long TotalAffectedUsers => Breaches.Sum(b => (long)b.PwnCount);
    
    /// <summary>
    /// Formatted date range string for display
    /// </summary>
    public string DateRangeDisplay
    {
        get
        {
            if (!FromDate.HasValue && !ToDate.HasValue)
                return "All Time";
            
            if (FromDate.HasValue && ToDate.HasValue)
                return $"{FromDate.Value:yyyy-MM-dd} to {ToDate.Value:yyyy-MM-dd}";
            
            if (FromDate.HasValue)
                return $"From {FromDate.Value:yyyy-MM-dd}";
            
            return $"Until {ToDate!.Value:yyyy-MM-dd}";
        }
    }
} 