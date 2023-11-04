namespace MarketOps.DataPump.Providers.Bossa.Common;

/// <summary>
/// Daily data files' definitions.
/// </summary>
internal static class BossaDaily
{
    public const char FieldsSeparator = ',';
    public const char NumberSeparator = '.';
    public const int StandardFieldsInLine = 7;
    public const int StockType2FieldsInLine = 8;    //fw contacts have openint in additional column
    public const int DtLength = 8;
}
