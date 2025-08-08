using BreachApi.Features.Breaches.Queries;
using BreachApi.Models;
using BreachApi.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BreachApi.Tests.Features.Breaches.Queries;

public class GetBreachesQueryHandlerTests
{
    private readonly Mock<ILogger<GetBreachesQueryHandler>> _mockLogger;
    private readonly Mock<IMemoryCache> _mockCache;
    private readonly Mock<IBreachApiService> _mockBreachApiService;
    private readonly GetBreachesQueryHandler _handler;

    public GetBreachesQueryHandlerTests()
    {
        _mockLogger = new Mock<ILogger<GetBreachesQueryHandler>>();
        _mockCache = new Mock<IMemoryCache>();
        _mockBreachApiService = new Mock<IBreachApiService>();
        _handler = new GetBreachesQueryHandler(_mockLogger.Object, _mockCache.Object, _mockBreachApiService.Object);
    }

    [Fact]
    public async Task Handle_CacheHit_ReturnsCachedData()
    {
        // Arrange
        var request = new GetBreachesQuery();
        var cachedBreaches = new List<Breach>
        {
            new() { Name = "CachedBreach", AddedDate = DateTime.UtcNow }
        };

        object cacheValue = cachedBreaches;
        _mockCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheValue))
            .Returns(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("CachedBreach", result[0].Name);
        _mockBreachApiService.Verify(x => x.GetAllBreachesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CacheMiss_CallsApiAndCachesResult()
    {
        // Arrange
        var request = new GetBreachesQuery();
        var apiBreaches = new List<Breach>
        {
            new() { Name = "ApiBreach", AddedDate = DateTime.UtcNow }
        };

        object cacheValue = null!;
        _mockCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheValue))
            .Returns(false);

        _mockBreachApiService
            .Setup(x => x.GetAllBreachesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(apiBreaches);

        var mockCacheEntry = new Mock<ICacheEntry>();
        _mockCache
            .Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(mockCacheEntry.Object);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("ApiBreach", result[0].Name);
        _mockBreachApiService.Verify(x => x.GetAllBreachesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockCache.Verify(x => x.Set(It.IsAny<object>(), It.IsAny<object>(), It.IsAny<MemoryCacheEntryOptions>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDateFilters_FiltersResults()
    {
        // Arrange
        var fromDate = new DateTime(2020, 6, 1);
        var toDate = new DateTime(2020, 12, 31);
        var request = new GetBreachesQuery { FromDate = fromDate, ToDate = toDate };

        var allBreaches = new List<Breach>
        {
            new() { Name = "EarlyBreach", AddedDate = new DateTime(2020, 1, 1) },
            new() { Name = "MiddleBreach", AddedDate = new DateTime(2020, 8, 1) },
            new() { Name = "LateBreach", AddedDate = new DateTime(2021, 1, 1) }
        };

        object cacheValue = allBreaches;
        _mockCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheValue))
            .Returns(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("MiddleBreach", result[0].Name);
    }

    [Fact]
    public async Task Handle_WithFromDateOnly_FiltersFromDate()
    {
        // Arrange
        var fromDate = new DateTime(2020, 6, 1);
        var request = new GetBreachesQuery { FromDate = fromDate };

        var allBreaches = new List<Breach>
        {
            new() { Name = "EarlyBreach", AddedDate = new DateTime(2020, 1, 1) },
            new() { Name = "MiddleBreach", AddedDate = new DateTime(2020, 8, 1) },
            new() { Name = "LateBreach", AddedDate = new DateTime(2021, 1, 1) }
        };

        object cacheValue = allBreaches;
        _mockCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheValue))
            .Returns(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, b => b.Name == "MiddleBreach");
        Assert.Contains(result, b => b.Name == "LateBreach");
        Assert.DoesNotContain(result, b => b.Name == "EarlyBreach");
    }

    [Fact]
    public async Task Handle_OrdersByAddedDateDescending()
    {
        // Arrange
        var request = new GetBreachesQuery();
        var allBreaches = new List<Breach>
        {
            new() { Name = "FirstBreach", AddedDate = new DateTime(2020, 1, 1) },
            new() { Name = "SecondBreach", AddedDate = new DateTime(2020, 6, 1) },
            new() { Name = "ThirdBreach", AddedDate = new DateTime(2020, 12, 1) }
        };

        object cacheValue = allBreaches;
        _mockCache
            .Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheValue))
            .Returns(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("ThirdBreach", result[0].Name);
        Assert.Equal("SecondBreach", result[1].Name);
        Assert.Equal("FirstBreach", result[2].Name);
    }
}