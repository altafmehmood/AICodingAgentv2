using BreachApi.ViewModels;

namespace BreachApi.Services;

/// <summary>
/// Service interface for PDF generation operations
/// </summary>
public interface IPdfService
{
    /// <summary>
    /// Generates a PDF report from the provided view model
    /// </summary>
    /// <param name="viewModel">The view model containing breach data</param>
    /// <returns>PDF file as byte array</returns>
    Task<byte[]> GenerateBreachReportPdfAsync(BreachReportViewModel viewModel);
} 