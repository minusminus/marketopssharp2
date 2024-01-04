using MarketOps.DataPump.Providers.PkoFunds.Config;

namespace MarketOps.Tests.DataPump.Providers.PkoFunds.TestDataTools;

internal static class PkofundsDefsTools
{
    public static PkoFundsDefs Get() =>
        new()
        {
            DownloadPath = string.Empty,
            StocksMapping = new()
            {
                { "PKO001", "PKO Zrównoważony" },
                { "PKO002", "PKO Dynamicznej Alokacji" },
                { "PKO005", "PKO Obligacji Skarbowych Krótkoterminowy" },
            }
        };
}
