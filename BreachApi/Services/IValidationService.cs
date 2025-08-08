namespace BreachApi.Services;

/// <summary>
/// Interface for validation services
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// Validates date range parameters
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>Validation result</returns>
    ValidationResult ValidateDateRange(DateTime? fromDate, DateTime? toDate);
}

/// <summary>
/// Represents the result of a validation operation
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    
    public static ValidationResult Success() => new() { IsValid = true };
    
    public static ValidationResult Failure(params string[] errors) => new() 
    { 
        IsValid = false, 
        Errors = errors.ToList() 
    };
}