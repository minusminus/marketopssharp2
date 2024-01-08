namespace MarketOps.DataPump.Providers.PkoFunds.Common;

/// <summary>
/// PKO funds csv files' definition.
/// </summary>
internal static class PkoCsvData
{
    public const char FieldsSeparator = ';';
    public const string DateFormat = "yyyy-MM-dd";

    public const int DateIndex = 0;

    public const int NotFoundDataIndex = -1;

    public const char DateSeparator = '-';
    public const char PriceSeparator = ',';
}
