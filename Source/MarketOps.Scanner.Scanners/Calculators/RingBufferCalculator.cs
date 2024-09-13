namespace MarketOps.Scanner.Scanners.Calculators;

/// <summary>
/// Executes operation on ring buffer with specified length.
/// Returns array of calculated values.
/// </summary>
internal static class RingBufferCalculator<T>
{
    public static T[] Calculate(in T[] data, int bufferLength, Func<T[], T> operation) =>
        Calculate(data, bufferLength, data.Length - bufferLength + 1, operation);

    public static T[] Calculate(in T[] data, int bufferLength, int resultLength, Func<T[], T> operation)
    {
        if (!CanCalculate(data, bufferLength, resultLength)) return [];

        var buffer = new RingBuffer<T>(bufferLength);
        InitialBufferFill(buffer, data);
        return CalculateResult(buffer, data, resultLength, operation);
    }

    private static bool CanCalculate(in T[] data, int bufferLength, int resultLength) =>
        data.Length >= bufferLength + resultLength - 1 && bufferLength > 0 && resultLength > 0;

    private static void InitialBufferFill(RingBuffer<T> buffer, T[] data)
    {
        for (int i = 0; i < buffer.Length - 1; i++)
            buffer.Add(data[i]);
    }

    private static T[] CalculateResult(RingBuffer<T> buffer, in T[] data, int resultLength, Func<T[], T> operation)
    {
        T[] res = new T[resultLength];

        for (int i = buffer.Length - 1; i < buffer.Length + resultLength - 1; i++)
        {
            buffer.Add(data[i]);
            res[i - (buffer.Length - 1)] = operation(buffer.Buffer);
        }

        return res;
    }
}
