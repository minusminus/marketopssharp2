﻿using MarketOps.DataPump.Common;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Downloading;
using MarketOps.DataPump.Providers.Bossa.DataDownload.Exceptions;
using MarketOps.Types;
using System.IO.Compression;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;

/// <summary>
/// Buffers downloaded files on disk.
/// </summary>
internal class DiskBuffer : IDownloadBuffer
{
    private readonly BossaDownloader _downloader;
    private readonly string _downloadPath = Path.Combine(Consts.ExecutingLocation, nameof(DiskBuffer));

    private readonly Dictionary<StockType, string> _dailyFilesBuffer = new();

    public DiskBuffer(BossaDownloader downloader)
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
        string zipFilePath;
        if(!_dailyFilesBuffer.TryGetValue(stockDefinition.Type, out zipFilePath!))
        {
            zipFilePath = Path.Combine(_downloadPath, $"{stockDefinition.Type}.zip");
            DownloadFile(PumpingDataRange.Daily, stockDefinition, zipFilePath);
            _dailyFilesBuffer.Add(stockDefinition.Type, zipFilePath);
        }

        return GetDataFromZip(zipFilePath, $"{stockDefinition.Name}.mst");
    }

    private void DownloadFile(PumpingDataRange dataRange, StockDefinitionShort stockDefinition, string zipFilePath) =>
        _downloader.Get(dataRange, stockDefinition.Type, stockDefinition.Name,
            stream =>
            {
                using var fs = File.OpenWrite(zipFilePath);
                stream.CopyTo(fs);
                fs.Flush();
            });

    private BufferEntry GetDataFromZip(string zipFilePath, string fileName) => 
        DiskBufferEntry.Create(zipFilePath, fileName);
}
