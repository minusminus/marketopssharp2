using MarketOps.DataPump.Providers.Bossa.DataDownload.Exceptions;
using System.IO.Compression;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;

/// <summary>
/// Buffer entry from disk buffer.
/// </summary>
internal class DiskBufferEntry : BufferEntry
{
    private readonly string _zipFilePath;
    private readonly string _fileName;
    private readonly FileStream _fileStream;
    private readonly ZipArchive _zipArchive;
    private ZipArchiveEntry? _entry;

    private DiskBufferEntry(string zipFilePath, string fileName, FileStream fileStream, ZipArchive zipArchive, ZipArchiveEntry? entry)
    {
        _zipFilePath = zipFilePath;
        _fileName = fileName;
        _fileStream = fileStream;
        _zipArchive = zipArchive;
        _entry = entry;
    }

    public static BufferEntry Create(string zipFilePath, string fileName)
    {
        var fs = File.OpenRead(zipFilePath);
        var zip = new ZipArchive(fs, ZipArchiveMode.Read);
        var entry = zip.GetEntry(fileName);
        return new DiskBufferEntry(zipFilePath, fileName, fs, zip, entry);
    }

    public override StreamReader GetStream() => 
        _entry is not null
            ? new StreamReader(_entry.Open())
            : throw new BossaFileErrorException(_zipFilePath, _fileName);

    protected override void DisposeManagedObjects()
    {
        _entry = null;
        _zipArchive.Dispose();
        _fileStream.Dispose();
    }
}
