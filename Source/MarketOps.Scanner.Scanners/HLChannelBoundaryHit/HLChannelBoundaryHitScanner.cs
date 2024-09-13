using MarketOps.Scanner.Common;
using MarketOps.Scanner.Scanners.Calculators;
using System.Text.Json;

namespace MarketOps.Scanner.Scanners.HLChannelBoundaryHit;

/// <summary>
/// Scans for lower boundary hit in price HL channel.
/// </summary>
public class HLChannelBoundaryHitScanner : IScanner
{
    private HLChannelBoundaryHitScannerParams _scannerParams = HLChannelBoundaryHitScannerParams.Default();

    public void SetParameters(string parametersJson)
    {
        _scannerParams = JsonSerializer.Deserialize<HLChannelBoundaryHitScannerParams>(parametersJson)
            ?? throw new Exception("Null object of parameters deserialization");
    }

    public string GetCurrentParameters() => 
        _scannerParams.ToString();

    public void Scan(StockData data, in ScanResult[] result)
    {
        var hlChannel = HLChannel.Calculate(data.H, data.L, _scannerParams.ChannelLength);

        int scansToCalculate = Math.Min(result.Length, hlChannel.H.Length);
        for (int i = 0; i < scansToCalculate; i++)
        {
            float lowerMargin = GetLowerPriceMargin(hlChannel, i, _scannerParams.HitMarginPcnt);
            result[i] = LowerMarginHit(data, i, lowerMargin)
                ? ScanResult.Signaled()
                : ScanResult.NotSignaled();
        }
    }

    private static float GetLowerPriceMargin(HLChannelData hlChannel, int index, float hitMarginPcnt) => 
        (hlChannel.H[index] - hlChannel.L[index]) * hitMarginPcnt + hlChannel.L[index];

    private static bool LowerMarginHit(StockData data, int index, float lowerMargin) => 
        (data.L[index] <= lowerMargin);
}
