using MarketOps.DataPump.Common;

namespace MarketOps.DataPump.Providers.PkoFunds.Exceptions;

internal class PkoFundsFundNotFoundException : DataPumpException
{
    public readonly string FundName;

    public PkoFundsFundNotFoundException(string fundName) : base(GetMessage(fundName))
    {
        FundName = fundName;
    }

    private static string GetMessage(in string fundName) =>
        $"[{fundName}] not found in data file.";
}
