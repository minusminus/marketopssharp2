using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.HttpResults; // Dla Ok<T>, BadRequest, NotFound, Problem
using Microsoft.AspNetCore.Http;             // Dla IResult, StatusCodes
using MarketOps.Viewer.Backend.DataStore.Abstractions;
using MarketOps.Viewer.Backend.DataStore.Dto;
using MarketOps.Viewer.Backend.Entities;
using MarketOps.Viewer.Backend.Operations;
using NSubstitute.ExceptionExtensions;

namespace MarketOps.Viewer.Tests.Backend.Operations;

// Załóżmy, że klasy DTO, Enumy, Interfejsy i Handlery są dostępne (np. w tym samym namespace lub przez using)

[TestFixture]
internal class GetStockDataHandlerTests
{
    // Mockujemy teraz poszczególne kwerendy
    private IGetStockTypeQuery _mockGetStockTypeQuery;
    private IGetOhlcvDataQuery _mockGetOhlcvDataQuery;
    private IDatabaseHelper _mockDatabaseHelper;
    private ILogger<GetStockDataHandler> _mockLogger;
    private GetStockDataHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _mockGetStockTypeQuery = Substitute.For<IGetStockTypeQuery>();
        _mockGetOhlcvDataQuery = Substitute.For<IGetOhlcvDataQuery>();
        _mockDatabaseHelper = Substitute.For<IDatabaseHelper>();
        _mockLogger = Substitute.For<ILogger<GetStockDataHandler>>();
        _handler = new GetStockDataHandler(
            _mockGetStockTypeQuery,
            _mockGetOhlcvDataQuery,
            _mockDatabaseHelper,
            _mockLogger);
    }

    [Test]
    public async Task HandleGetStockDataAsync_ValidRequest_ReturnsOkWithData()
    {
        // Arrange
        const int stockId = 1;
        const string timeframe = "daily";
        var startDate = new DateTime(2023, 1, 1);
        var endDate = new DateTime(2023, 1, 31);
        const StockType stockType = StockType.Stock;
        const string tableName = "public.at_dzienne0";
        var expectedData = new List<OhlcvDataPointDto> {
            new OhlcvDataPointDto(new DateTime(2023, 1, 2), 100, 105, 99, 102, 1000)
        };
        _mockGetStockTypeQuery.ExecuteAsync(stockId).Returns(Task.FromResult<StockType?>(stockType));
        _mockDatabaseHelper.GetOhlcvTableName(stockType, Timeframe.Daily).Returns(tableName);
        _mockGetOhlcvDataQuery.ExecuteAsync(stockId, tableName, startDate, endDate).Returns(Task.FromResult<IEnumerable<OhlcvDataPointDto>>(expectedData));

        // Act
        var result = await _handler.HandleGetStockDataAsync(stockId, timeframe, startDate, endDate);

        // Assert
        result.ShouldBeOfType<Ok<IEnumerable<OhlcvDataPointDto>>>();
        var okResult = (Ok<IEnumerable<OhlcvDataPointDto>>)result;
        okResult.Value.ShouldNotBeNull();
        okResult.Value.ShouldBe(expectedData); // Porównanie referencji lub wartości jeśli rekordy
        okResult.Value.Count().ShouldBe(1);
    }

    [Test]
    public async Task HandleGetStockDataAsync_InvalidTimeframe_ReturnsBadRequest()
    {
        // Arrange
        const int stockId = 1;
        const string invalidTimeframe = "yearly";

        // Act
        var result = await _handler.HandleGetStockDataAsync(stockId, invalidTimeframe);

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        var badRequestResult = (BadRequest<string>)result;
        badRequestResult.Value.ShouldBe("Invalid timeframe. Use 'daily', 'weekly', or 'monthly'.");
    }

    [Test]
    public async Task HandleGetStockDataAsync_StockNotFound_ReturnsNotFound()
    {
        // Arrange
        const int nonExistentStockId = 999;
        _mockGetStockTypeQuery.ExecuteAsync(nonExistentStockId).Returns(Task.FromResult<StockType?>(null));

        // Act
        var result = await _handler.HandleGetStockDataAsync(nonExistentStockId);

        // Assert
        result.ShouldBeOfType<NotFound<string>>();
        var notFoundResult = (NotFound<string>)result;
        notFoundResult.Value.ShouldBe($"Stock with ID {nonExistentStockId} not found.");
        await _mockGetOhlcvDataQuery.DidNotReceiveWithAnyArgs().ExecuteAsync(default, default, default, default);
    }

    [Test]
    public async Task HandleGetStockDataAsync_UnsupportedStockTypeTimeframe_ReturnsBadRequest()
    {
        // Arrange
        const int stockId = 1;
        const StockType stockType = StockType.IndexFuture; // Załóżmy, że dla Monthly nie ma tabeli
        const string timeframe = "monthly";
        _mockGetStockTypeQuery.ExecuteAsync(stockId).Returns(Task.FromResult<StockType?>(stockType));
        _mockDatabaseHelper.GetOhlcvTableName(stockType, Timeframe.Monthly).Returns((string?)null);

        // Act
        var result = await _handler.HandleGetStockDataAsync(stockId, timeframe);

        // Assert
        result.ShouldBeOfType<BadRequest<string>>();
        var badRequestResult = (BadRequest<string>)result;
        badRequestResult.Value.ShouldBe($"Data for timeframe '{timeframe}' and stock type '{stockType}' is not supported.");
    }

    [Test]
    public async Task HandleGetStockDataAsync_QueryThrowsException_ReturnsProblem()
    {
        // Arrange
        const int stockId = 1;
        var exception = new Exception("Database connection failed");
        _mockGetStockTypeQuery.ExecuteAsync(stockId).ThrowsAsync(exception);

        // Act
        var result = await _handler.HandleGetStockDataAsync(stockId);

        // Assert
        result.ShouldBeOfType<ProblemHttpResult>();
        var problemResult = (ProblemHttpResult)result;
        problemResult.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        problemResult.ProblemDetails.Title.ShouldBe("An error occurred while processing your request.");
        problemResult.ProblemDetails.Detail.ShouldBe($"An error occurred while fetching data for stock ID {stockId}.");
        _mockLogger.ReceivedWithAnyArgs().LogError(exception, default);
    }
}
