using BreachApi.Services;
using Xunit;

namespace BreachApi.Tests.Services;

public class ValidationServiceTests
{
    private readonly ValidationService _validationService = new();

    [Fact]
    public void ValidateDateRange_ValidDates_ReturnsSuccess()
    {
        // Arrange
        var fromDate = new DateTime(2020, 1, 1);
        var toDate = new DateTime(2020, 12, 31);

        // Act
        var result = _validationService.ValidateDateRange(fromDate, toDate);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateDateRange_FromDateAfterToDate_ReturnsFailure()
    {
        // Arrange
        var fromDate = new DateTime(2020, 12, 31);
        var toDate = new DateTime(2020, 1, 1);

        // Act
        var result = _validationService.ValidateDateRange(fromDate, toDate);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("From date cannot be later than to date", result.Errors);
    }

    [Fact]
    public void ValidateDateRange_DateTooEarly_ReturnsFailure()
    {
        // Arrange
        var fromDate = new DateTime(2000, 1, 1);
        var toDate = new DateTime(2020, 1, 1);

        // Act
        var result = _validationService.ValidateDateRange(fromDate, toDate);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("From date cannot be earlier than", result.Errors[0]);
    }

    [Fact]
    public void ValidateDateRange_DateTooLate_ReturnsFailure()
    {
        // Arrange
        var fromDate = DateTime.UtcNow.AddDays(5);
        var toDate = DateTime.UtcNow.AddDays(10);

        // Act
        var result = _validationService.ValidateDateRange(fromDate, toDate);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Any(e => e.Contains("cannot be in the future")));
    }

    [Fact]
    public void ValidateDateRange_DateRangeTooLarge_ReturnsFailure()
    {
        // Arrange
        var fromDate = new DateTime(2010, 1, 1);
        var toDate = new DateTime(2025, 1, 1);

        // Act
        var result = _validationService.ValidateDateRange(fromDate, toDate);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Date range cannot exceed", result.Errors[0]);
    }

    [Fact]
    public void ValidateDateRange_NullDates_ReturnsSuccess()
    {
        // Act
        var result = _validationService.ValidateDateRange(null, null);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateDateRange_OnlyFromDate_ReturnsSuccess()
    {
        // Arrange
        var fromDate = new DateTime(2020, 1, 1);

        // Act
        var result = _validationService.ValidateDateRange(fromDate, null);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateDateRange_OnlyToDate_ReturnsSuccess()
    {
        // Arrange
        var toDate = new DateTime(2020, 12, 31);

        // Act
        var result = _validationService.ValidateDateRange(null, toDate);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}