using MarketOps.DataPump.Common;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Bossa.Processing;

/// <summary>
/// Interface for downloading data files from bossa.
/// </summary>
internal interface IDataDownloader
{
    public IEnumerable<string> GetLines(PumpingDataRange dataRange, StockDefinitionShort stockDefinition);
}
