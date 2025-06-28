using MediatR;

namespace BreachApi.Features.Breaches.Queries;

/// <summary>
/// Query to generate a PDF report of breaches with optional date filtering
/// </summary>
public class GetBreachesPdfQuery : IRequest<byte[]>
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