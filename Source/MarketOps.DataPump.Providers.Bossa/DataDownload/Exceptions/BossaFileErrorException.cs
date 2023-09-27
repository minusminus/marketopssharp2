using MarketOps.DataPump.Common;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.Exceptions;

internal class BossaFileErrorException : DataPumpException
{
    public BossaFileErrorException(string zipFilePath, string fileName) 
        : base($"[{fileName}] not found in [{zipFilePath}]")
    { }
}
