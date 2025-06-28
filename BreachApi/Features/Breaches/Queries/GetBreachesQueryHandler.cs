using MediatR;
using BreachApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using BreachApi.Services;

namespace BreachApi.Features.Breaches.Queries;

/// <summary>
/// Handler for GetBreachesQuery that retrieves breaches from HaveIBeenPwned API
/// </summary>
public class GetBreachesQueryHandler : IRequestHandler<GetBreachesQuery, List<Breach>>
{
    private const string CacheKey = "breaches_all";
    private const int CacheExpirationMinutes = 30;
    
    private readonly ILogger<GetBreachesQueryHandler> _logger;
    private readonly IMemoryCache _cache;
    private readonly IBreachApiService _breachApiService;
    
    public GetBreachesQueryHandler(
        ILogger<GetBreachesQueryHandler> logger, 
        IMemoryCache cache,
        IBreachApiService breachApiService)
    {
        _logger = logger;
        _cache = cache;
        _breachApiService = breachApiService;
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
            // Try to get cached breaches first
            List<Breach> breaches;
            
            if (_cache.TryGetValue(CacheKey, out List<Breach>? cachedBreaches) && cachedBreaches != null)
            {
                _logger.LogInformation("Retrieved {BreachCount} breaches from cache", cachedBreaches.Count);
                breaches = cachedBreaches;
            }
            else
            {
                // Call the external API with resilience policies
                breaches = await _breachApiService.GetAllBreachesAsync(cancellationToken);
                
                // Cache the results
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes),
                    SlidingExpiration = TimeSpan.FromMinutes(10),
                    Priority = CacheItemPriority.High
                };
                
                _cache.Set(CacheKey, breaches, cacheOptions);
                _logger.LogDebug("Cached {BreachCount} breaches for {ExpirationMinutes} minutes", 
                    breaches.Count, CacheExpirationMinutes);
            }
            
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing breach query");
            throw;
        }
    }
} 