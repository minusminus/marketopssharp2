using MarketOps.DataPump.Common;

namespace MarketOps.DataPump.Providers.PkoFunds.Exceptions;

internal class PkoFundsDataException : DataPumpException
{
    public PkoFundsDataException(string message) : base(message)
    {
    }
}
