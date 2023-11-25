namespace MarketOps.DataPump.Common;

/// <summary>
/// Base DataPump exception class.
/// </summary>
public abstract class DataPumpException : Exception
{
    public DataPumpException(string message) : base(message) 
    { }

    public DataPumpException(string message, Exception innerException) : base(message, innerException) 
    { }
}
