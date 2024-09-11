namespace MarketOps.Scanner.Common;

public readonly struct ScanResult
{
    public readonly ScanResultSignal Signal;
    public readonly Dictionary<string, object>? AdditionalData;

    public ScanResult(ScanResultSignal signal, Dictionary<string, object>? additionalData)
    {
        Signal = signal;
        AdditionalData = additionalData;
    }

    public static ScanResult[] Initialize(int count) => 
        Enumerable
        .Range(0, count)
        .Select(_ => new ScanResult(ScanResultSignal.Signal, null))
        .ToArray();

    public static ScanResult Signaled(Dictionary<string, object>? additionalData) =>
        new(ScanResultSignal.Signal, additionalData);

    public static ScanResult NotSignaled(Dictionary<string, object>? additionalData) =>
        new(ScanResultSignal.NoSignal, additionalData);
}

public enum ScanResultSignal 
{ 
    Uspecified,
    Signal,
    NoSignal
};
