using MarketOps.Common.Pg.Construction;
using MarketOps.Common.Pg.Operations;
using MarketOps.DataPump.Common;

namespace MarketOps.DataPump.Storers.Pg.Operations;

/// <summary>
/// Stores PumpingData in Pg database.
/// </summary>
internal class PgDataPumpStorer : PgOperationBase, IDataPumpDataStorer
{
    public PgDataPumpStorer(PostgresOptions options) : base(options)
    { }

    public void Store(IEnumerable<PumpingData> data)
    {
        foreach (var pumpingData in data)
            StorePumpingData(pumpingData);
    }

    private void StorePumpingData(PumpingData pumpingData)
    {
        throw new NotImplementedException();
    }
}
