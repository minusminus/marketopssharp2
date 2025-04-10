using MarketOps.Viewer.Backend.DataStore.Dto;

namespace MarketOps.Viewer.Backend.DataStore.Abstractions;

internal interface IGetOhlcvDataQuery
{
    Task<IEnumerable<OhlcvDataPointDto>> ExecuteAsync(int stockId, string tableName, DateTime? startDate, DateTime? endDate);
}
