using MarketOps.Viewer.Backend.DataStore.Abstractions;
using MarketOps.Viewer.Backend.DataStore.Dto;
using MarketOps.Viewer.Backend.Entities;

namespace MarketOps.Viewer.Backend.Operations;

internal class GetStocksHandler
{
    private readonly IGetAllEnabledStocksQuery _getAllEnabledStocksQuery;
    private readonly ILogger<GetStocksHandler> _logger;

    public GetStocksHandler(IGetAllEnabledStocksQuery getAllEnabledStocksQuery, ILogger<GetStocksHandler> logger)
    {
        _getAllEnabledStocksQuery = getAllEnabledStocksQuery;
        _logger = logger;
    }

    public async Task<IResult> HandleGetStocksAsync()
    {
        try
        {
            var rawResult = await _getAllEnabledStocksQuery.ExecuteAsync();
            var mappedResult = MapToStockInfoDto(rawResult);
            return TypedResults.Ok(mappedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request for stock list");
            return TypedResults.Problem("An error occurred while fetching stock list.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static List<StockInfoDto> MapToStockInfoDto(IEnumerable<StockInfoQueryResultDto> result) =>
        result
        .Select(r => new StockInfoDto(
            r.Id,
            r.Symbol,
            r.StockName,
            ((StockType)r.TypeInt).ToString() // Mapowanie enum na string
        ))
        .ToList();
}