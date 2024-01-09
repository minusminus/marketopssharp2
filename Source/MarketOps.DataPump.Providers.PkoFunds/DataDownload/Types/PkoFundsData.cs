namespace MarketOps.DataPump.Providers.PkoFunds.DataDownload.Types;

/// <summary>
/// PKO funds data from downloaded file.
/// </summary>
internal class PkoFundsData
{
    public readonly IReadOnlyDictionary<string, int> FundNameToIndex;
    public readonly IReadOnlyDictionary<string, int> DateToIndex;
    public readonly string[][] Data;

    public PkoFundsData(Dictionary<string, int> fundNameToIndex, Dictionary<string, int> dateToIndex, string[][] data)
    {
        FundNameToIndex = fundNameToIndex;
        DateToIndex = dateToIndex;
        Data = data;
    }
}
