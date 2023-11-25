using MarketOps.Types;

namespace MarketOps.DataPump.Common;

/// <summary>
/// Pumping data generated from source and saved to storage.
/// </summary>
public class PumpingData
{
    public readonly PumpingDataRange DataRange;
    public readonly StockDefinitionShort StockDefinition;
    public readonly string O;
    public readonly string H;
    public readonly string L;
    public readonly string C;
    public readonly string V;
    public readonly string Ts;

    public PumpingData(PumpingDataRange dataRange, StockDefinitionShort stockDefinition, string o, string h, string l, string c, string v, string ts)
    {
        DataRange = dataRange;
        StockDefinition = stockDefinition;
        O = o;
        H = h;
        L = l;
        C = c;
        V = v;
        Ts = ts;
    }
}
