namespace MarketOps.Viewer.Backend.DataReading.Dto;

internal record OhlcvDataPointDto(
    DateTime Timestamp,
    decimal? Open,
    decimal? High,
    decimal? Low,
    decimal? Close,
    int? Volume
);