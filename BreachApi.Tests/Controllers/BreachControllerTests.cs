using BreachApi.Controllers;
using BreachApi.Features.Breaches.Queries;
using BreachApi.Models;
using BreachApi.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BreachApi.Tests.Controllers;

public class BreachControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<BreachController>> _mockLogger;
    private readonly Mock<IValidationService> _mockValidationService;
    private readonly BreachController _controller;

    public BreachControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<BreachController>>();
        _mockValidationService = new Mock<IValidationService>();
        _controller = new BreachController(_mockMediator.Object, _mockLogger.Object, _mockValidationService.Object);
    }

    [Fact]
    public async Task GetBreaches_ValidInput_ReturnsOkResult()
    {
        // Arrange
        var fromDate = new DateTime(2020, 1, 1);
        var toDate = new DateTime(2020, 12, 31);
        var expectedBreaches = new List<Breach>
        {
            new() { Name = "TestBreach1", Title = "Test Breach 1" },
            new() { Name = "TestBreach2", Title = "Test Breach 2" }
        };

        _mockValidationService
            .Setup(x => x.ValidateDateRange(fromDate, toDate))
            .Returns(ValidationResult.Success());

        _mockMediator
            .Setup(x => x.Send(It.IsAny<GetBreachesQuery>(), default))
            .ReturnsAsync(expectedBreaches);

        // Act
        var result = await _controller.GetBreaches(fromDate, toDate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var breaches = Assert.IsType<List<Breach>>(okResult.Value);
        Assert.Equal(2, breaches.Count);
        Assert.Equal("TestBreach1", breaches[0].Name);
    }

    [Fact]
    public async Task GetBreaches_InvalidInput_ReturnsBadRequest()
    {
        // Arrange
        var fromDate = new DateTime(2020, 12, 31);
        var toDate = new DateTime(2020, 1, 1);
        var validationErrors = new[] { "From date cannot be later than to date" };

        _mockValidationService
            .Setup(x => x.ValidateDateRange(fromDate, toDate))
            .Returns(ValidationResult.Failure(validationErrors));

        // Act
        var result = await _controller.GetBreaches(fromDate, toDate);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var errorResponse = badRequestResult.Value;
        Assert.NotNull(errorResponse);
    }

    [Fact]
    public async Task GetBreaches_NullDates_ReturnsOkResult()
    {
        // Arrange
        var expectedBreaches = new List<Breach>
        {
            new() { Name = "TestBreach1", Title = "Test Breach 1" }
        };

        _mockValidationService
            .Setup(x => x.ValidateDateRange(null, null))
            .Returns(ValidationResult.Success());

        _mockMediator
            .Setup(x => x.Send(It.IsAny<GetBreachesQuery>(), default))
            .ReturnsAsync(expectedBreaches);

        // Act
        var result = await _controller.GetBreaches();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var breaches = Assert.IsType<List<Breach>>(okResult.Value);
        Assert.Single(breaches);
    }

    [Fact]
    public async Task GetBreachesPdf_ValidInput_ReturnsFileResult()
    {
        // Arrange
        var fromDate = new DateTime(2020, 1, 1);
        var toDate = new DateTime(2020, 12, 31);
        var expectedPdfBytes = new byte[] { 1, 2, 3, 4, 5 };

        _mockValidationService
            .Setup(x => x.ValidateDateRange(fromDate, toDate))
            .Returns(ValidationResult.Success());

        _mockMediator
            .Setup(x => x.Send(It.IsAny<GetBreachesPdfQuery>(), default))
            .ReturnsAsync(expectedPdfBytes);

        // Act
        var result = await _controller.GetBreachesPdf(fromDate, toDate);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/pdf", fileResult.ContentType);
        Assert.Equal(expectedPdfBytes, fileResult.FileContents);
        Assert.Contains("breach-report", fileResult.FileDownloadName);
    }

    [Fact]
    public async Task GetBreachesPdf_InvalidInput_ReturnsBadRequest()
    {
        // Arrange
        var fromDate = new DateTime(2020, 12, 31);
        var toDate = new DateTime(2020, 1, 1);
        var validationErrors = new[] { "From date cannot be later than to date" };

        _mockValidationService
            .Setup(x => x.ValidateDateRange(fromDate, toDate))
            .Returns(ValidationResult.Failure(validationErrors));

        // Act
        var result = await _controller.GetBreachesPdf(fromDate, toDate);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errorResponse = badRequestResult.Value;
        Assert.NotNull(errorResponse);
    }
}