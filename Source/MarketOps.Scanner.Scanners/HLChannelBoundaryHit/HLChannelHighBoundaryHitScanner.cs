using MarketOps.Scanner.Common;
using MarketOps.Scanner.Scanners.Calculators;
using System.Text.Json;

namespace MarketOps.Scanner.Scanners.HLChannelBoundaryHit;

/// <summary>
/// Scans for high bound hit in price HL channel.
/// </summary>
public class HLChannelHighBoundaryHitScanner : IScanner
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
            float higherMargin = GetHigherPriceMargin(hlChannel, i, _scannerParams.HitMarginPcnt);
            result[i] = HigherMarginHit(data, i, higherMargin)
                ? ScanResult.Signaled()
                : ScanResult.NotSignaled();
        }
    }

    private static float GetHigherPriceMargin(HLChannelData hlChannel, int index, float hitMarginPcnt) =>
         hlChannel.H[index] - (hlChannel.H[index] - hlChannel.L[index]) * hitMarginPcnt;

    private static bool HigherMarginHit(StockData data, int index, float higherMargin) =>
        (data.H[index] >= higherMargin);
}
