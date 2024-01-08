using MarketOps.Types;

namespace MarketOps.DataPump.CliCommands;

/// <summary>
/// Execution options from commandline.
/// </summary>
internal class ExecutionOptions
{
    public bool ParsedCorrectly = false;
    public PumpingDataProvider PumpingDataProvider;
    public StockType[] StockTypes;
    public bool SimulateStore;
}
