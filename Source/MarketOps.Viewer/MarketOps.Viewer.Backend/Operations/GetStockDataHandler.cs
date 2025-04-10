using MarketOps.Viewer.Backend.DataStore.Abstractions;
using MarketOps.Viewer.Backend.Entities;

namespace MarketOps.Viewer.Backend.Operations;

internal class GetStockDataHandler
{
    private readonly IGetStockTypeQuery _getStockTypeQuery;
    private readonly IGetOhlcvDataQuery _getOhlcvDataQuery;
    private readonly IDatabaseHelper _databaseHelper;
    private readonly ILogger<GetStockDataHandler> _logger;

    public GetStockDataHandler(
        IGetStockTypeQuery getStockTypeQuery,
        IGetOhlcvDataQuery getOhlcvDataQuery,
        IDatabaseHelper databaseHelper,
        ILogger<GetStockDataHandler> logger)
    {
        _getStockTypeQuery = getStockTypeQuery;
        _getOhlcvDataQuery = getOhlcvDataQuery;
        _databaseHelper = databaseHelper;
        _logger = logger;
    }

    public async Task<IResult> HandleGetStockDataAsync(
        int stockId,
        string timeframe = "daily",
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        Timeframe requestedTimeframe = GetTimeFrame(timeframe);
        if (requestedTimeframe == Timeframe.Undefined)
            return TypedResults.BadRequest("Invalid timeframe. Use 'daily', 'weekly', or 'monthly'.");

        try
        {
            var stockType = await _getStockTypeQuery.ExecuteAsync(stockId);
            if (stockType == null)
            {
                _logger.LogWarning("Stock with ID {StockId} not found.", stockId);
                return TypedResults.NotFound($"Stock with ID {stockId} not found.");
            }

            if (!Enum.IsDefined(typeof(StockType), stockType.Value) || stockType.Value == StockType.Undefined)
            {
                _logger.LogWarning("Stock with ID {StockId} has unsupported type {StockType}.", stockId, stockType.Value);
                return TypedResults.NotFound($"Stock with ID {stockId} found, but its type '{stockType.Value}' is not supported for data retrieval.");
            }

            var tableName = _databaseHelper.GetOhlcvTableName(stockType.Value, requestedTimeframe);
            if (string.IsNullOrEmpty(tableName))
            {
                _logger.LogWarning("Data not supported for timeframe {Timeframe} and stock type {StockType}", timeframe, stockType.Value);
                return TypedResults.BadRequest($"Data for timeframe '{timeframe}' and stock type '{stockType.Value}' is not supported.");
            }

            var data = await _getOhlcvDataQuery.ExecuteAsync(stockId, tableName, startDate, endDate);

            return TypedResults.Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request for stock data for ID {StockId}", stockId);
            return TypedResults.Problem($"An error occurred while fetching data for stock ID {stockId}.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static Timeframe GetTimeFrame(string timeframe) =>
           timeframe?.ToLowerInvariant() switch
           {
               "daily" => Timeframe.Daily,
               "weekly" => Timeframe.Weekly,
               "monthly" => Timeframe.Monthly,
               _ => Timeframe.Undefined
           };
}
