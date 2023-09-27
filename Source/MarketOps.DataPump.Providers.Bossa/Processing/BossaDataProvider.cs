﻿using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.Stages;
using MarketOps.Types;
using Microsoft.Extensions.Logging;

namespace MarketOps.DataPump.Providers.Bossa.Processing;

/// <summary>
/// Data provider from bossa.
/// </summary>
internal class BossaDataProvider : IDataPumpDataProvider
{
    private readonly IDataDownloader _dataDownloader;
    private readonly ILogger _logger;

    public BossaDataProvider(IDataDownloader dataDownloader, ILogger logger)
    {
        _dataDownloader = dataDownloader;
        _logger = logger;
    }

    public IEnumerable<PumpingData> Get(DateTime lastTs, PumpingDataRange dataRange, StockDefinitionShort stockDefinition) => 
        _dataDownloader.GetLines(dataRange, stockDefinition)
            .Split()
            .Verify(dataRange, _logger)
            .FilterOutToDate(lastTs, dataRange)
            .ToPumpingData(dataRange, stockDefinition);
}