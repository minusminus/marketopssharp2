namespace MarketOps.Scanner.Abstractions;

/// <summary>
/// Interface for scanning execution.
/// </summary>
internal interface IScanningExecutor
{
    public Task Execute(CancellationToken token);
}
