using MediatR;
using BreachApi.Models;

namespace BreachApi.Features.Breaches.Queries;

/// <summary>
/// Query to retrieve breaches with optional date filtering
/// </summary>
public class GetBreachesQuery : IRequest<List<Breach>>
{
    /// <summary>
    /// Optional start date for filtering breaches (inclusive)
    /// </summary>
    public DateTime? FromDate { get; set; }
    
    /// <summary>
    /// Optional end date for filtering breaches (inclusive)
    /// </summary>
    public DateTime? ToDate { get; set; }
} 