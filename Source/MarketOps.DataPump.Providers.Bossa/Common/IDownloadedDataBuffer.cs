using MarketOps.DataPump.Common;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Bossa.Common;

/// <summary>
/// Interface for buffering downloaded data files from bossa.
/// </summary>
internal interface IDownloadedDataBuffer
{
    public StreamReader Get(PumpingDataRange dataRange, StockType stockType, StockDefinitionShort stockDefinition);
}
