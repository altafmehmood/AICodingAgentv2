using Microsoft.AspNetCore.Mvc;
using MediatR;
using BreachApi.Features.Breaches.Queries;
using BreachApi.Models;
using Microsoft.Extensions.Logging;

namespace BreachApi.Controllers;

/// <summary>
/// Controller for managing breach data
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BreachController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BreachController> _logger;
    
    public BreachController(IMediator mediator, ILogger<BreachController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    /// <summary>
    /// Retrieves breaches with optional date filtering
    /// </summary>
    /// <param name="fromDate">Optional start date for filtering (inclusive)</param>
    /// <param name="toDate">Optional end date for filtering (inclusive)</param>
    /// <returns>List of breaches ordered by AddedDate descending</returns>
    /// <response code="200">Returns the list of breaches</response>
    /// <response code="500">If there was an error retrieving the breaches</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<Breach>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<Breach>>> GetBreaches(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        _logger.LogInformation("GetBreaches endpoint called with FromDate: {FromDate}, ToDate: {ToDate}", 
            fromDate?.ToString("yyyy-MM-dd"), toDate?.ToString("yyyy-MM-dd"));
        
        var query = new GetBreachesQuery
        {
            FromDate = fromDate,
            ToDate = toDate
        };
        
        var breaches = await _mediator.Send(query);
        
        _logger.LogInformation("GetBreaches endpoint completed successfully. Returning {BreachCount} breaches", 
            breaches.Count);
        
        return Ok(breaches);
    }
} 