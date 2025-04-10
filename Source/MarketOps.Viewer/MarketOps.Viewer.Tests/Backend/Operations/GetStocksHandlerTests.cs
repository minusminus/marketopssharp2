using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using MarketOps.Viewer.Backend.DataStore.Abstractions;
using MarketOps.Viewer.Backend.DataStore.Dto;
using MarketOps.Viewer.Backend.Entities;
using MarketOps.Viewer.Backend.Operations;
using NSubstitute.ExceptionExtensions;

namespace MarketOps.Viewer.Tests.Backend.Operations;

[TestFixture]
internal class GetStocksHandlerTests
{
    // Mockujemy tylko potrzebną kwerendę
    private IGetAllEnabledStocksQuery _mockGetAllEnabledStocksQuery;
    private ILogger<GetStocksHandler> _mockLogger;
    private GetStocksHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _mockGetAllEnabledStocksQuery = Substitute.For<IGetAllEnabledStocksQuery>();
        _mockLogger = Substitute.For<ILogger<GetStocksHandler>>();
        _handler = new GetStocksHandler(_mockGetAllEnabledStocksQuery, _mockLogger);
    }

    [Test]
    public async Task HandleGetStocksAsync_WhenStocksExist_ReturnsOkWithMappedData()
    {
        // Arrange
        var rawData = new List<StockInfoQueryResultDto> {
            new (){ Id = 1, Symbol = "CDR", StockName = "CD Projekt", TypeInt = (int)StockType.Stock },
            new (){ Id = 2, Symbol = "WIG20", StockName = "WIG20 Index", TypeInt = (int)StockType.Index }
        };
        var expectedMappedData = new List<StockInfoDto> {
            new StockInfoDto(1, "CDR", "CD Projekt", "Stock"),
            new StockInfoDto(2, "WIG20", "WIG20 Index", "Index")
        };

        _mockGetAllEnabledStocksQuery.ExecuteAsync().Returns(Task.FromResult<IEnumerable<StockInfoQueryResultDto>>(rawData));

        // Act
        var result = await _handler.HandleGetStocksAsync();

        // Assert
        result.ShouldBeOfType<Ok<List<StockInfoDto>>>();
        var okResult = (Ok<List<StockInfoDto>>)result;
        okResult.Value.ShouldNotBeNull();
        okResult.Value.ShouldBe(expectedMappedData, ignoreOrder: false);
    }

    [Test]
    public async Task HandleGetStocksAsync_WhenNoStocksExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var rawData = new List<StockInfoQueryResultDto>();
        _mockGetAllEnabledStocksQuery.ExecuteAsync().Returns(Task.FromResult<IEnumerable<StockInfoQueryResultDto>>(rawData));

        // Act
        var result = await _handler.HandleGetStocksAsync();

        // Assert
        result.ShouldBeOfType<Ok<List<StockInfoDto>>>();
        var okResult = (Ok<List<StockInfoDto>>)result;
        okResult.Value.ShouldNotBeNull();
        okResult.Value.ShouldBeEmpty();
    }

    [Test]
    public async Task HandleGetStocksAsync_QueryThrowsException_ReturnsProblem()
    {
        // Arrange
        var exception = new Exception("Failed to query stocks");
        _mockGetAllEnabledStocksQuery.ExecuteAsync().ThrowsAsync(exception);

        // Act
        var result = await _handler.HandleGetStocksAsync();

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problemResult = (ProblemHttpResult)result;
        problemResult.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        problemResult.ProblemDetails.Detail.ShouldBe("An error occurred while fetching stock list.");
        _mockLogger.ReceivedWithAnyArgs().LogError(exception, default);
    }
}