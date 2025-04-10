namespace MarketOps.Viewer.Backend.DataStore.Dto;

internal record OhlcvDataPointDto(
    DateTime Timestamp,
    decimal? Open,
    decimal? High,
    decimal? Low,
    decimal? Close,
    int? Volume
);