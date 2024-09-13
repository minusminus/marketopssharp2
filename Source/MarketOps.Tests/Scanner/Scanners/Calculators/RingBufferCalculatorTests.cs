using MarketOps.Scanner.Scanners.Calculators;

namespace MarketOps.Tests.Scanner.Scanners.Calculators;

[TestFixture]
public class RingBufferCalculatorTests
{
    [TestCase(new[] { 1f, 2f, 3f }, 3, new[] { 1f })]
    [TestCase(new[] { 3f, 2f, 1f }, 3, new[] { 1f })]
    [TestCase(new[] { 1f, 2f, 3f, 4f, 5f }, 3, new[] { 1f, 2f, 3f })]
    [TestCase(new[] { 5f, 4f, 3f, 2f, 1f }, 3, new[] { 3f, 2f, 1f })]
    [TestCase(new[] { 1f, 2f }, 3, new float[0])]
    [TestCase(new float[0], 3, new float[0])]
    public void Calculate__CalculatesCorrectly(float[] input, int bufferLength, float[] expected)
    {
        RingBufferCalculator<float>.Calculate(input, bufferLength, (data) => data.Min()).ShouldBe(expected);
    }
}
