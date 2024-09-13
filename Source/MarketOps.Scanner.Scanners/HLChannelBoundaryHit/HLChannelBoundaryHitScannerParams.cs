namespace MarketOps.Scanner.Scanners.HLChannelBoundaryHit;

/// <summary>
/// Example JSON for cmdline:
/// "{ \"ChannelLength\": 100, \"HitMarginPcnt\": 0.05 }"
/// </summary>
internal record HLChannelBoundaryHitScannerParams(
    int ChannelLength,
    float HitMarginPcnt)
{
    public static HLChannelBoundaryHitScannerParams Default() =>
        new(100, 0.05f);
}