﻿using MarketOps.Types;

namespace MarketOps.DataPump.Common;

/// <summary>
/// Interface to generate pumps' stock data from external source.
/// </summary>
public interface IDataPumpDataProvider
{
    public IEnumerable<PumpingData> Get(DateTime lastTs, PumpingDataRange dataRange, StockDefinitionShort stockDefinition);
}
