namespace MarketOps.Scanner.Scanners.Calculators;

/// <summary>
/// Calculates high-low channel on provided data.
/// </summary>
internal static class HLChannel
{
    public static HLChannelData Calculate(in float[] highs, in float[] lows, int period) =>
        CanCalculate(highs, lows, period)
            ? CalculateHLChannelData(highs, lows, period)
            : new([], []);

    private static HLChannelData CalculateHLChannelData(in float[] highs, in float[] lows, int period) =>
        new(
            RingBufferCalculator<float>.Calculate(highs, period, (data) => data.Max()),
            RingBufferCalculator<float>.Calculate(lows, period, (data) => data.Min())
            );

    private static bool CanCalculate(in float[] highs, in float[] lows, int period) =>
        highs.Length == lows.Length
        && highs.Length >= period
        && lows.Length >= period
        && period > 0;
}
