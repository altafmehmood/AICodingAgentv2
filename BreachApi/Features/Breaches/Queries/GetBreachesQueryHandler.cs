using MediatR;
using Flurl.Http;
using BreachApi.Models;
using Microsoft.Extensions.Logging;

namespace BreachApi.Features.Breaches.Queries;

/// <summary>
/// Handler for GetBreachesQuery that retrieves breaches from HaveIBeenPwned API
/// </summary>
public class GetBreachesQueryHandler : IRequestHandler<GetBreachesQuery, List<Breach>>
{
    private const string HaveIBeenPwnedApiUrl = "https://haveibeenpwned.com/api/v3/breaches";
    private readonly ILogger<GetBreachesQueryHandler> _logger;
    
    public GetBreachesQueryHandler(ILogger<GetBreachesQueryHandler> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Handles the GetBreachesQuery by calling the HaveIBeenPwned API and filtering results
    /// </summary>
    /// <param name="request">The query request containing optional date filters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of breaches filtered by date and ordered by AddedDate descending</returns>
    public async Task<List<Breach>> Handle(GetBreachesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving breaches with filters - FromDate: {FromDate}, ToDate: {ToDate}", 
            request.FromDate?.ToString("yyyy-MM-dd"), request.ToDate?.ToString("yyyy-MM-dd"));
        
        try
        {
            // Call the HaveIBeenPwned API
            _logger.LogDebug("Calling HaveIBeenPwned API at {ApiUrl}", HaveIBeenPwnedApiUrl);
            
            var breaches = await HaveIBeenPwnedApiUrl
                .WithHeader("User-Agent", "BreachApi-1.0")
                .GetJsonAsync<List<Breach>>(cancellationToken: cancellationToken);
            
            _logger.LogInformation("Successfully retrieved {BreachCount} breaches from HaveIBeenPwned API", 
                breaches.Count);
            
            // Apply date filtering if provided
            var filteredBreaches = breaches.AsEnumerable();
            
            if (request.FromDate.HasValue)
            {
                filteredBreaches = filteredBreaches.Where(b => b.AddedDate >= request.FromDate.Value);
                _logger.LogDebug("Applied FromDate filter: {FromDate}", request.FromDate.Value.ToString("yyyy-MM-dd"));
            }
            
            if (request.ToDate.HasValue)
            {
                filteredBreaches = filteredBreaches.Where(b => b.AddedDate <= request.ToDate.Value);
                _logger.LogDebug("Applied ToDate filter: {ToDate}", request.ToDate.Value.ToString("yyyy-MM-dd"));
            }
            
            // Order by AddedDate descending
            var result = filteredBreaches
                .OrderByDescending(b => b.AddedDate)
                .ToList();
            
            _logger.LogInformation("Returning {FilteredBreachCount} breaches after filtering and ordering", 
                result.Count);
            
            return result;
        }
        catch (FlurlHttpException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while calling HaveIBeenPwned API. Status: {StatusCode}, Message: {Message}", 
                ex.StatusCode, ex.Message);
            
            // Log the error and rethrow with a more meaningful message
            throw new InvalidOperationException($"Failed to retrieve breaches from HaveIBeenPwned API: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while retrieving breaches");
            throw;
        }
    }
} 