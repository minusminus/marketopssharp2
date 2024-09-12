using MarketOps.Scanner.Common;

namespace MarketOps.Scanner.Abstractions;

/// <summary>
/// Interface for scanners factory.
/// </summary>
internal interface IScannersFactory
{
    public IScanner? GetScanner(string scannerName);
}
