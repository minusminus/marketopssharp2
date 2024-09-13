namespace MarketOps.Scanner.Scanners.Calculators;

/// <summary>
/// Cyclic buffer of elements of type T.
/// Returns correct elements after initial fill (Filled == true).
/// </summary>
internal class RingBuffer<T>
{
    private readonly T[] _buffer;
    private int _startIndex = 0;
    private bool _filled = false;

    public T[] Buffer => _buffer;
    public int Length => _buffer.Length;
    public bool Filled => _filled;

    public RingBuffer(int length)
    {
        _buffer = new T[length];
    }

    public void Add(T element)
    {
        _buffer[_startIndex] = element;
        _filled = _filled || _startIndex + 1 == _buffer.Length;
        _startIndex = WrapBufferIndex(_startIndex + 1);
    }

    public T ElementAt(int index) => _buffer[PositionInBuffer(index)];

    private int PositionInBuffer(int index) => WrapBufferIndex(_startIndex + index);

    private int WrapBufferIndex(int index) => index % _buffer.Length;
}
