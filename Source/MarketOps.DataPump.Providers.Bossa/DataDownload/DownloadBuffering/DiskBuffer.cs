using MarketOps.DataPump.Common;
using MarketOps.Types;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;

/// <summary>
/// Buffers downloaded files on disk.
/// 
/// Has simple locking on buffer dictionary access, so can be instantiated as a singleton.
/// This simple lock prevents parallel downloading of the same file, but does not allow to download different files in parallel.
/// For daily data with one file for one stock type it should be good enough.
/// </summary>
internal class DiskBuffer : IDownloadBuffer
{
    private readonly IBossaDownloader _downloader;
    private readonly string _downloadPath = Path.Combine(Consts.ExecutingLocation, nameof(DiskBuffer));

    private readonly object _dailyBufferLock = new();
    private readonly Dictionary<StockType, string> _dailyFilesBuffer = new();

    public DiskBuffer(IBossaDownloader downloader)
    {
        _downloader = downloader;
        Directory.CreateDirectory(_downloadPath);
    }

    public BufferEntry GetFile(PumpingDataRange dataRange, StockDefinitionShort stockDefinition) =>
        dataRange switch
        {
            PumpingDataRange.Daily => GetDailyFile(stockDefinition),
            _ => throw new ArgumentException($"Not supported data range {dataRange}"),
        };


    private BufferEntry GetDailyFile(StockDefinitionShort stockDefinition)
    {
        string zipFilePath = GetDailyZipFilePath(stockDefinition);
        return GetDataFromZip(zipFilePath, $"{stockDefinition.Name}.mst");
    }

    private string GetDailyZipFilePath(StockDefinitionShort stockDefinition)
    {
        string zipFilePath;
        lock (_dailyBufferLock)
        {
            if (!GetFromDailyBuffer(stockDefinition.Type, out zipFilePath))
                zipFilePath = DownloadAndStoreInDailyBuffer(stockDefinition);
        }
        return zipFilePath;
    }

    private bool GetFromDailyBuffer(StockType stockType, out string zipFilePath) => 
        _dailyFilesBuffer.TryGetValue(stockType, out zipFilePath!);

    private string DownloadAndStoreInDailyBuffer(StockDefinitionShort stockDefinition)
    {
        string zipFilePath = Path.Combine(_downloadPath, $"{stockDefinition.Type}.zip");
        DownloadFile(PumpingDataRange.Daily, stockDefinition, zipFilePath);
        _dailyFilesBuffer.Add(stockDefinition.Type, zipFilePath);
        return zipFilePath;
    }

    private void DownloadFile(PumpingDataRange dataRange, StockDefinitionShort stockDefinition, string zipFilePath) =>
        _downloader.Get(dataRange, stockDefinition.Type, stockDefinition.Name,
            stream =>
            {
                using var fs = File.OpenWrite(zipFilePath);
                stream.CopyTo(fs);
                fs.Flush();
            });

    private static BufferEntry GetDataFromZip(string zipFilePath, string fileName) => 
        DiskBufferEntry.Create(zipFilePath, fileName);
}
