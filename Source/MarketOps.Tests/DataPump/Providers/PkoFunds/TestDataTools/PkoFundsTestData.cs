using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Processing;
using MarketOps.DataPump.Providers.PkoFunds.DataDownload.Types;

namespace MarketOps.Tests.DataPump.Providers.PkoFunds.TestDataTools;

/// <summary>
/// Test data for Pko funds.
/// </summary>
internal static class PkoFundsTestData
{
    public static readonly List<string> TestData = new()
    {
        "Data;PKO Zrównoważony;PKO Dynamicznej Alokacji;PKO Obligacji Skarbowych Krótkoterminowy",
        "2023-11-24;2310,07;112,23;106,99;",
        "2023-11-23;2309,50;112,21;106,98;",
        "2023-11-22;2309,40;112,19;106,96;",
        "2023-11-21;2310,00;112,16;106,94;",
        "2023-11-20;2309,91;112,14;106,92;",
    };

    public static readonly PkoFundsData FundsData = PkoDataStreamReader.Read(DataStreamTools.CreateDataStream(TestData));
}
