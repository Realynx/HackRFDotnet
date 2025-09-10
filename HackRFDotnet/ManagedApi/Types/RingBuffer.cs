using System.Buffers;

namespace HackRFDotnet.ManagedApi.Types;

internal sealed class RingBuffer<T> {
    public int Capacity { get; }

    public int Count {
        get {
            if (_start < _end) {
                return _end - _start;
            }

            return Capacity - (_start - _end);
        }
    }

    private readonly T[] _array;
    private int _start;
    private int _end;

    public RingBuffer(int capacity) {
        Capacity = capacity;
        _array = new T[capacity];
    }

    public void Write(ReadOnlySpan<T> buffer) {
        // Buffer larger than capacity - only write end of buffer
        if (buffer.Length >= Capacity) {
            buffer[^Capacity..].CopyTo(_array);
            _start = 0;
            _end = Capacity;
            return;
        }

        // Buffer smaller than capacity at end of No need to wrap around
        var freeUntilWrap = Capacity - _end;
        if (buffer.Length <= freeUntilWrap) {
            buffer.CopyTo(_array.AsSpan(_end));
            _end += buffer.Length;
            return;
        }

        // Buffer is larger enough that two copies are needed
        buffer[..freeUntilWrap].CopyTo(_array.AsSpan(_end));
        buffer[freeUntilWrap..].CopyTo(_array);

        // Advance pointers
        if (_end + buffer.Length > _start) {
            _end += buffer.Length;
            _end %= Capacity;
            _start = _end;
        }
        else {
            _end += buffer.Length;
        }
    }

    public ReadOnlySequence<T> Peek() {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Reads items from the ringer buffer without advancing the end pointer
    /// </summary>
    /// <returns>The number of items read</returns>
    public int Peek(Span<T> buffer) {
        // [||||||||||||||||||||||||||||]
        //  ^ start                 ^ end
        if (_start < _end) {
            if (buffer.Length <= _end - _start) {
                _array.AsSpan(_start, buffer.Length - _start).CopyTo(buffer);
                return buffer.Length;
            }

            _array.AsSpan(_start, _end).CopyTo(buffer);
            return _end - _start;
        }
        // [||||||||||||||||||||||||||||]
        //  ^ end                 ^ start
        else {
            if (buffer.Length <= Capacity - _start) {
                _array.AsSpan(_start, Capacity - _start).CopyTo(buffer);
                return buffer.Length;
            }

            _array.AsSpan(_start, Capacity - _start).CopyTo(buffer);
            _array.AsSpan(0, _end).CopyTo(buffer[(Capacity - _start)..]);
            return Count;
        }
    }

    public ReadOnlySequence<T> Read(int count, out int read) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Reads items from the ringer buffer and advances the end pointer
    /// </summary>
    /// <returns>The number of items read</returns>
    public int Read(Span<T> buffer) {
        var read = Peek(buffer);

        // Advance pointers
        if (_start + read > _end) {
            _start = 0;
            _end = 0;
        }
        else {
            _start += read;
        }

        return read;
    }
}