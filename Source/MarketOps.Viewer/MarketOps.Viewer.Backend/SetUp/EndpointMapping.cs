using MarketOps.Viewer.Backend.Operations;
using Microsoft.AspNetCore.Mvc;

namespace MarketOps.Viewer.Backend.SetUp;

internal static class EndpointMapping
{
    public static void MapEndpoints(this IEndpointRouteBuilder appBuilder)
    {
        var apiGroup = appBuilder.MapGroup("/api");

        apiGroup.MapGet("/stocks", async (
                GetStocksHandler handler) =>
                    await handler.HandleGetStocksAsync())
            .WithName("GetStocks")
            .WithTags("Stocks");

        apiGroup.MapGet("/stockdata/{stockId}", async (
                int stockId,
                [FromQuery] string timeframe,
                [FromQuery] DateTime? startDate,
                [FromQuery] DateTime? endDate,
                GetStockDataHandler handler) => 
                    await handler.HandleGetStockDataAsync(stockId, timeframe, startDate, endDate))
            .WithName("GetStockData")
            .WithTags("Stocks");
    }
}
