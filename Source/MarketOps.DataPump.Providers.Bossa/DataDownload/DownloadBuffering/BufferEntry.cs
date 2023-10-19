namespace MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;

/// <summary>
/// Base class for buffer items to properly manage disposable objects.
/// Only returned StreamReader should not be internally disposed.
/// </summary>
internal abstract class BufferEntry : IDisposable
{
    private bool disposedValue;

    protected void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
                DisposeManagedObjects();
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected abstract void DisposeManagedObjects();

    public abstract StreamReader? GetStream();
}
