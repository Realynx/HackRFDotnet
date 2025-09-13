namespace HackRFDotnet.ManagedApi.Streams.Buffers;

internal sealed class RingBuffer<T> : UnsafeRingBuffer<T> {
    private int _writeStart;
    private int _readStart;

    public int Count {
        get {
            var difference = _writeStart - _readStart;
            return difference > 0 ? difference : difference * -1;
        }
    }

    public RingBuffer(int capacity) : base(capacity) {
    }

    /// <summary>
    /// Writes items to the ring buffer.
    /// </summary>
    public void Write(ReadOnlySpan<T> buffer) {
        Write(buffer, buffer.Length);
    }

    /// <summary>
    /// Writes items to the ring buffer.
    /// </summary>
    public void Write(ReadOnlySpan<T> buffer, int count) {
        Write(buffer, _writeStart, count);

        var newWritePoint = (_writeStart + count) % Length;
        if (_writeStart < _readStart && newWritePoint > _readStart) {
            _readStart = newWritePoint + 1;
        }

        _writeStart = newWritePoint;
    }

    /// <summary>
    /// Reads items from the ringer buffer without modifying the start and end pointers.
    /// </summary>
    /// <returns>The number of items read.</returns>
    public int Peek(Span<T> buffer, int count) {
        var bytesRead = ReadSpan(buffer, _writeStart, count);
        return bytesRead;
    }

    /// <summary>
    /// Consumes items from the ringer buffer.
    /// </summary>
    /// <returns>The number of items read</returns>
    public int Read(Span<T> buffer) {
        return Read(buffer, buffer.Length);
    }


    /// <summary>
    /// Consumes items from the ringer buffer.
    /// </summary>
    /// <returns>The number of items read</returns>
    public int Read(Span<T> buffer, int count) {
        var bytesRead = Peek(buffer, count);
        _readStart = (_readStart + bytesRead) % Length;

        return bytesRead;
    }
}