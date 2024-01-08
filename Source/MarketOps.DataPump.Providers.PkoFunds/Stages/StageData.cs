namespace MarketOps.DataPump.Providers.PkoFunds.Stages;

/// <summary>
/// Data processed through pumping stages.
/// </summary>
internal record StageData(
    string Ts,
    string Price);
