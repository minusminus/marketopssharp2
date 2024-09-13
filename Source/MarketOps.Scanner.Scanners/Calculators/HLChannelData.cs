namespace MarketOps.Scanner.Scanners.Calculators;

/// <summary>
/// High-low channel result data.
/// </summary>
internal record HLChannelData(
    float[] H,
    float[] L);