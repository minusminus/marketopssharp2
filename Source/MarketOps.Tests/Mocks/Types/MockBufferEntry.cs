using MarketOps.DataPump.Providers.Bossa.DataDownload.DownloadBuffering;
using System.Text;

namespace MarketOps.Tests.Mocks.Types;

/// <summary>
/// Mocked BufferEntry class returning specified string content.
/// </summary>
internal class MockBufferEntry : BufferEntry
{
    private readonly MemoryStream _contentStream;

    public MockBufferEntry(string content)
    {
        var contentArray = Encoding.UTF8.GetBytes(content);
        _contentStream = new MemoryStream(contentArray);
    }

    public override StreamReader GetStream() =>
        new(_contentStream);

    protected override void DisposeManagedObjects()
    {
        _contentStream.Dispose();
    }
}
