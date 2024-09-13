namespace MarketOps.Scanner.Common;

public record StockData(
    int Count,
    float[] O,
    float[] H,
    float[] L,
    float[] C,
    Int64[] V,
    DateTime[] Ts);