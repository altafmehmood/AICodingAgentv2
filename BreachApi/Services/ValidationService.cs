namespace BreachApi.Services;

/// <summary>
/// Service for input validation
/// </summary>
public class ValidationService : IValidationService
{
    private static readonly DateTime MinAllowedDate = new(2007, 1, 1); // Around when HIBP started tracking
    private static readonly DateTime MaxAllowedDate = DateTime.UtcNow.AddDays(1);
    private static readonly TimeSpan MaxDateRange = TimeSpan.FromDays(365 * 10); // 10 years max range

    /// <summary>
    /// Validates date range parameters
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>Validation result</returns>
    public ValidationResult ValidateDateRange(DateTime? fromDate, DateTime? toDate)
    {
        var errors = new List<string>();

        // Validate individual dates
        if (fromDate.HasValue)
        {
            if (fromDate.Value < MinAllowedDate)
            {
                errors.Add($"From date cannot be earlier than {MinAllowedDate:yyyy-MM-dd}");
            }
            
            if (fromDate.Value > MaxAllowedDate)
            {
                errors.Add($"From date cannot be in the future beyond {MaxAllowedDate:yyyy-MM-dd}");
            }
        }

        if (toDate.HasValue)
        {
            if (toDate.Value < MinAllowedDate)
            {
                errors.Add($"To date cannot be earlier than {MinAllowedDate:yyyy-MM-dd}");
            }
            
            if (toDate.Value > MaxAllowedDate)
            {
                errors.Add($"To date cannot be in the future beyond {MaxAllowedDate:yyyy-MM-dd}");
            }
        }

        // Validate date range logic
        if (fromDate.HasValue && toDate.HasValue)
        {
            if (fromDate.Value > toDate.Value)
            {
                errors.Add("From date cannot be later than to date");
            }

            var dateRange = toDate.Value - fromDate.Value;
            if (dateRange > MaxDateRange)
            {
                errors.Add($"Date range cannot exceed {MaxDateRange.Days} days");
            }
        }

        return errors.Any() ? ValidationResult.Failure(errors.ToArray()) : ValidationResult.Success();
    }
}