using MediatR;
using BreachApi.Services;
using BreachApi.ViewModels;
using Microsoft.Extensions.Logging;

namespace BreachApi.Features.Breaches.Queries;

/// <summary>
/// Handler for GetBreachesPdfQuery that generates PDF reports
/// </summary>
public class GetBreachesPdfQueryHandler : IRequestHandler<GetBreachesPdfQuery, byte[]>
{
    private readonly IMediator _mediator;
    private readonly IPdfService _pdfService;
    private readonly ILogger<GetBreachesPdfQueryHandler> _logger;

    public GetBreachesPdfQueryHandler(
        IMediator mediator,
        IPdfService pdfService,
        ILogger<GetBreachesPdfQueryHandler> logger)
    {
        _mediator = mediator;
        _pdfService = pdfService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetBreachesPdfQuery by retrieving breach data and generating a PDF report
    /// </summary>
    /// <param name="request">The query request containing optional date filters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>PDF file as byte array</returns>
    public async Task<byte[]> Handle(GetBreachesPdfQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating PDF report with filters - FromDate: {FromDate}, ToDate: {ToDate}", 
            request.FromDate?.ToString("yyyy-MM-dd"), request.ToDate?.ToString("yyyy-MM-dd"));

        try
        {
            // First, retrieve the breach data using the existing query
            var getBreachesQuery = new GetBreachesQuery
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate
            };

            var breaches = await _mediator.Send(getBreachesQuery, cancellationToken);

            _logger.LogInformation("Retrieved {BreachCount} breaches for PDF generation", breaches.Count);

            // Create the view model for PDF generation
            var viewModel = new BreachReportViewModel
            {
                Breaches = breaches,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                GeneratedDate = DateTime.UtcNow
            };

            // Generate the PDF
            var pdfBytes = await _pdfService.GenerateBreachReportPdfAsync(viewModel);

            _logger.LogInformation("Successfully generated PDF report. Size: {PdfSize} bytes", pdfBytes.Length);

            return pdfBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF report");
            throw;
        }
    }
} 