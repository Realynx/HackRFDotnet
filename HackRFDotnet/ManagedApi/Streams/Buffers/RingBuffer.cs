namespace HackRFDotnet.ManagedApi.Streams.Buffers;

internal sealed class RingBuffer<T> : UnsafeRingBuffer<T> {
    private int _writeStart;
    private int _readStart;
    private bool _empty = true;
    private bool _full = false;

    public int AvailableBytes {
        get {
            if (_full) {
                return Length;
            }

            if (_empty) {
                return 0;
            }

            var difference = _writeStart - _readStart;
            return difference < 0 ? Length + difference : difference;
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

        var looped = (_writeStart + count) >= Length;
        if (newWritePoint == _readStart ||
            (looped && _readStart > _writeStart)
            || (newWritePoint > _readStart && _writeStart < _readStart)) {
            _readStart = newWritePoint;
            _full = true;
        }
        else {
            _full = false;
        }

        _empty = false;
        _writeStart = newWritePoint;
    }

    /// <summary>
    /// Reads items from the ringer buffer without modifying the start and end pointers.
    /// </summary>
    /// <returns>The number of items read.</returns>
    public int Peek(Span<T> buffer, int count) {
        var bytesRead = ReadSpan(buffer, _readStart, count);
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

        var newReadPoint = (_readStart + bytesRead) % Length;

        var looped = (_readStart + count) >= Length;
        if (newReadPoint == _writeStart ||
            (looped && _writeStart > _readStart)
            || (newReadPoint > _writeStart && _readStart < _writeStart)) {
            _readStart = _writeStart;
            _empty = true;

            return bytesRead;
        }

        _readStart = newReadPoint;
        return bytesRead;
    }
}