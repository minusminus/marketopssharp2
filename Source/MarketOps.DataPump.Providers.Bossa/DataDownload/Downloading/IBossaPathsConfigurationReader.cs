using MarketOps.DataPump.Providers.Bossa.DataDownload.Types;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.Downloading;

/// <summary>
/// Interface to read bossa configuration paths.
/// </summary>
internal interface IBossaPathsConfigurationReader
{
    public BossaPaths Read();
}
