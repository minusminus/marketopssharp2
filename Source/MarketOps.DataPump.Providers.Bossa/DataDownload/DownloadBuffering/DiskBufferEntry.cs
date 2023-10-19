using System.IO.Compression;

namespace MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;

/// <summary>
/// Buffer entry from disk buffer.
/// File stream is opened in shared mode, to be readable by multiple tasks.
/// </summary>
internal class DiskBufferEntry : BufferEntry
{
    private readonly FileStream _fileStream;
    private readonly ZipArchive _zipArchive;
    private ZipArchiveEntry? _entry;

    private DiskBufferEntry(FileStream fileStream, ZipArchive zipArchive, ZipArchiveEntry? entry)
    {
        _fileStream = fileStream;
        _zipArchive = zipArchive;
        _entry = entry;
    }

    public static BufferEntry Create(string zipFilePath, string fileName)
    {
        var fs = new FileStream(zipFilePath, new FileStreamOptions()
        {
            Access = FileAccess.Read,
            Mode = FileMode.Open,
            Share = FileShare.Read
        });
        var zip = new ZipArchive(fs, ZipArchiveMode.Read);
        var entry = zip.GetEntry(fileName);
        return new DiskBufferEntry(fs, zip, entry);
    }

    public override StreamReader? GetStream() =>
        _entry is not null
            ? new StreamReader(_entry.Open())
            : null;

    protected override void DisposeManagedObjects()
    {
        _entry = null;
        _zipArchive.Dispose();
        _fileStream.Dispose();
    }
}
