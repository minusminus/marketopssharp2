namespace MarketOps.Scanner.Scanners.Calculators;

/// <summary>
/// Calculates high-low channel on provided data.
/// </summary>
internal static class HLChannel
{
    public static HLChannelData Calculate(float[] highs, float[] lows, int period) =>
        CanCalculate(highs, lows, period)
            ? CalculateHLChannelData(highs, lows, period)
            : new([], []);

    private static HLChannelData CalculateHLChannelData(float[] highs, float[] lows, int period) =>
        new(
            RingBufferCalculator<float>.Calculate(highs, period, (data) => data.Max()),
            RingBufferCalculator<float>.Calculate(lows, period, (data) => data.Min())
            );

    private static bool CanCalculate(float[] highs, float[] lows, int period) =>
        highs.Length == lows.Length
        && highs.Length >= period
        && lows.Length >= period
        && period > 0;
}
