using MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;

namespace MarketOps.Tests.Mocks.Types;

/// <summary>
/// Mocked BufferEntry class returning null stream.
/// </summary>
internal class MockNullBufferEntry : BufferEntry
{
    public override StreamReader? GetStream() => 
        null;

    protected override void DisposeManagedObjects()
    { }
}
